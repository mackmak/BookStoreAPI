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

        private async static Task SeedUsers(UserManager<IdentityUser> userManager)
        {
            //if there's no admin user
            if (await userManager.FindByEmailAsync("admin@bookstore.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "admin",
                    Email = "admin@bookstore.com"
                };

                //create one
                var result = await userManager.CreateAsync(user, "P@ssword1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, strAdmin);
                }
            }

            if (await userManager.FindByEmailAsync("customer1@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer1",
                    Email = "customer1@gmail.com"
                };

                var result = await userManager.CreateAsync(user, "P@ssword1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, strCustomer);
                }
            }

            if (await userManager.FindByEmailAsync("customer2@gmail.com") == null)
            {
                var user = new IdentityUser
                {
                    UserName = "customer2",
                    Email = "customer2@gmail.com"
                };

                var result = await userManager.CreateAsync(user, "P@ssword1");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, strCustomer);
                }
            }
        }

        private async static Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {

            //if there's no admin role
            if (! await roleManager.RoleExistsAsync(strAdmin))
            {
                var role = new IdentityRole
                {
                    Name = strAdmin
                };

                //create one
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync(strCustomer))
            {
                var role = new IdentityRole
                {
                    Name = strCustomer
                };

                await roleManager.CreateAsync(role);
            }

        }
    }
}
