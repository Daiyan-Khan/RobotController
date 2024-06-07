using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using robot_controller_api.Persistence;
using robot_controller_api.Authentication;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllers();
// 4.1P:
// builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandADO>();
// builder.Services.AddScoped<IMapDataAccess, MapADO>();

//Add Authentication
builder.Services.AddAuthentication("BasicAuthentication") .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler> ("BasicAuthentication", default);
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    policy.RequireClaim(ClaimTypes.Role, "Admin"));
    options.AddPolicy("UserOnly", policy =>
    policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));
});

//4.1P
builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandAD0>();
builder.Services.AddScoped<IMapDataAccess, MapAD0>();

//4.2C
// builder.Services.AddScoped<IRobotCommandDataAccess,RobotCommandRepository>();
// builder.Services.AddScoped<IMapDataAccess, MapRepository>();

// 4.3D:
//builder.Services.AddDbContext<RobotContext>(options => options.UseNpgsql("Host=localhost;Database=postgres;Username=postgres;Password=1234"));
//builder.Services.AddScoped<IRobotCommandDataAccess, RobotCommandEF>();
//builder.Services.AddScoped<IMapDataAccess, MapEF>();

//6.1D
// builder.Services.AddScoped<IUserDataAccess, UserRepository>();
builder.Services.AddScoped<IUserModelDataAccess, UserADO>();

//5.1P
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseSwagger();
app.UseSwaggerUI(setup => setup.InjectStylesheet("/wwwroot/styles/theme-flattop. css"));
app.MapControllers();

app.Run();
