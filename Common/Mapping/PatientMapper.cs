using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.RequestModels;
using Common.ResponseModels;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Mapping
{
	public class PatientMapper: Profile
	{
		public PatientMapper()
        {
			CreateMap<PatientDto, Account>();
			CreateMap<PatientDto, Patient>();
			CreateMap<PatientCreateRequestModel, PatientDto>();
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
