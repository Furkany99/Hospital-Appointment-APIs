namespace Common.Dto
{
	public class RoutineDto
	{
		public int Id { get; set; }

		public int DoctorId { get; set; }

		public DayOfWeek DayOfWeek { get; set; }

		public bool IsOnLeave { get; set; }

		public List<TimeBlockDto>? TimeBlocks { get; set; }
	}
}