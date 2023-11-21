using AutoMapper;
using Common.Dto;
using DataAccess.Entities;

namespace Common.Mapping
{
	public class DateInfoMapper : Profile
	{
		public DateInfoMapper()
		{

			CreateMap<OneTime, DateInfoDto>()
				.ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.Day.DayOfWeek))
				.ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
				.ForMember(dest => dest.Day, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Day)))
				.ForMember(dest => dest.dateInfoTimeDtos, opt => opt.MapFrom(src => src.OneTimeTimeBlocks));

			CreateMap<Routine, DateInfoDto>()
			   .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => (DayOfWeek)src.DayOfWeek))
			   .ForMember(dest => dest.IsOnLeave, opt => opt.MapFrom(src => src.IsOnLeave))
			   .ForMember(dest => dest.Day, opt => opt.MapFrom(src => src.DayOfWeek))
			   .ForMember(dest => dest.dateInfoTimeDtos, opt => opt.MapFrom(src => src.TimeBlocks));

		}
	}
}