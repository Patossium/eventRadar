namespace eventRadar.Data.Dtos
{
    public record CityDto(int Id, string Name);
    public record CreateCityDto(string Name);
    public record UpdateCityDto(string Name);
}
