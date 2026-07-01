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
            else if (sortBy == "email")
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Email) : query.OrderBy(s => s.Email);
            }
            else
            {
                query = queryParams.Desc ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id);
            }

            var total = await query.CountAsync();

            var items = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();
            

            return new PagedResult<Student>
            {
                Items = items,
                TotalCount = total,
                PageNumber = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }
    }


}
