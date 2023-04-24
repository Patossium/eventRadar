using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedEventDto(int Id, int UserId, User User, int EventId, Event Event);
    public record CreateFollowedEventDto(int UserId, User User, int EventId, Event Event);
    public record UpdateFollowedEventDto(int UserId, User User, int EventId, Event Event);
}
