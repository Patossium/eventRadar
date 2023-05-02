namespace eventRadar.Data.Dtos
{
    public record WebsiteDto(int Id, string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath, string UrlExtensionForEvent, string EventLink, string CategoryLink, string PagerLink, string TicketLinkType, string FullLocationPath);
    public record CreateWebsiteDto(string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath, string UrlExtensionForEvent, string EventLink, string CategoryLink, string PagerLink, string TicketLinkType, string FullLocationPath);
    public record UpdateWebsiteDto(string Url, string TitlePath, string LocationPath, string PricePath, string DatePath, string ImagePath, string TicketPath, string UrlExtensionForEvent, string EventLink, string CategoryLink, string PagerLink, string TicketLinkType, string FullLocationPath);
}
