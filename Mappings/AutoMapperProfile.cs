using AutoMapper;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTOs;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Student, StudentResponseDTO>();
        CreateMap<AddStudentDTO, Student>();
        // Partial update mapping: only overwrites destination properties whose source
        // value is non-null. This lets UpdateStudentDTO be used for partial updates —
        // fields the client omits (left null) won't overwrite existing values on the entity.
        CreateMap<UpdateStudentDTO, Student>()
    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<RegisterDTO, AppUser>();

    }
}