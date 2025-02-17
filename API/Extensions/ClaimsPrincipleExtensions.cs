using System.Security.Claims;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static async Task<AppUser?> GetUserByEmail(this UserManager<AppUser> userManager, string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return null;
        return await userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public static async Task<AppUser?> GetCurrentUserAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        return await userManager.GetUserByEmail(email);
    }

    public static async Task<AppUser?> GetUserWithAddressAsync(this UserManager<AppUser> userManager, ClaimsPrincipal user)
    {
        var email = user.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return null;

        return await userManager.Users
            .Include(u => u.Address) 
            .FirstOrDefaultAsync(u => u.Email == email);
    }
}
