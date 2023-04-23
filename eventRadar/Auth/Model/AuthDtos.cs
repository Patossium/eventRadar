﻿using System.ComponentModel.DataAnnotations;

namespace eventRadar.Auth.Model
{
    public class AuthDtos
    {
        public record RegisterUserDto([Required] string Username, [EmailAddress][Required] string Email, [Required] string Password, [Required] string Name, [Required] string Lastname);
        public record LoginDto(string Username, string Password);
        public record UserDto(string Id, string  Username, string Email, string Name, string Lastname);
        public record SuccessfullLoginDto(string AccessToken);
    }
}
