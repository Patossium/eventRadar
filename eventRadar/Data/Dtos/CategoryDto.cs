namespace eventRadar.Data.Dtos
{
    public record CategoryDto (int Id, string Name,string WebsiteName);
    public record CreateCategoryDto(string Name, string WebsiteName);   
    public record UpdateCategoryDto(string Name, string WebsiteName);
}
