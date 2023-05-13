using System.ComponentModel.DataAnnotations;

namespace eventRadar.Auth.Model
{
    public class AuthDtos
    {
        public record RegisterUserDto([Required] string Username, [EmailAddress][Required] string Email, [Required] string Password, [Required] string Name, [Required] string Surname);
        public record LoginDto(string Username, string Password);
        public record NewUserDto(string Id, string  Username, string Email, string Name, string Surname);
        public record SuccessfullLoginDto(string AccessToken);
    }
}
