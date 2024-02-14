using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Department;
using Common.Models.RequestModels.Doctor;
using Common.Models.ResponseModels.Department;
using DataAccess.Entities;
using Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
    public class DepartmentMapper : Profile
	{
        public DepartmentMapper()
        {
            CreateMap<DepartmentDto,Department>();
            CreateMap<Department,DepartmentDto>();
            CreateMap<DoctorDepartmentUpdateRequest, DoctorDto>().ReverseMap();
            CreateMap<DepartmentRequestModel,DepartmentDto>();
			CreateMap<DepartmentDto, DepartmentListResponseModel>();
            CreateMap<DepartmentDto,DepartmentResponseModel>();
		}
    }
}
