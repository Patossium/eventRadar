namespace eventRadar.Data.Dtos
{
    public record WebsiteDto(int Id, string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath);
    public record CreateWebsiteDto(string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath);
    public record UpdateWebsiteDto(string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath);
}
