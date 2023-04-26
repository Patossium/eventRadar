using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record VisitedEventDto(int Id, string userId, User User, Event Event, int EventId);
    public record CreateVisitedEventDto();
}
