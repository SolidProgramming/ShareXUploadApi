global using ShareXUploadApi.Services;
global using System.Net;
global using ShareXUploadApi.Classes;
global using ShareXUploadApi.Models;
global using Microsoft.EntityFrameworkCore.Design;
//global using ShareXUploadApi.Interfaces;
global using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http.Connections;
using MySql.Data.MySqlClient;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();


builder.Services.AddHttpClient();
builder.Services.AddSingleton<ILinkService, LinkService>();
builder.Services.AddSingleton<IFileService, FileService>();
builder.Services.AddSingleton<IDBService, DBService>();
builder.Services.AddLogging();
builder.Services.AddSingleton<MySqlConnection>();

var app = builder.Build();



app.MapPost("sharex/upload",
    async (IFileService fileService, IDBService dbService, ILogger<DBService> loggerDBService, ILogger<FileService> loggerFileService, IConfiguration config, MySqlConnection mysqlConn, HttpRequest request) =>
{
    if (!request.Form.Files.Any())
    {
        return Results.BadRequest("No file uploaded");
    }

    if (request.Form.Files.Count > 1)
    {
        return Results.BadRequest("Too many files. Limit = 1");
    }

    FileModel file = new()
    {
        Guid = Guid.NewGuid().ToString(),
        Filename = request.Form.Files[0].FileName,
        File = request.Form.Files[0]
    };

    await dbService.InsertFileDataAsync(file);

    (string? Message, HttpStatusCode StatusCode) = await fileService.UploadAsync(file);

    if (StatusCode == HttpStatusCode.OK) return Results.Ok(Message);

    return Results.Problem(Message);
});

app.MapGet("sharex/getsharelink",
    async (ILinkService linkService, [FromQuery] string guid) =>
{
    if (linkService is null) return Results.BadRequest();

    if (string.IsNullOrEmpty(guid)) return Results.BadRequest();

    (string? Message, HttpStatusCode StatusCode) = await linkService.GetLinkByGuidAsync(guid);

    if (StatusCode != HttpStatusCode.OK) return Results.BadRequest();

    return Results.Ok(Message);
});



// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
