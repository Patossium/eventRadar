namespace eventRadar.Data.Dtos
{
    public record ChangedEventDto(string id, string OldInformation, string NewInformation, DateTime ChangeTime);
    public record CreateChangedEventDto(string OldInformation, string NewInformation, DateTime ChangeTime);
    public record UpdateChangedEventDto(string OldInformation, string NewInformation, DateTime ChangeTime);

}
