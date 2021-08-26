using Dangl.Identity.Shared;
using Microsoft.AspNetCore.Identity;
using System;

namespace Dangl.OpenCDE.Data.Models
{
    public class CdeUser : IdentityUser<Guid>, IDanglIdentityUser
    {
        public Guid IdenticonId { get; set; }
    }
}
