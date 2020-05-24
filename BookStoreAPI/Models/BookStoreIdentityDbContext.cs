using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreAPI.Models
{
    public class BookStoreIdentityDbContext:IdentityDbContext
    {
        public BookStoreIdentityDbContext(DbContextOptions<BookStoreIdentityDbContext>options):base(options)
        {

        }
    }
}
