using JobTrackerVSA.Web.Data;
using JobTrackerVSA.Web.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using JobTrackerVSA.Web.Features.Maintenance;
using JobTrackerVSA.Web.Features.JobApplications.List;
using Scalar.AspNetCore;
using JobTrackerVSA.Web.Infrastructure.Storage;
using Azure.Storage.Blobs;
using JobTrackerVSA.Web.Features.JobApplications.ViewResume;

var builder = WebApplication.CreateBuilder(args);


// Load local configuration not committed to git
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true);

// Add Application Insights Telemetry
builder.Services.AddApplicationInsightsTelemetry();

// Add services to the container.
builder.Services.AddOpenApi();

// Global Authorization Policy (Secure by Default)
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// Auth0 Configuration
builder.Services.AddAuth0WebAppAuthentication(options => {
    options.Domain = builder.Configuration["Auth0:Domain"] ?? "";
    options.ClientId = builder.Configuration["Auth0:ClientId"] ?? "";
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"] ?? "";
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Register interceptor as a singleton so tests and DI can replace/spy it if needed.
builder.Services.AddSingleton<InterviewStatusInterceptor>();

builder.Services.AddDbContext<AppDbContext>((serviceProvider, options) =>
    options
        .UseSqlServer(connectionString)
        .AddInterceptors(serviceProvider.GetRequiredService<InterviewStatusInterceptor>()));


builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// Auth Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<JobTrackerVSA.Web.Infrastructure.Auth.ICurrentUserService, JobTrackerVSA.Web.Infrastructure.Auth.CurrentUserService>();

// Blob Storage Service
builder.Services.Configure<BlobStorageSettings>(builder.Configuration.GetSection("BlobStorage"));
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("BlobStorage") ?? builder.Configuration["BlobStorage:ConnectionString"]));
builder.Services.AddScoped<IResumeStorageService, AzureBlobResumeStorageService>();

// Add services to the container.
builder.Services.AddRazorPages()
    .WithRazorPagesRoot("/Features");

builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapOpenApi().AllowAnonymous();
app.MapScalarApiReference().AllowAnonymous();

app.UseHttpsRedirection();

app.UseRouting();

app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapHealthChecks("/health").AllowAnonymous();

app.MapMaintenanceEndpoints();
app.MapJobApplicationsEndpoints();
app.MapViewResumeEndpoint();

app.Run();
