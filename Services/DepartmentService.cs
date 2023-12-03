using AutoMapper;
using Common.Dto;
using Common.Models.RequestModels.Department;
using DataAccess.Contexts;
using DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			Department department = _mapper.Map<Department>(deprtmanetDto);

			_context.Departments.Add(department);
			_context.SaveChanges();
		}

		public void DeleteDepartments(int id) 
		{
			var departments = _context.Departments.Find(id);

			if (departments != null)
			{
				_context.Departments.Remove(departments);
				_context.SaveChanges();

			}
		}

		public DepartmentDto UpdateDepartment(int id, DepartmentUpdateRequestModel departmentUpdate)
		{
			var existingDepartment = _context.Departments.Find(id);

			if (existingDepartment != null && departmentUpdate != null)
			{
				existingDepartment.DepartmentName = departmentUpdate.DepartmentName;


				_context.SaveChanges();

				DepartmentDto departmentDto = _mapper.Map<DepartmentDto>(existingDepartment);
				return departmentDto;

			}

			else
			{
				throw new KeyNotFoundException();
			}


		}

		public List<DepartmentDto> GetDepartments() 
		{
			var departments = _context.Departments.ToList();
			var departmentList = departments.Select(department => _mapper.Map<DepartmentDto>(department)).ToList();
			return departmentList;
		}
    }
}
