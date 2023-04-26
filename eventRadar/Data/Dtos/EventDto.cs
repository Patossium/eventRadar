using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record EventDto(int Id, string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, string Location, string Category);
    public record CreateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, string Location, string Category);
    public record UpdateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicketLink, bool Updated, string Location, string Category);
}
