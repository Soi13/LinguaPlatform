using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LinguaPlatform.Models
{
    public class User:IdentityUser
    {
        public string LimnuToken { get; set; }
        public bool IsActive { get; set; }
        public string LimnuUserId { get; set; }
        public string LimnuSecret { get; set; }
    }
}
