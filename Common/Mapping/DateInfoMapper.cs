using AutoMapper;
using Common.Dto;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Mapping
{
	public class DateInfoMapper:Profile
	{
        public DateInfoMapper()
        {

			CreateMap<Doctor, DateInfoDto>();
			CreateMap<DateInfoDto, Doctor>();
		}
    }
}
