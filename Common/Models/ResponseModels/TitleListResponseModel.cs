using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models.ResponseModels
{
	public class TitleListResponseModel
	{
		public int Count { get; set; }
		public List<TitleResponseModel> titleResponseModels { get; set; } = new List<TitleResponseModel>();
	}
}
