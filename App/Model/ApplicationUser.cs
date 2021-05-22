using System;
using Microsoft.AspNetCore.Identity;

namespace StoreModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string CustomTag { get; set; }
    }
}