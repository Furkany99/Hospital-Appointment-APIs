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
	public class OneTimeMapper:Profile
	{
        public OneTimeMapper()
        {
            CreateMap<OneTime,OneTimeDto>().ForMember(x =>x.Day, a =>a.MapFrom(src => DateOnly.FromDateTime(src.Day)))
             .ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			.ForMember(desc =>desc.OneTimeTimeBlocks, a => a.MapFrom(src => src.OneTimeTimeBlocks)).ForMember(a => a.Id, x => x.MapFrom(src => src.Id));
            CreateMap<OneTimeDto, OneTime>().ForMember(dest => dest.Day, opt => opt.MapFrom(src => new DateTime(src.Day.Year, src.Day.Month, src.Day.Day)))
            .ForMember(x =>x.OneTimeTimeBlocks,a=>a.MapFrom(src => src.OneTimeTimeBlocks)).ForMember(x =>x.DoctorId, a=>a.MapFrom(src => src.DoctorId));
			CreateMap<OneTimeDto, OneTimeRequestModel>();
            CreateMap<OneTimeRequestModel, OneTimeDto>();
            CreateMap<OneTimeTimeBlock, OneTimeTimeBlockDto>().ForMember(desc => desc.StartTime, opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.StartTime)))
            .ForMember(desc => desc.EndTime, a => a.MapFrom(src => TimeOnly.FromTimeSpan(src.EndTime)));
            CreateMap<OneTimeTimeBlockDto, OneTimeTimeBlock>().ForMember(desc => desc.StartTime, opt => opt.MapFrom(src => src.StartTime.ToTimeSpan()))
			.ForMember(desc => desc.EndTime, a => a.MapFrom(src => src.EndTime.ToTimeSpan()));
            CreateMap<OneTimeDto, OneTimeResponseModel>().ForMember(x => x.OneTimeTimeBlocks, a => a.MapFrom(src => src.OneTimeTimeBlocks));
			CreateMap<OneTimeResponseModel,OneTimeDto>().ForMember(x => x.OneTimeTimeBlocks, a => a.MapFrom(src => src.OneTimeTimeBlocks));
			CreateMap<OneTime, OneTimeResponseModel>()
	        .ForMember(dest => dest.Day, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Day)))
	        .ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
	        .ForMember(dest => dest.OneTimeTimeBlocks, opt => opt.MapFrom(src => src.OneTimeTimeBlocks));
            CreateMap<OneTimeResponseModel,OneTime>().ForMember(dest => dest.Day, opt => opt.MapFrom(src => new DateTime(src.Day.Year,src.Day.Month,src.Day.Day)))
			.ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			.ForMember(dest => dest.OneTimeTimeBlocks, opt => opt.MapFrom(src => src.OneTimeTimeBlocks));
			CreateMap<OneTimeDto, OneTimeUpdateRequest>();
			CreateMap<OneTimeUpdateRequest, OneTimeDto>();
		}
    }
}
