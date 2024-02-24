using AutoMapper;
using Common.Mapping;
using Common.Models.RequestModels.Doctor;
using DataAccess.Contexts;
using DataAccess.Entities;
using Moq;
using Services;


namespace xUnitTest
{
	public class DoctorTest
	{
		private static IMapper _mapper;

		public DoctorTest()
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
		public void DoctorService_UpdateDoctor()
		{
			// Arrange
			var contextMock = new Mock<HospitalAppointmentContext>();
			var doctorService = new DoctorService(contextMock.Object, _mapper);

			
			var existingDoctor = new Doctor { Id = 1, Name = "Eski Ad", Surname = "Eski Soyad" };
			var doctorUpdate = new DoctorUpdateRequestModel { Name = "Yeni Ad", Surname = "Yeni Soyad" };

			
			contextMock.Setup(c => c.Doctors.Find(It.IsAny<int>())).Returns(existingDoctor);

			// Act
			var updatedDoctorDto = doctorService.UpdateDoctor(1, doctorUpdate);

			// Assert
			Assert.Equal("Yeni Ad", updatedDoctorDto.Name);
			Assert.Equal("Yeni Soyad", updatedDoctorDto.Surname);
			contextMock.Verify(c => c.SaveChanges(), Times.Once);
		}
	}
}
