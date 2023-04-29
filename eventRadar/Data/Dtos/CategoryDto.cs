namespace eventRadar.Data.Dtos
{
    public record CategoryDto (int Id, string Name, string SourceUrl);
    public record CreateCategoryDto(string Name, string SourceUrl);   
    public record UpdateCategoryDto(string Name, string SourceUrl);
}
