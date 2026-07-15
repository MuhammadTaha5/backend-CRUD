using AutoMapper;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentResponseDTO>();
        CreateMap<AddStudentDTO, Student>();
        CreateMap<UpdateStudentDTO, Student>()
    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<RegisterDTO, AppUser>();

    }
}