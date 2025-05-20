using AutoMapper;
using TheInBazar.Domain.Entities;
using TheInBazar.Service.DTOs;

namespace TheInBazar.Service.Mappers;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User,UserForCreationDto>().ReverseMap();
        CreateMap<User,UserForResultDto>().ReverseMap();
        CreateMap<User, UserForUpdateDto>().ReverseMap();
    }
}
