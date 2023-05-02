namespace eventRadar.Data.Dtos
{
    public record LocationDto(int Id, string Name, string City, string Country, string Address);
    public record CreateLocationDto(string Name, string City, string Country, string Address);
    public record UpdateLocationDto(string Name, string City, string Country, string Address);
}
