using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record EventDto(int Id, string Url, string Title, string Date, string ImageLink, double Price, string TicetLink, bool Updated, Location Location, Category Category);
    public record CreateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicetLink, bool Updated, Location Location, Category Category);
    public record UpdateEventDto(string Url, string Title, string Date, string ImageLink, double Price, string TicetLink, bool Updated, Location Location, Category Category);
}
