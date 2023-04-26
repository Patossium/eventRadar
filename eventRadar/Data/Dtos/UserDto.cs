﻿namespace eventRadar.Data.Dtos
{
    public record UserDto(string Id, string Username, string Email, string Password, string Name, string Surname, bool Blocked);
    public record BlockUserDto(bool Blocked);
    public record ChangePasswordDto (string Password);
}
