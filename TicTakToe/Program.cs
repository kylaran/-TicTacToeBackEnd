using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Quartz;
using TicTakToe;
using TicTakToe.Context;
using TicTakToe.Jobs;

var builder = WebApplication.CreateBuilder(args);

var pathBase = builder.Configuration["PATH_BASE"];


builder.Logging
    .AddSimpleConsole(o =>
    {
        o.TimestampFormat = "HH:mm:ss ";
        o.UseUtcTimestamp = true;
    });

builder.Services.AddDbContext<TicTacToeDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("TicTacToeConnection")), ServiceLifetime.Transient, ServiceLifetime.Transient);

builder.Services.AddQuartz(q =>
{
    q.UseMicrosoftDependencyInjectionJobFactory();

    var jobKey = new JobKey("CloseExistGamesJob");

    q.AddJob<CloseExistGamesJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
      .ForJob(jobKey)
      .WithIdentity("CloseExistGamesJob-trigger")
      .WithSimpleSchedule(x => 
      x.WithInterval(TimeSpan.FromSeconds(30))
      .RepeatForever()));
 });

builder.Services.AddQuartzHostedService(
       q => q.WaitForJobsToComplete = true);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.AddControllers();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.MigrateDatabase<TicTacToeDbContext>();

if (!string.IsNullOrEmpty(pathBase))
    app.UsePathBase(pathBase);

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
