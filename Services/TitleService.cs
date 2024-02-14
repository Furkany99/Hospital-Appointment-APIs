using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Title;
using DataAccess.Contexts;
using DataAccess.Entities;
using static Common.Exceptions.ExceptionHandlingMiddleware;

namespace Services
{
    public class TitleService
	{
        private readonly IMapper _mapper;
        private readonly HospitalAppointmentContext _context;

        public TitleService(HospitalAppointmentContext context,IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public void CreateTitle(TitleDto titleDto) 
        {
            Title titles = _mapper.Map<Title>(titleDto);

			if(titles == null ) 
			{
				throw new NotFoundException("Title not found!");
			}

            _context.Titles.Add(titles);
            _context.SaveChanges();
        }

        public TitleDto UpdateTitle(int id,TitleUpdateRequestModel titleUpdate) 
        {
			var existingTitle = _context.Titles.Find(id);

			if (existingTitle == null && titleUpdate == null)
			{
				throw new NotFoundException();

			}

			existingTitle.TitleName = titleUpdate.TitleName;

			_context.SaveChanges();

			TitleDto titleDto = _mapper.Map<TitleDto>(existingTitle);
			return titleDto;


		}

        public void DeleteTitle(int id) 
        {
			var titles = _context.Titles.Find(id);

			if (titles == null)
			{
				throw new NotFoundException("Title not found");
			}

			_context.Titles.Remove(titles);
			_context.SaveChanges();
		}

        public List<TitleDto> GetTitles() 
        {
			var titles = _context.Titles.ToList();
			var titlesList = titles.Select(title => _mapper.Map<TitleDto>(title)).ToList();
			return titlesList;
		}

		public TitleDto GetTitleById(int id)
		{
			var titleByID = _context.Titles.FirstOrDefault(x => x.Id == id);

			if (titleByID == null)
			{
				throw new NotFoundException("Title not found!");
			}

			var titleDto = _mapper.Map<TitleDto>(titleByID);

			return titleDto;
		}
	}
}
