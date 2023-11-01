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
	public class TitleMapper:Profile
	{
        public TitleMapper()
        {
            CreateMap<TitleDto, Title>();
            CreateMap<Title, TitleDto>();
            CreateMap<TitleRequestModel, TitleDto>();
            CreateMap<TitleDto,TitleResponseModel>();
        }
    }
}
