using docmaster.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace docmaster.Areas.Identity.Data
{
    public static class ContextSeed
    {
        public static async Task SeedSuperAdminAsync(UserManager<docmasterUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            IdentityResult roleResult;
            bool adminRoleExists = await roleManager.RoleExistsAsync("Master");
            if (!adminRoleExists)
            {
               
                roleResult = await roleManager.CreateAsync(new IdentityRole("Master"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("SuperUser"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Admin"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Basic"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Premium"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Ultimate"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Suspended"));
                roleResult = await roleManager.CreateAsync(new IdentityRole("Banned"));
            }
            //Seed Default User
            var defaultUser = new docmasterUser
            {
                UserName = "farai@imspulse.com",
                Email = "farai@imspulse.com",
                FirstName = "Farai",
                LastName = "Chakarisa",
                Company = "IMS Pulse",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "@Paradice1");
                    await userManager.AddToRoleAsync(defaultUser, "Master");

                }

            }
        }
    }



    public enum Roles
    {
        SuperUser,
        Admin,
        Moderator,
        Basic,
        Premium,
        Ultimate,
        Suspended,
        Banned,
        Blocked,
    }
}
