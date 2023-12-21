using api;
using api.Middleware;
using infrastructure;
using infrastructure.repository;
using service;
using service.services;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString,
        dataSourceBuilder => dataSourceBuilder.EnableParameterLogging());
}

if (builder.Environment.IsProduction())
{
    builder.Services.AddNpgsqlDataSource(Utilities.ProperlyFormattedConnectionString);
}
builder.Services.AddAvatarBlobService();
builder.Services.AddSingleton<TransactionCalculator>();
builder.Services.AddSingleton<NotificationFacade>();
builder.Services.AddSingleton<HttpClient>();

builder.Services.AddSingleton<UserRepository>();
builder.Services.AddSingleton<PasswordHashRepository>();
builder.Services.AddSingleton<GroupRepository>();
builder.Services.AddSingleton<ExpenseRepository>();
builder.Services.AddSingleton<MailRepository>();
builder.Services.AddSingleton<CurrencyApiRepository>();
builder.Services.AddSingleton<NotificationRepository>();


builder.Services.AddSingleton<NotificationService>();
builder.Services.AddSingleton<AccountService>();
builder.Services.AddSingleton<UserService>();
builder.Services.AddSingleton<GroupService>();
builder.Services.AddSingleton<ExpenseService>();    

builder.Services.AddJwtService();
builder.Services.AddSwaggerGenWithBearerJWT();

var policyCollection = new HeaderPolicyCollection()
    .AddDefaultSecurityHeaders()
    .AddCrossOriginResourcePolicy(builder =>
    {
        builder.SameOrigin(); // Tillad kun anmodninger til samme oprindelse
    })
    .AddCustomHeader("X-Frame-Options", "DENY") // Forebyg UI-redigering fra tredjepart
    .AddCustomHeader("X-XSS-Protection", "1; mode=block") // Aktiver XSS-beskyttelse
    .AddCustomHeader("X-Content-Type-Options", "nosniff"); // Forhindre MIME-sniffing;

var allowedOrigins = new[] { "frontend link bro" }; // TODO tilføj urls der skal kunne tilgå vores side aka frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder => builder.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod());
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseAuthentication();
app.UseAuthorization(); 

app.MapControllers();

app.UseSecurityHeaders(policyCollection);

app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<ProtocolWhitelistMiddleware>();
app.UseMiddleware<JwtBearerHandler>();
app.UseMiddleware<GlobalExceptionHandler>();
app.Run();
