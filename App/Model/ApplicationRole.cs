using System;
using Microsoft.AspNetCore.Identity;
namespace StoreModels
{
  public class ApplicationRole : IdentityRole<Guid>
  {
    public string Description { get; set; }
  }
}


