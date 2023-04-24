﻿namespace eventRadar.Data.Dtos
{
    public record ChangedEventDto(int id, string OldInformation, string NewInformation, DateTime ChangeTime);
    public record CreateChangedEventDto(string OldInformation, string NewInformation, DateTime ChangeTime);
    public record UpdateChangedEventDto(string OldInformation, string NewInformation, DateTime ChangeTime);

}
