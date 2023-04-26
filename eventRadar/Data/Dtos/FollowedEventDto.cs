using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedEventDto(int Id, string UserId, User User, Event Event, int EventId);
    public record CreateFollowedEventDto();
}
