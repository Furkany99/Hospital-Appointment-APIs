using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Title;
using Common.Models.ResponseModels.Title;
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
