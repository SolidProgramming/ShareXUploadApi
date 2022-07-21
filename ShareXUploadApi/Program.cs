global using ShareXUploadApi.Services;
global using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<ILinkService, LinkService>();
builder.Services.AddSingleton<IFileService, FileService>();

var app = builder.Build();

app.MapPost("sharex/upload", async (IFileService fileService, HttpRequest request) =>
{
    if (!request.Form.Files.Any())
    {
        return Results.BadRequest("No file uploaded");
    }

    if (request.Form.Files.Count > 1)
    {
        return Results.BadRequest("Too many files. Limit = 1");
    }

    (string? Message, HttpStatusCode StatusCode) = await fileService.UploadAsync(request);

    if (StatusCode == HttpStatusCode.OK) return Results.Ok(Message);

    return Results.Problem(Message);
});

app.MapGet("sharex/getsharelink", async (ILinkService linkService, [FromQuery] string guid) =>
{
    if (linkService is null) return Results.BadRequest();

    if (string.IsNullOrEmpty(guid)) return Results.BadRequest();

    (string? Message, HttpStatusCode StatusCode) = await linkService.GetLinkByGuidAsync(guid);

    if (StatusCode != HttpStatusCode.OK) return Results.BadRequest();

    return Results.Ok(Message);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
