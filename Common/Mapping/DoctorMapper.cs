using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
using Common.RequestModels;
using Common.ResponseModels;
using DataAccess.Entities;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
	public class DoctorMapper:Profile
	{
		public DoctorMapper()
		{
			CreateMap<DoctorDto,Doctor>().ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
			CreateMap<DoctorDto,Account>();
			CreateMap<DoctorRequestModel, DoctorDto>().ForMember(dest => dest.DepartmentIds, opt => opt.MapFrom(src => src.DepartmentId));
			CreateMap<Doctor, DoctorDto>()
			.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Account.Email))
			.ForMember(dest => dest.DepartmentIds, opt => opt.MapFrom(src => src.Departments.Select(dd => dd.Id).ToList()));
			CreateMap<DoctorDto, DoctorUpdateRequestModel>();
			CreateMap<DoctorDto, DoctorResponseModel>();
			CreateMap<DoctorDepartmentUpdateRequest, DoctorDto>();
			CreateMap<DoctorTitleUpdateRequest, DoctorDto>();




		}
	}
}


