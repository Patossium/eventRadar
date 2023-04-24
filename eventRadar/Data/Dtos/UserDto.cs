namespace eventRadar.Data.Dtos
{
    public record UserDto(int Id, string Name, string Email, string Password, string Lastname, string Username, bool Administrator, bool Blocked);
    public record CreateUserDto(string Name, string Email, string Password, string Lastname, string Username, bool Administrator, bool Blocked);
    public record UpdateUserDto(string Name, string Email, string Password, string Lastname, string Username, bool Adminsitrator, bool Blocked);
}
