using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedLocationDto(string Id, string UserId, User User, Location Location, string LocationId);
    public record CreateFollowedLocationDto();
}
