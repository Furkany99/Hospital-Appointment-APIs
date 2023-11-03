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

namespace Common.Mapping
{
	public class RoutineAndOneTimeMapper:Profile
	{
        public RoutineAndOneTimeMapper()
        {
			CreateMap<DoctorOneTimeAndRoutineDto, Doctor>()
			.ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.DoctorId));
			CreateMap<Doctor, DoctorOneTimeAndRoutineDto>();
		}
    }
}
