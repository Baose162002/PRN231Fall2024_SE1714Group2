using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Request
{
    public class UpdateReviewDTO
    {
           [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }

        [Required]
        public string Feedback { get; set; }
        public DateTime ReviewDate { get; set; } = DateTime.Now;

    }
}
