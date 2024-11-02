using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

var builder = WebApplication.CreateBuilder(args);

var connectionString = $"Server={Environment.GetEnvironmentVariable("DB_SERVER")};" +
                       $"Database={Environment.GetEnvironmentVariable("DB_NAME")};" +
                       $"User Id={Environment.GetEnvironmentVariable("DB_USER")};" +
                       $"Password={Environment.GetEnvironmentVariable("DB_PASSWORD")};";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/procedimentos", async (AppDbContext db) =>
{
    return await db.Agendamentos.ToListAsync();
})
.WithName("GetProcedimentos")
.WithOpenApi();

app.MapPost("/procedimentos", async (AppDbContext db, Agendamento novoProcedimento) =>
{
    db.Agendamentos.Add(novoProcedimento);
    await db.SaveChangesAsync();
    return Results.Created($"/procedimentos/{novoProcedimento.Id}", novoProcedimento);
})
.WithName("AddProcedimento")
.WithOpenApi();

app.Run();

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Agendamento> Agendamentos { get; set; }
}

public class Agendamento
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public string? TipoDeProcedimento { get; set; }
    public DateTime? Data { get; set; }
    public string? Telefone { get; set; }
}
