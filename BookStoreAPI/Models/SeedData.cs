using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreAPI.Models
{
    public static class SeedData
    {
        private static string strAdmin = "Administrator";
        private static string strCustomer = "Customer";


        public async static Task Seed(UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            await SeedRoles(roleManager);
            await SeedUsers(userManager);
        }

        private async static Task CreateUser(UserManager<IdentityUser> userManager, 
            string userName, string email, string password, string role)
        {
            //1 - if user doesnt exist
            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new IdentityUser
                {
                    UserName = userName,
                    Email = email
                };

                //2 - create one
                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            await CreateUser(userManager, "admin", "admin@bookstore.com", "P@ssword1", strAdmin);
            await CreateUser(userManager, "customer1", "customer1@gmail.com", "P@ssword1", strCustomer);
            await CreateUser(userManager, "customer2", "customer2@gmail.com", "P@ssword1", strCustomer);
        }

        private async static Task CreateRole(RoleManager<IdentityRole> roleManager,
            string roleName)
        {
            //if role doesnt exist
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var role = new IdentityRole
                {
                    Name = roleName
                };

                //create one
                await roleManager.CreateAsync(role);
            }
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {

            await CreateRole(roleManager, strAdmin);
            await CreateRole(roleManager, strCustomer);
        }
    }
}
