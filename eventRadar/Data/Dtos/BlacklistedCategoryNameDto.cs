namespace eventRadar.Data.Dtos
{
    public record BlacklistedCategoryNameDto(int Id,string Name);
    public record CreateBlacklistedCategoryNameDto(string Name);
    public record UpdateBlacklistedCategoryNameDto(string Name);
}
