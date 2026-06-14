using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SameMess.API.Hubs;
using SameMess.API.Middleware;
using SameMess.API.Services;
using SameMess.Application;
using SameMess.Application.Interfaces.Services;
using SameMess.Infrastructure;
using SameMess.Infrastructure.Data;
using SameMess.Infrastructure.Services;

// App lưu thời gian dạng UTC (DateTime.UtcNow). Bật chế độ legacy của Npgsql để map
// DateTime -> "timestamp without time zone" và không bắt buộc Kind=Utc (tránh lỗi khi ghi timestamptz).
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Host PaaS (Render/Fly/Railway) cấp cổng động qua biến môi trường PORT → Kestrel lắng nghe đúng cổng đó.
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(port))
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

const string WebClientCorsPolicy = "WebClient";

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SameMess API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization. Enter: Bearer {token}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var jwtSection = builder.Configuration.GetSection("Jwt");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!)),
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };

    // SignalR (WebSocket) không gửi header Authorization được → đọc access_token từ query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                context.Token = accessToken;
            return Task.CompletedTask;
        }
    };
});

// CORS cho web client (đọc origin từ config "Cors:AllowedOrigins"; mặc định cho dev)
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy(WebClientCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()); // cần thiết để gửi/nhận refresh token cookie
});

// Lưu ảnh local (đổi sang cloud sau chỉ cần thay implementation này)
builder.Services.AddScoped<IPhotoStorageService, LocalPhotoStorageService>();

// SignalR cho chat realtime; định danh user bằng claim "sub"
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, SubClaimUserIdProvider>();

// AI: Gemini cho gợi ý mở lời + kiểm duyệt chat (typed HttpClient)
builder.Services.AddHttpClient<IAiAssistantService, GeminiAiService>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Tự áp dụng migration khi khởi động → DB (local hoặc cloud) tự tạo/cập nhật schema, khỏi chạy tay.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

// Bật Swagger ở MỌI môi trường (kể cả Production trên Render) để tiện test/demo API.
// Muốn ẩn ở production thật sau này thì bọc lại trong if (app.Environment.IsDevelopment()).
app.UseSwagger();
app.UseSwaggerUI();

// Trang gốc "/" tự chuyển hướng sang Swagger UI (thay vì trả 404).
app.MapGet("/", () => Results.Redirect("/swagger"));

// Chỉ ép HTTPS ở local dev. Trên production, reverse proxy (Render/Nginx) đã đảm nhận TLS.
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseStaticFiles();      // phục vụ ảnh đã upload trong wwwroot
app.UseCors(WebClientCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");
app.Run();
