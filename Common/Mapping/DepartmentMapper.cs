using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
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
            CreateMap<DepartmentRequestModel,DepartmentDto>();
			CreateMap<DepartmentDto, DepartmentListResponseModel>();
            CreateMap<DepartmentDto,DepartmentResponseModel>();
		}
    }
}
