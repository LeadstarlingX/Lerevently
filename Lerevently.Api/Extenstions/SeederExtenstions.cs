﻿using Microsoft.AspNetCore.Builder;

namespace Lerevently.Api.Extenstions;

public static class SeederExtenstions
{
    public static async Task SeedDataAsync(this IApplicationBuilder app, bool forceReseed = false)
    {
        await Lerevently.Modules.Users.Infrastructure.DataSeeder.DataSeeder.SeedDataAsync(app, forceReseed);
        await Lerevently.Modules.Events.Infrastructure.DataSeeder.DataSeeder.SeedDataAsync(app, forceReseed);
        await Lerevently.Modules.Ticketing.Infrastructure.DataSeeder.DataSeeder.SeedDataAsync(app, forceReseed);
    }
}