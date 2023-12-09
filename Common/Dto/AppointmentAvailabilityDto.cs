namespace Common.Dto
{
	public class AppointmentAvailabilityDto
	{
		public int DoctorId { get; set; }
		public List<DateInfoDto> RoutinesAndOneTimes { get; set; }
		public List<AppointmentDto> Appointments { get; set; }
	}
}