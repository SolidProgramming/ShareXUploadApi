global using ShareXUploadApi.Services;
global using System.Net;
global using ShareXUploadApi.Classes;
global using ShareXUploadApi.Models;
global using Microsoft.EntityFrameworkCore.Design;
//global using ShareXUploadApi.Interfaces;
global using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//if(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
//{
//    builder.Services.AddEndpointsApiExplorer();
//    builder.Services.AddSwaggerGen();
//}

builder.Services.AddAuthentication("BasicAuthentication")
   .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>
                ("BasicAuthentication", null);
builder.Services.AddAuthorization();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<ILinkService, LinkService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IDBService, DBService>();
builder.Services.AddLogging();
builder.Services.AddSingleton<MySqlConnection>();


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors();

app.MapPost("sharex/upload", [Authorize]
async (IFileService fileService, IDBService dbService, ILogger<DBService> loggerDBService, ILogger<FileService> loggerFileService, ILinkService linkService, IConfiguration config, MySqlConnection mysqlConn, HttpRequest request, HttpContext context) =>
{
    FileUploadResponseModel apiResponse = new();

    if (!request.Form.Files.Any())
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        apiResponse.ErrorMessage = "No files uploaded";
        apiResponse.Success = false;
        await context.Response.WriteAsJsonAsync(apiResponse);
        return;
    }

    if (request.Form.Files.Count > 1)
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        apiResponse.ErrorMessage = "To many files uploaded. Upload limit = 1";
        apiResponse.Success = false;
        await context.Response.WriteAsJsonAsync(apiResponse);
        return;
    }

    FileModel file = new()
    {
        Guid = Guid.NewGuid().ToString(),
        FileExtension = request.Form.Files[0].FileName,
        File = request.Form.Files[0]
    };

    (bool insertSuccess, string? insertErrorMessage) = await dbService.InsertFileDataAsync(file);

    if (!insertSuccess)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        apiResponse.ErrorMessage = insertErrorMessage;
        apiResponse.Success = false;
        await context.Response.WriteAsJsonAsync(apiResponse);
        return;
    }

    (bool uploadSuccess, string? uploadErrorMessage) = await fileService.UploadAsync(file);

    if (!uploadSuccess)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        apiResponse.ErrorMessage = uploadErrorMessage;
        apiResponse.Success = false;
        await context.Response.WriteAsJsonAsync(apiResponse);
        return;
    }

    (bool linkSuccess, string? publicUrl, string? linkErrorMessage) = await linkService.GetLinkByGuidAsync(file.Guid);

    if (!linkSuccess)
    {
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        apiResponse.ErrorMessage = linkErrorMessage;
        apiResponse.Success = false;
        await context.Response.WriteAsJsonAsync(apiResponse);
        return;
    }

    apiResponse.Success = true;
    apiResponse.PublicUrl = publicUrl;
    apiResponse.Guid = file.Guid;

    await context.Response.WriteAsJsonAsync(apiResponse);
});


app.MapGet("urlshortener", [Authorize] async ([FromQuery] string guid, IDBService dbService, ILogger<DBService> loggerDBService, ILogger<FileService> loggerFileService, ILinkService linkService, IConfiguration config, MySqlConnection mysqlConn, HttpRequest request, HttpContext context) =>
{
    if (string.IsNullOrEmpty(guid))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsync("guid empty");
        return;
    }

    (bool shortLinkSuccess, string? shortLink, string? shortLinkErrorMessage) = await linkService.CreateShortLinkAsync(guid);

    if (shortLinkSuccess && !string.IsNullOrEmpty(shortLink))
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await context.Response.WriteAsync(shortLink);
        return;
    }

    if (shortLinkSuccess && !string.IsNullOrEmpty(shortLinkErrorMessage))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(shortLinkErrorMessage);
        return;
    }
});

app.MapGet("/p/urlshortener", async ([FromQuery] string url, IDBService dbService, ILogger<DBService> loggerDBService, ILogger<FileService> loggerFileService, ILinkService linkService, IConfiguration config, MySqlConnection mysqlConn, HttpRequest request, HttpContext context) =>
{
    if (string.IsNullOrEmpty(url))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsync("url empty");
        return;
    }

    (bool shortLinkSuccess, string? shortLink, string? shortLinkErrorMessage) = await linkService.CreatePublicShortLinkAsync(url);

    if (shortLinkSuccess && !string.IsNullOrEmpty(shortLink))
    {
        context.Response.StatusCode = (int)HttpStatusCode.OK;
        await context.Response.WriteAsync(shortLink);
        return;
    }

    if (shortLinkSuccess && !string.IsNullOrEmpty(shortLinkErrorMessage))
    {
        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(shortLinkErrorMessage);
        return;
    }
});

app.MapGet("/p/{linkId}", async (string linkId, HttpContext context, ILinkService linkService) =>
{
    //string path = context.Request.Path.ToUriComponent().Trim('/');

    (bool getLinkSuccess, string? publicUrl, string? getLinkErrorMessage) = await linkService.GetLinkByPublicShortLinkIdAsync(linkId);

    if (getLinkSuccess && !string.IsNullOrEmpty(publicUrl))
    {
        return Results.Redirect(publicUrl);
    }

    return Results.NotFound();
});

app.MapGet("/{linkId}",async (string linkId, HttpContext context, ILinkService linkService) =>
{
    //string path = context.Request.Path.ToUriComponent().Trim('/');

    (bool getLinkSuccess, string? publicUrl, string? getLinkErrorMessage) = await linkService.GetLinkByShortLinkIdAsync(linkId);

    if (getLinkSuccess && !string.IsNullOrEmpty(publicUrl))
    {
        return Results.Redirect(publicUrl);
    }

    return Results.NotFound();
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
