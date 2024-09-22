using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class DelTeacherFromGroup
    {
        [Required]
        public int IdGroups { get; set; }

        [Required]
        public string IdTeacher { get; set; }
    }
}
