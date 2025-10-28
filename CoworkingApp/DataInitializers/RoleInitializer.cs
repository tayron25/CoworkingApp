using Microsoft.AspNetCore.Identity;

namespace CoworkingApp.DataInitializers
{
    public static class RoleInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "Cliente" }; // Aquí defines los roles

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    // Crea el rol si no existe
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}