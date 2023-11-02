using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
	public class RoutineMapper:Profile
	{
		public RoutineMapper()
        {
			CreateMap<Routine, RoutineDto>().ForMember(x => x.DayOfWeek, opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek))
			.ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id)).ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks));
			CreateMap<RoutineDto, Routine>().ForMember(x => x.DoctorId, opt => opt.MapFrom(src => src.DoctorId))
			.ForMember(x => x.DayOfWeek,opt => opt.MapFrom(src => (int)src.DayOfWeek))
			.ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			.ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks)); 
			CreateMap<RoutineRequestModel, RoutineDto>()
			.ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks));
			CreateMap<RoutineDto, RoutineRequestModel>();
			//Burada kullanıcı istemciden request yaptığı için dönüşüm yapmamız gerekiyor. Fakat yapılamıyor.
			CreateMap<TimeBlock, TimeBlockDto>().ForMember(x => x.StartTime, opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.StartTime)))
			.ForMember(x => x.EndTime, opt => opt.MapFrom(src =>TimeOnly.FromTimeSpan(src.EndTime)));
			CreateMap<TimeBlockRequestModel, TimeBlockDto>();
			//Burada kullanıcıdan bir değer aldığımız için ToTimeSpan dönüşümü yaptık
			CreateMap<TimeBlockDto, TimeBlock>().ForMember(x => x.StartTime, opt => opt.MapFrom(src => src.StartTime.ToTimeSpan()))
			.ForMember(x => x.EndTime, opt => opt.MapFrom(src => src.EndTime.ToTimeSpan()));
			CreateMap<RoutineDto, RoutineUpdateRequestModel>();
			CreateMap<RoutineUpdateRequestModel,RoutineDto>();
			CreateMap<RoutineDto, RoutineListResponseModel>();
			CreateMap<RoutineListResponseModel, RoutineDto>();
			CreateMap<RoutineDto,RoutineResponseModel>().ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek))
			.ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			.ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks));

		}
    }
}
