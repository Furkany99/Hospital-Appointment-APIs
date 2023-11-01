using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels;
using Common.Models.ResponseModels;
using Common.ResponseModels;
using Microsoft.AspNetCore.Mvc;
using Services;

namespace WebAPI.Controllers
{
	[Route("admin/[controller]")]
	[ApiController]
	public class TitleController : ControllerBase
	{
		private readonly TitleService _titleService;
		private readonly IMapper _mapper;

		public TitleController(TitleService titleService,IMapper mapper)
        {
			_titleService = titleService;
			_mapper = mapper;
		}

		[HttpPost()]
		public IActionResult CreateTitle(TitleRequestModel titleRequest) 
		{
			try
			{
				var titles = _mapper.Map<TitleDto>(titleRequest);
				_titleService.CreateTitle(titles);
				return Ok();
			}
			catch
			{
				return BadRequest();
			}
		}

		[HttpGet()]
		public List<TitleListResponseModel> GetTitles()
		{
			var titles = _titleService.GetTitles();
			var titleResponseList = titles.Select(titleDto => _mapper.Map<TitleResponseModel>(titleDto)).ToList();

			var titleListResponse = new TitleListResponseModel
			{
				Count = titleResponseList.Count,
				titleResponseModels = titleResponseList
				
			};

			return new List<TitleListResponseModel> { titleListResponse };


		}

		[HttpGet("{id}")]
		public TitleResponseModel GetTitlesByID(int id)
		{
			var titleDto = _titleService.GetTitleById(id);
			var titleResponseModel = _mapper.Map<TitleResponseModel>(titleDto);
			return titleResponseModel;
		}

		[HttpPut()]
		public TitleUpdateRequestModel UpdateTitle(int id, TitleUpdateRequestModel titleUpdate)
		{
			var updatedTitle = _titleService.UpdateTitle(id, titleUpdate);
			var TitleRequestModel = _mapper.Map<TitleUpdateRequestModel>(updatedTitle);
			if (updatedTitle != null)
			{
				return TitleRequestModel;
			}
			else
			{
				return null;
			}

		}

		[HttpDelete()]
		public IActionResult DeleteTitle(int id)
		{
			_titleService.DeleteTitle(id);
			return Ok();

		}
	}
}
