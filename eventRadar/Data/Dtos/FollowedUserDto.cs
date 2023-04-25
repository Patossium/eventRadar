﻿using eventRadar.Auth.Model;
using eventRadar.Models;

namespace eventRadar.Data.Dtos
{
    public record FollowedUserDto(string id, string UserId, User User, User Followed_User, string Followed_UserId);
    public record CreateFollowedUserDto();
}
