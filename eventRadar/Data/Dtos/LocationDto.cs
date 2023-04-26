namespace eventRadar.Data.Dtos
{
    public record LocationDto(int Id, string Name, string City);
    public record CreateLocationDto(string Name, string City);
    public record UpdateLocationDto(string Name, string City);
}
