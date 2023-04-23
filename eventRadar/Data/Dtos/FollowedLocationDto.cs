namespace eventRadar.Data.Dtos
{
    public record FollowedLocationDto(int Id, int UserId, int LocationId);
    public record CreateFollowedLocationDto(int UserId, int LocationId);
    public record UpdateFollowedLocationDto(int UserId, int LocationId);
}
