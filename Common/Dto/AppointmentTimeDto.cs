namespace Common.Dto
{
	public class AppointmentTimeDto
	{
		public bool HasAppointment { get; set; }

		public TimeOnly StartTime { get; set; }

		public TimeOnly EndTime { get; set; }
	}
}