using Microsoft.AspNetCore.Identity;

namespace WorkOrderFlow.Web.Data;

public static class IdentityDataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string demoEmail = "demo@workorderflow.local";
        const string demoPassword = "Demo123!";

        var existingUser = await userManager.FindByEmailAsync(demoEmail);

        if (existingUser != null)
        {
            if (!existingUser.EmailConfirmed)
            {
                existingUser.EmailConfirmed = true;
                await userManager.UpdateAsync(existingUser);
            }

            return;
        }

        var demoUser = new IdentityUser
        {
            UserName = demoEmail,
            Email = demoEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(demoUser, demoPassword);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Demo user could not be created: {errors}");
        }
    }
}