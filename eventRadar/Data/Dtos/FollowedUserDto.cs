using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedUserDto(int id, int UserId, User User, int FollowedUserId, User Followed_User);
    public record CreateFollowedUserDto(int UserId, User User, int FollowedUserId, User Followed_User);
    public record UpdateFollowedUserDto(int UserId, User User, int FollowedUserId, User Followed_User);
}
