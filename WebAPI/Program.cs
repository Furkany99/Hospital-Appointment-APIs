using Common.Mapping;
using DataAccess.Contexts;
using Services;
using Services.Mapping;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<HospitalAppointmentContext>();
builder.Services.AddTransient<PatientService>();
builder.Services.AddTransient<DoctorService>();
builder.Services.AddTransient<DepartmentService>();
builder.Services.AddTransient<TitleService>();
builder.Services.AddAutoMapper(typeof(PatientMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DoctorMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DepartmentMapper).Assembly);
builder.Services.AddAutoMapper(typeof(TitleMapper).Assembly);
builder.Services.AddAutoMapper(typeof (RoutineMapper).Assembly);
builder.Services.AddAutoMapper(typeof(OneTimeMapper).Assembly);
builder.Services.AddAutoMapper(typeof(DateInfoMapper).Assembly);
builder.Services.AddAutoMapper(typeof(AppointmentMapper).Assembly);

var app = builder.Build();

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
