using AutoMapper;
using SameMess.Application.DTOs.Auth;
using SameMess.Domain.Entities;

namespace SameMess.Application.Mappings;

public class AuthMappingProfile : Profile
{
    public AuthMappingProfile()
    {
        CreateMap<User, UserInfoDto>()
            .ForMember(dest => dest.DisplayName,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.DisplayName : string.Empty))
            .ForMember(dest => dest.AvatarUrl,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarUrl : null))
            .ForMember(dest => dest.AvatarFrame,
                opt => opt.MapFrom(src => src.Profile != null ? src.Profile.AvatarFrame : null));
    }
}
