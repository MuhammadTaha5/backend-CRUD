using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.DTOs;
using StudentManagement.Repositories;

namespace StudentManagement.Domain.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        protected override string[] FilterableProperties => new[] { "Name", "Email", "Age", "Gpa"};
        protected override string[] SortableProperties => new[] { "Name", "Gpa", "Age", "Id" };
        public StudentRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }


}
