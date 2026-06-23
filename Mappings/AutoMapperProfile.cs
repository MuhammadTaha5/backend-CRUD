using AutoMapper;
using MyFirstAPI.Models;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentResponseDTO>();
        CreateMap<AddStudentDTO, Student>();
        CreateMap<UpdateStudentDTO, Student>();

    }
}