using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
	public class AppointmentMapper:Profile
	{
        public AppointmentMapper()
        {
            CreateMap<Appointment, AppointmentDto>().ForMember(x => x.Duration, a => a.MapFrom(src => TimeOnly.FromTimeSpan(src.Duration)));
            CreateMap<AppointmentDto, Appointment>().ForMember(x => x.Duration, a=> a.MapFrom(src => src.Duration.ToTimeSpan()));
			CreateMap<AppointmentDto,AppointmentRequestModel>().ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration));
			CreateMap<AppointmentRequestModel, AppointmentDto>()
			.ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.Duration))
			.ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Detail))
			.ForMember(dest => dest.DocId, opt => opt.MapFrom(src => src.DocId))
			.ForMember(dest => dest.DepartmentId, opt => opt.MapFrom(src => src.DepartmentId))
			.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.Date))
			.ForMember(dest => dest.PatientId, opt => opt.MapFrom(src => src.PatientId));
			CreateMap<AppointmentDto, Status>().ForMember(x => x.Id, s=>s.MapFrom(src => src.StatusId));
			CreateMap<AppointmentDto,Patient>().ForMember(x => x.Id, s=> s.MapFrom(src => src.PatientId));


		}
    }
}
