namespace eventRadar.Data.Dtos
{
    public record FollowedEventDto(int Id, int UserId, int LocationId);
    public record CreateFollowedEventDto(int UserId, int LocationId);
    public record UpdateFollowedEventDto(int UserId, int LocationId);
}
