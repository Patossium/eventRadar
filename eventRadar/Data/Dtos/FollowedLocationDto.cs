﻿using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedLocationDto(int Id, string userId, User User, Location Location, int LocationId);
    public record CreateFollowedLocationDto();
}
