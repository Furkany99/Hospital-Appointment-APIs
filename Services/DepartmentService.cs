using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Department;
using DataAccess.Contexts;
using DataAccess.Entities;
using static Common.Exceptions.ExceptionHandlingMiddleware;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Services
{
    public class DepartmentService
	{
		private readonly HospitalAppointmentContext _context;
		private readonly IMapper _mapper;

		public DepartmentService(HospitalAppointmentContext Context,IMapper mapper)
        {
			_context = Context;
			_mapper = mapper;
		}

		public void CreateDepartments(DepartmentDto deprtmanetDto) 
		{
			if (_mapper == null)
			{
				throw new InvalidOperationException("_mapper is not initialized.");
			}

			Department department = _mapper.Map<Department>(deprtmanetDto);

			_context.Departments.Add(department);
			_context.SaveChanges();
		}

		public void DeleteDepartments(int id) 
		{
			var departments = _context.Departments.Find(id);

			if (departments == null)
			{
				throw new NotFoundException("Department not found!");

			}
			_context.Departments.Remove(departments);
			_context.SaveChanges();
		}

		public DepartmentDto UpdateDepartment(int id, DepartmentUpdateRequestModel departmentUpdate)
		{
			var existingDepartment = _context.Departments.Find(id);

			if (existingDepartment == null && departmentUpdate == null)
			{
				throw new NotFoundException("Department not found!");

			}

			existingDepartment.DepartmentName = departmentUpdate.DepartmentName;


			_context.SaveChanges();

			DepartmentDto departmentDto = _mapper.Map<DepartmentDto>(existingDepartment);
			return departmentDto;
		}

		public List<DepartmentDto> GetDepartments() 
		{
			var departments = _context.Departments.ToList();
			var departmentList = departments.Select(department => _mapper.Map<DepartmentDto>(department)).ToList();
			return departmentList;
		}
    }
}
