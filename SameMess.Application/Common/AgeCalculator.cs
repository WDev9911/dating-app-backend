namespace SameMess.Application.Common;

public static class AgeCalculator
{
    /// <summary>Tính tuổi (năm tròn) từ ngày sinh; trả null nếu không có ngày sinh.</summary>
    public static int? FromDateOfBirth(DateOnly? dateOfBirth)
    {
        if (dateOfBirth is not { } dob) return null;

        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var age = today.Year - dob.Year;
        if (dob > today.AddYears(-age)) age--;
        return age;
    }
}
