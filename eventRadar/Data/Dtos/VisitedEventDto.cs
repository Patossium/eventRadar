using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record VisitedEventDto(string Id, string UserId, User User, Event Event, string EventId);
    public record CreateVisitedEventDto();
}
