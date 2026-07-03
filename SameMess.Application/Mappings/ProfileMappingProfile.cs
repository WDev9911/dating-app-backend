using AutoMapper;
using SameMess.Application.Common;
using SameMess.Application.DTOs.Preference;
using SameMess.Application.DTOs.Profile;
using SameMess.Domain.Entities;
using SameMess.Domain.Enums;

namespace SameMess.Application.Mappings;

public class ProfileMappingProfile : Profile
{
    public ProfileMappingProfile()
    {
        CreateMap<Photo, PhotoDto>();
        CreateMap<UserPreference, PreferenceDto>();

        // ProfileDto được gom từ User (Profile + Photos + Preference)
        CreateMap<User, ProfileDto>()
            .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
            .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Profile!.DisplayName))
            .ForMember(d => d.Gender, o => o.MapFrom(s => s.Profile!.Gender))
            .ForMember(d => d.DateOfBirth, o => o.MapFrom(s => s.Profile!.DateOfBirth))
            .ForMember(d => d.Age, o => o.MapFrom(s => AgeCalculator.FromDateOfBirth(s.Profile!.DateOfBirth)))
            .ForMember(d => d.Bio, o => o.MapFrom(s => s.Profile!.Bio))
            .ForMember(d => d.Height, o => o.MapFrom(s => s.Profile!.Height))
            .ForMember(d => d.Location, o => o.MapFrom(s => s.Profile!.Location))
            .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Profile!.Latitude))
            .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Profile!.Longitude))
            .ForMember(d => d.DatingGoal, o => o.MapFrom(s => s.Profile!.DatingGoal))
            .ForMember(d => d.AvatarUrl, o => o.MapFrom(s => s.Profile!.AvatarUrl))
            .ForMember(d => d.AvatarFrame, o => o.MapFrom(s => s.Profile!.AvatarFrame))
            .ForMember(d => d.IsAdmin, o => o.MapFrom(s => s.Role == UserRole.Admin))
            .ForMember(d => d.IsProfileCompleted, o => o.MapFrom(s => s.Profile!.IsProfileCompleted))
            .ForMember(d => d.IsPhotoVerified, o => o.MapFrom(s => s.Profile!.IsPhotoVerified))
            .ForMember(d => d.VerificationStatus, o => o.MapFrom(s => s.Profile!.VerificationStatus))
            .ForMember(d => d.Photos, o => o.MapFrom(s => s.Photos))
            .ForMember(d => d.Preference, o => o.MapFrom(s => s.Preference));
    }
}
