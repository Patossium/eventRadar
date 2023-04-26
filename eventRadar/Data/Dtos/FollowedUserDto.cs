using eventRadar.Auth.Model;

namespace eventRadar.Data.Dtos
{
    public record FollowedUserDto(int id, int UserId, User User, User Followed_User, int Followed_UserId);
    public record CreateFollowedUserDto();
}
