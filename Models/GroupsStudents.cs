using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class GroupsStudents
    {
        public int Id { get; set; }
               
        public int IdGroups { get; set; }

        public string IdStudent { get; set; }

        public string IdTeacher { get; set; }
                
        public string UserId { get; set; }
    }
}
