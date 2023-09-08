namespace API.Extensions;

public static class DateTimeExtensions
{
    public static int CalculateAge(this DateTime dob)
    {
        var today = DateTime.Now;
        var age = today.Year - dob.Year;
        // xsi este año aún no tiene su cumpleaños
        if (dob > today.AddYears(-age)) age--;

        return age;
    }
}
