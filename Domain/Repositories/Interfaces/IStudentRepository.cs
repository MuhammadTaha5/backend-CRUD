using MyFirstAPI.Models;
using StudentManagement.DTOs;
using StudentManagement.Repositories;

namespace StudentManagement.Domain.Repositories
{
    public interface IStudentRepository:IRepository<Student>
    {
        
    }
}