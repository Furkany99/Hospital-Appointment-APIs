using AutoMapper;
using Common.Dto;
using Common.Mapping;
using DataAccess.Contexts;
using DataAccess.Entities;
using Moq;
using Services;

namespace xUnitTest
{
	public class AppointmentTest
	{

		private static IMapper _mapper;

		public AppointmentTest()
        {
			if (_mapper == null)
			{
				var mappingConfig = new MapperConfiguration(mc =>
				{
					mc.AddProfile(new DoctorMapper());
				});
				IMapper mapper = mappingConfig.CreateMapper();
				_mapper = mapper;
			}

		}

		[Fact]
		public void HasDoctorAppointments_WithAppointments_ReturnsTrue()
		{
			// Arrange
			int doctorId = 1;
			DateOnly startDate = new DateOnly(2024, 2, 24);
			DateOnly endDate = new DateOnly(2024, 2, 28);
			DateTime testDateTime = startDate.ToDateTime(TimeOnly.Parse("10:00 PM"));

			var appointments = new List<Appointment>
			{
				new Appointment { DocId = doctorId, Date = testDateTime}
				
			};

			var mockContext = new Mock<HospitalAppointmentContext>();
			var mockDoctorService = new Mock<DoctorService>(mockContext.Object, _mapper);

			var mockDbSet = MockDatabaseHelper.CreateMockDbSet(appointments);
			mockContext.Setup(c => c.Appointments).Returns(mockDbSet);

			var appointmentService = new AppointmentService(mockContext.Object, _mapper, mockDoctorService.Object);

			// Act
			bool result = appointmentService.HasDoctorAppointments(doctorId, startDate, endDate);

			// Assert
			Assert.True(result);
		}

		




	}
}