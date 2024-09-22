using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class Groups
    {
        public int Id { get; set; }
        
        [Required]
        public int? IdKindLanguages { get; set; }        
        
        [Required]
        public int? IdLevelKnowledge { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Length of group name can be between 3 and 50 symbols.")]
        public string GroupName { get; set; }

        public byte MaxCountStudents { get; set; }

        [StringLength(1000, ErrorMessage = "Length of notes can't be more than 1000 symbols.")]
        public string Notes { get; set; }

        public string IdSession { get; set; }
                
        public string UserId { get; set; }

        public string WhiteBoardUrl { get; set; }

        public string WhiteBoardId { get; set; }

        public bool TeacherIsConnected { get; set; }
    }
}
