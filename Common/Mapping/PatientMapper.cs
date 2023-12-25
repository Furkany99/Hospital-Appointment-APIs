using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels.Patient;
using DataAccess.Entities;

namespace Services.Mapping
{
    public class PatientMapper: Profile
	{
		public PatientMapper()
        {
			CreateMap<PatientDto, Account>().ForMember(dest => dest.FirebaseUid, opt => opt.MapFrom(src => src.FirebaseUID));
			CreateMap<PatientDto, Patient>();
			CreateMap<PatientCreateRequestModel, PatientDto>().ForMember(dest => dest.FirebaseUID, opt => opt.MapFrom(src => src.Height));
			CreateMap<Patient, PatientDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email));
			CreateMap<PatientDto,PatientUpdateRequestModel>().ForMember(dest => dest.Name,opt => opt.MapFrom(src => src.Name))
			.ForMember(dest => dest.Surname,opt => opt.MapFrom(src => src.Surname));
			CreateMap<PatientDto, PatientResponseModel>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
			CreateMap<PatientDto, PatientListResponseModel>();
		


		}
    }
}
