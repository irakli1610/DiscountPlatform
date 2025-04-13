// Copyright (C) TBC Bank. All Rights Reserved.

using System.Security.Claims;

namespace offers.itacademy.ge.API.Utils
{
    public static class AuthUtils
    {
        public static int GetUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirstValue("UserId");
            if (!int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid or missing UserId claim.");

            return userId;
        }
    }
}
