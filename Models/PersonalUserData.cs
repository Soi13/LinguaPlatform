using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace LinguaPlatform.Models
{
    public class PersonalUserData
    {
        public int Id { get; set; }

      
        public string IdUser { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Length of name can be between 2 and 100 symbols.")]
        public string Name { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Length of surname can be between 2 and 100 symbols.")]
        public string Surname { get; set; }

        [Required]       
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(1, ErrorMessage = "Length of gender can be only 1 symbol.")]
        public string Gender { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Length of city can be between 3 and 100 symbols.")]
        public string City { get; set; }

    }
}
