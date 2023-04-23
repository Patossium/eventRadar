namespace eventRadar.Data.Dtos
{
    public record FollowedUserDto(int id, int UserId, int FollowedUserId);
    public record CreateFollowedUserDto(int UserId, int FollowedUserId);
    public record UpdateFollowedUserDto(int UserId, int FollowedUserId);
}
