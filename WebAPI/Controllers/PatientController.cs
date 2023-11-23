using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.RequestModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class PatientController : ControllerBase
	{
		private readonly IMapper _mapper;
		private readonly PatientService _patientService;

		public PatientController(PatientService patientService, IMapper mapper)
        {
			_patientService = patientService;
			_mapper = mapper;
		}

		[HttpPost("Appointments")]
		public IActionResult Appointment(AppointmentRequestModel requestModel)
		{
			try
			{
				var appointments = _mapper.Map<AppointmentDto>(requestModel);
				_patientService.CreateAppointment(appointments);
				return Ok();
			}
			catch
			{
				return BadRequest();
			}

		}
	}
}
