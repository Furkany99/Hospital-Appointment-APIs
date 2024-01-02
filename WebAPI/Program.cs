using Common.Mapping;
using DataAccess.Contexts;
using Services;
using Services.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

//HospitalAppointmentContext context = new();
//var datas = await context.Patients.ToListAsync();


var config = new ConfigurationBuilder().AddJsonFile("appsettings.json",optional: false ,reloadOnChange : true).Build();
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
	c=>
	{
		c.SwaggerDoc("v1",new OpenApiInfo { Title = "dotnetClaimAuthorization", Version = "v1" });
		c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			In = ParameterLocation.Header,
			Description = "Please instert token",
			Name = "Authorization",
			Type = SecuritySchemeType.Http,
			BearerFormat = "JWT",
			Scheme = "bearer"
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
				new string[] {}
			}
			
			
		});
	});
builder.Services.AddHttpClient();
builder.Services.AddDbContext<HospitalAppointmentContext>();
builder.Services.AddTransient<PatientService>();
builder.Services.AddTransient<DoctorService>();
builder.Services.AddTransient<DepartmentService>();
builder.Services.AddTransient<TitleService>();
builder.Services.AddTransient<AppointmentService>();
builder.Services.AddTransient<AuthService>();
builder.Services.AddAutoMapper(typeof(PatientMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DoctorMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DepartmentMapper).Assembly);
builder.Services.AddAutoMapper(typeof(TitleMapper).Assembly);
builder.Services.AddAutoMapper(typeof (RoutineMapper).Assembly);
builder.Services.AddAutoMapper(typeof(OneTimeMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DateInfoMapper).Assembly);
builder.Services.AddAutoMapper(typeof(AppointmentMapper).Assembly);

FirebaseApp.Create(new AppOptions
{
	Credential = GoogleCredential.FromFile(@"C:\Users\User\Desktop\loginauth-a5aca-firebase-adminsdk-lbu54-d417a6c357.json")
});

// firebase auth
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(opt =>
{

	opt.Authority = "https://securetoken.google.com/loginauth-a5aca";
	opt.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		ValidIssuer = "https://securetoken.google.com/loginauth-a5aca",	
		ValidAudience = "loginauth-a5aca",

	};
});


builder.Services.AddAuthorization(options =>
{
	
	options.AddPolicy("AdminPolicy", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireRole("Admin");
	});

	options.AddPolicy("DoctorPolicy", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireRole("Doctor");
	});

	options.AddPolicy("PatientPolicy", policy =>
	{
		policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
		policy.RequireAuthenticatedUser();
		policy.RequireRole("Patient");
	});
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
	
}


app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
