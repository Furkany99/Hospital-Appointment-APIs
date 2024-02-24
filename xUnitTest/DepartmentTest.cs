using AutoMapper;
using Common.Dto;
using Common.Mapping;
using Common.Models.RequestModels.Department;
using DataAccess.Contexts;
using DataAccess.Entities;
using Moq;
using Services;

namespace xUnitTest
{
	public class DepartmentTest
	{
		private static IMapper _mapper;

		public DepartmentTest()
        {
			if (_mapper == null)
			{
				var mappingConfig = new MapperConfiguration(mc =>
				{
					mc.AddProfile(new DepartmentMapper());
				});
				IMapper mapper = mappingConfig.CreateMapper();
				_mapper = mapper;
			}

		}

        [Fact]
		public void CreateDepartments_ShouldAddDepartmentToContext()
		{
			// Arrange
			DepartmentDto departmentDto = new()
			{
				Id = 55,
				DepartmentName = "Test",

			};

			if (_mapper == null)
			{
				throw new ArgumentNullException(nameof(_mapper));
			}

			var contextMock = new Mock<HospitalAppointmentContext>();
			var departmentService = new DepartmentService(contextMock.Object, _mapper);
			var mockDbSet = MockDatabaseHelper.CreateMockDbSet(new List<Department>());
			contextMock.Setup(c => c.Departments).Returns(mockDbSet);

			// Act
			departmentService.CreateDepartments(departmentDto);

			// Assert
			contextMock.Verify(c => c.Departments.Add(It.IsAny<Department>()), Times.Once);
			contextMock.Verify(c => c.SaveChanges(), Times.Once);
		}

		[Fact]
		public void DeleteDepartments_ShouldRemoveDepartmentFromContext()
		{
			// Arrange
			var idToDelete = 1;
			var contextMock = new Mock<HospitalAppointmentContext>();
			var departmentService = new DepartmentService(contextMock.Object, _mapper);
			contextMock.Setup(c => c.Departments.Find(idToDelete)).Returns(new Department { Id = idToDelete });

			// Act
			departmentService.DeleteDepartments(idToDelete);

			// Assert
			contextMock.Verify(c => c.Departments.Find(idToDelete), Times.Once);
			contextMock.Verify(c => c.Departments.Remove(It.IsAny<Department>()), Times.Once);
			contextMock.Verify(c => c.SaveChanges(), Times.Once);
		}

		[Fact]
		public void UpdateDepartment_ShouldUpdateDepartmentAndReturnDto()
		{
			// Arrange
			int departmentId = 1;
			var existingDepartment = new Department
			{
				Id = departmentId,
				DepartmentName = "ExistingDepartment"
			};

			var departmentUpdate = new DepartmentUpdateRequestModel
			{
				DepartmentName = "UpdatedDepartment"
			};

			var contextMock = new Mock<HospitalAppointmentContext>();
			contextMock.Setup(c => c.Departments.Find(departmentId)).Returns(existingDepartment);
			var departmentService = new DepartmentService(contextMock.Object, _mapper);

			// Act
			var updatedDepartmentDto = departmentService.UpdateDepartment(departmentId, departmentUpdate);

			// Assert
			contextMock.Verify(c => c.SaveChanges(), Times.Once);
			Assert.Equal(departmentId, updatedDepartmentDto.Id);
			Assert.Equal(departmentUpdate.DepartmentName, updatedDepartmentDto.DepartmentName);
		}

		[Fact]
		public void GetDepartments_ShouldReturnListOfDepartmentDto()
		{
			// Arrange
			var departments = new List<Department>
		{
			new Department { Id = 1, DepartmentName = "Department1" },
			new Department { Id = 2, DepartmentName = "Department2" }
		};

			var departmentDtos = new List<DepartmentDto>
		{
			new DepartmentDto { Id = 1, DepartmentName = "Department1" },
			new DepartmentDto { Id = 2, DepartmentName = "Department2" }
		};

			var _contextMock = new Mock<HospitalAppointmentContext>();
			_contextMock.Setup(c => c.Departments).Returns(MockDatabaseHelper.CreateMockDbSet(departments));
			var departmentService = new DepartmentService(_contextMock.Object, _mapper);

			// Act
			var result = departmentService.GetDepartments();

			// Assert
			Assert.NotNull(result);
			Assert.Equal(departmentDtos.Count, result.Count);

			for (int i = 0; i < departmentDtos.Count; i++)
			{
				Assert.Equal(departmentDtos[i].Id, result[i].Id);
				Assert.Equal(departmentDtos[i].DepartmentName, result[i].DepartmentName);
			}
		}
	}
}