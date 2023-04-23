namespace eventRadar.Data.Dtos
{
    public record UserDto(int Id, string Name, string Email, string Password, string Lastname, string Username, bool Administrator);
    public record CreateUserDto(string Name, string Emial, string Password, string Lastname, string Username, bool Administrator);
    public record UpdateUserDto(string Name, string Emial, string Password, string Lastname, string Username, bool Adminsitrator);
}
