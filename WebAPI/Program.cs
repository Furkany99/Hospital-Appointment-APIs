using Common.Mapping;
using DataAccess.Contexts;
using Services;
using Services.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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
