using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.DTOs;
using StudentManagement.Repositories;

namespace StudentManagement.Domain.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        protected override string[] SearchableProperties => new[] { "Name", "Email" };
        protected override string[] SortableProperties => new[] { "Name", "Gpa", "Age", "Id" };
        private readonly DbSet<Student> db_table;
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            db_table = context.Set<Student>();
        }
    }


}
