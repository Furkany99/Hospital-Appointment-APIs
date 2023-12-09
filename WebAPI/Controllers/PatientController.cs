using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.RequestModels.Appointment;
using Common.Models.ResponseModels.Appointment;
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

		

		
	}
}
