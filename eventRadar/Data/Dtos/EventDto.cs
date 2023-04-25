using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record EventDto(string Id, string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, Location Location, Category Category);
    public record CreateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, Location Location, Category Category);
    public record UpdateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, Location Location, Category Category);
}
