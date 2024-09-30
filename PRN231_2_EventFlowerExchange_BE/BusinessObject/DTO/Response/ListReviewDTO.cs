using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListReviewDTO
    {
        public int ReviewId { get; set; }
        public int Rating { get; set; }
        public string Feedback { get; set; }
        public DateTime ReviewDate { get; set; }
        public int BatchId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
    }
}
