using System;
using Microsoft.AspNetCore.Identity;

namespace StoreModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        [PersonalData]
        public string Name {get; set;}
        [PersonalData]
        public string Address {get; set;}
    }
}