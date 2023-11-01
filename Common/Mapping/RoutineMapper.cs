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
			CreateMap<Routine, RoutineDto>().ForMember(x => x.DayOfWeek, opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek)).ForMember(x => x.Id, opt => opt.MapFrom(src => src.Id));
			CreateMap<RoutineDto, Routine>().ForMember(x => x.DayOfWeek,opt => opt.MapFrom(src => (int)src.DayOfWeek))
			.ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			.ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks)); 
			CreateMap<RoutineRequestModel, RoutineDto>()
			.ForMember(dest => dest.TimeBlocks, opt => opt.MapFrom(src => src.TimeBlocks));
			CreateMap<RoutineDto, RoutineRequestModel>();
			CreateMap<TimeBlockRequestModel, TimeBlockDto>();
			CreateMap<TimeBlockDto, TimeBlock>().ForMember(x => x.StartTime, opt => opt.MapFrom(src => src.StartTime.ToTimeSpan()))
			.ForMember(x => x.EndTime, opt => opt.MapFrom(src => src.EndTime.ToTimeSpan()));
			CreateMap<RoutineDto, RoutineUpdateRequestModel>();
			CreateMap<RoutineUpdateRequestModel,RoutineDto>();
			CreateMap<RoutineDto, RoutineListResponseModel>();
			CreateMap<RoutineDto,RoutineResponseModel>();

		}
    }
}
