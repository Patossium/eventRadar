using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedLocationDto(int Id, int UserId, User User, int LocationId, Location Location);
    public record CreateFollowedLocationDto(int UserId, User User, int LocationId, Location Location);
    public record UpdateFollowedLocationDto(int UserId, User User, int LocationId, Location Location);
}
