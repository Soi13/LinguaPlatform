using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class DelStudentFromGroup
    {
        [Required]
        public int IdGroups { get; set; }

        [Required]
        public string IdStudent { get; set; }
    }
}
