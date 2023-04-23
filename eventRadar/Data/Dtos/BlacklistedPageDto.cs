namespace eventRadar.Data.Dtos
{
    public record BlacklistedPageDto(int Id, string Url, string Comment);
    public record CreateBlacklistedPageDto(string Url, string Comment);
    public record UpdateBlacklistedPageDto(string Url, string Comment);

}
