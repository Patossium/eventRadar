﻿namespace eventRadar.Data.Dtos
{
    public record UserDto(int Id, string Username, string Email, string Password, string Name, string Surname, bool Blocked);
}
