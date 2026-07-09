using AutoMapper;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentResponseDTO>();
        CreateMap<AddStudentDTO, Student>();
        CreateMap<UpdateStudentDTO, Student>();
        CreateMap<RegisterDTO, AppUser>();

    }
}