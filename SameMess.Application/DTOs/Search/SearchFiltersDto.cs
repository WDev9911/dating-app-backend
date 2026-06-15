using SameMess.Application.DTOs.Settings;

namespace SameMess.Application.DTOs.Search;

public class SearchFiltersDto
{
    public List<InterestDto> Interests { get; set; } = new();
    public List<string> Cities { get; set; } = new();
    public List<string> Genders { get; set; } = new();
    public List<string> Personalities { get; set; } = new();
}
