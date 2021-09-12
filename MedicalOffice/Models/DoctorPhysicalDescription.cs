using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MedicalOffice.Models
{
    public class DoctorPhysicalDescription
    {
        [Key, ForeignKey("Doctor")]
        public int DoctorID { get; set; }

        [Display(Name = "Height (meters)")]
        [Required(ErrorMessage = "You cannot leave the height blank.")]
        [Range(0.5d, 10d, ErrorMessage = "The height must be between 0.5 and 10 meters.")]
        public double Height { get; set; }

        [Display(Name = "Weight (kg)")]
        [Required(ErrorMessage = "You cannot leave theWeight height blank.")]
        [Range(10d, 500d, ErrorMessage = "The weight must be between 10 and 500 kilograms.")]
        public double Weight { get; set; }

        [Display(Name = "Hair Colour")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Hair colour needs to be between 3 and 50 characters.")]
        public string HairColour { get; set; }

        [Display(Name = "Identifying Marks")]
        [StringLength(2000, ErrorMessage = "Only 2000 characters for Identifying Marks.")]
        [DataType(DataType.MultilineText)]
        public string IdentifyingMarks { get; set; }
    }
}
