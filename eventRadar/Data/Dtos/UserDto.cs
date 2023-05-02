namespace eventRadar.Data.Dtos
{
    public record UserDto(string Id, string Username, string Email, string Password, string Name, string Surname, DateTimeOffset? LockoutEnd, bool LockoutEnabled);
    public record ChangePasswordDto (string Password);
}
