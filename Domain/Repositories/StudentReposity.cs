using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Data;
using MyFirstAPI.Models;
using StudentManagement.DTOs;
using StudentManagement.Repositories;

namespace StudentManagement.Domain.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        private readonly DbSet<Student> db_table;
        public StudentRepository(ApplicationDbContext context) : base(context)
        {
            db_table = context.Set<Student>();
        }

        public override async Task<PagedResult<Student>> GetQueryAsync(QueryParams queryParams)
        {
            var query = db_table.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(queryParams.Search))
            {
                var search = queryParams.Search.ToLower();
                query = query.Where(s => s.Name.ToLower().Contains(search) || s.Email.ToLower().Contains(search));
            }

            // Sort
            string sortBy = queryParams.SortBy.ToLower();

            if (sortBy == "name")
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
            }
            else if (sortBy == "gpa")
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Gpa) : query.OrderBy(s => s.Gpa);
            }
            else if (sortBy == "age")
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Age) : query.OrderBy(s => s.Age);
            }
        
            else
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id);
            }

            var total = await query.CountAsync();

            var result = await query.ToPagedResultAsync(queryParams.Page, queryParams.PageSize);

            return result;
        }
    }


}
