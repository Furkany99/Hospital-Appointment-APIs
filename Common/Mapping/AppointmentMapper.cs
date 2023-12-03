using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.RequestModels.Appointment;
using Common.Models.ResponseModels;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
    public class AppointmentMapper:Profile
	{
        public AppointmentMapper()
        {
            CreateMap<Appointment, AppointmentDto>().ForMember(dest => dest.Date, opt => opt.MapFrom(src => DateOnly.FromDateTime(src.Date)));
			CreateMap<AppointmentDto, Appointment>().ForMember(dest => dest.Date, opt => opt.MapFrom(src => new DateTime(src.Date.Year, src.Date.Month, src.Date.Day)));
			CreateMap<AppointmentDto,AppointmentRequestModel>();
			CreateMap<AppointmentRequestModel, AppointmentDto>()
			.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
			.ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Detail))
			.ForMember(dest => dest.DocId, opt => opt.MapFrom(src => src.DocId))
			.ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
			.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
			.ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId));
			CreateMap<AppointmentDto, Status>().ForMember(x => x.Id, s=>s.MapFrom(src => src.StatusId));
			CreateMap<AppointmentDto,Patient>().ForMember(x => x.Id, s=> s.MapFrom(src => src.PatientId));
			CreateMap<AppointmentTime, AppointmentTimeDto>()
			.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => TimeOnly.FromTimeSpan( src.StartTime)))
			.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => TimeOnly.FromTimeSpan(src.EndTime)));
			CreateMap<AppointmentTimeDto, AppointmentTime>()
				.ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToTimeSpan()))
				.ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToTimeSpan()));
			CreateMap<AppointmentDto, AppointmentResponseModel>();
			CreateMap<AppointmentDto, AppointmentListResponseModel>();
			CreateMap<AppointmentDto, AppointmentUpdateRequestModel>();
			CreateMap<AppointmentUpdateRequestModel, AppointmentDto>();
			CreateMap<AppointmentUpdateRequestModel, Appointment>().ForMember(a => a.Date, opt => opt.MapFrom(src => new DateTime(src.Date.Year, src.Date.Month, src.Date.Day)))
				.ForMember(dest => dest.AppointmentTimes, opt => opt.MapFrom(src =>
		src.appointmentTimes.Select(appointmentTime => new AppointmentTime
		{
			StartTime = appointmentTime.StartTime.ToTimeSpan(),
			EndTime = appointmentTime.EndTime.ToTimeSpan()
		})));
			//CreateMap<AppointmentUpdateRequestModel, Appointment>();
			CreateMap<AppointmentTimeDto,AppointmentTime>().ForMember(a => a.StartTime, opt => opt.MapFrom(src => src.StartTime.ToTimeSpan()))
				.ForMember(x => x.EndTime, opt => opt.MapFrom(src => src.EndTime.ToTimeSpan()));




		}
    }
}
