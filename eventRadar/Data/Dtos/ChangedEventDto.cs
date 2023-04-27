using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record ChangedEventDto(int Id, string OldInformation, string NewInformation, DateTime ChangeTime, Event Event);
    public record CreateChangedEventDto(string OldInformation, string NewInformation, DateTime ChangeTime, Event Event);

}
