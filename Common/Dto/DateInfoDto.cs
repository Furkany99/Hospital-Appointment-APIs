namespace Common.Dto
{
	public class DateInfoDto
	{
		public int isOneTime { get; set; }

		public DayOfWeek DayOfWeek { get; set; }

		public bool IsOnLeave { get; set; }

		public DateOnly Day { get; set; }

		public List<DateInfoTimeDto>? dateInfoTimeDtos { get; set; }
	}
}