using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedEventDto(string Id, string UserId, User User, Event Event, string EventId);
    public record CreateFollowedEventDto();
}
