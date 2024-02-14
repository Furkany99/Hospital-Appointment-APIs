using AutoMapper;
using Common.Dto;
using Common.Models;
using Common.Models.RequestModels.Patient;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Common.Exceptions.ExceptionHandlingMiddleware;

namespace Services
{
	public class PatientService
	{
		private readonly HospitalAppointmentContext _context;
		private readonly IMapper _mapper;

		public PatientService(HospitalAppointmentContext Context, IMapper mapper)
		{
			_context = Context;
			_mapper = mapper;
		}

		public void CreatePatient(PatientDto patientDto, string firebaseUserId)
		{
			Patient patient = _mapper.Map<Patient>(patientDto);
			Account account = _mapper.Map<Account>(patientDto);

			account.FirebaseUid = firebaseUserId;

			patient.Account = account;

			_context.Patients.Add(patient);
			_context.Accounts.Add(account);

			_context.SaveChanges();
		}

		public PatientDto Update(int id, PatientUpdateRequestModel patientUpdate)
		{
			var existingPatient = _context.Patients.Find(id);

			if (existingPatient == null && patientUpdate == null)
			{
				throw new NotFoundException("Patient not found or update data missing.");
			}

			existingPatient.Name = patientUpdate.Name;
			existingPatient.Surname = patientUpdate.Surname;

			_context.SaveChanges();

			PatientDto updatedPatientDto = _mapper.Map<PatientDto>(existingPatient);
			return updatedPatientDto;
		}

		public void Delete(int id)
		{
			var existingPatient = _context.Patients.Find(id);

			if (existingPatient == null)
			{
				throw new NotFoundException("Patient not found!");
			}
			_context.Patients.Remove(existingPatient);
			_context.SaveChanges();
		}

		public List<PatientDto> GetPatients()
		{
			var patients = _context.Patients.Include(p => p.Account).ToList();
			var patientsList = patients.Select(patient => _mapper.Map<PatientDto>(patient)).ToList();
			return patientsList;
		}

		public PatientDto GetPatientById(int id)
		{
			var patientById = _context.Patients
			.Include(x => x.Account)
			.FirstOrDefault(x => x.Id == id);
			if (patientById == null)
			{
				throw new NotFoundException("Patient not found!");
			}

			var patientDto = _mapper.Map<PatientDto>(patientById);

			return patientDto;
		}

		

	}
}