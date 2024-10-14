using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BusinessObject
{
    [Table("Company")]
    public class Company
    {
        [Key]
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyDescription { get; set; }

        public string TaxNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User Seller { get; set; }
        public Enum.EnumList.Status Status { get; set; }
        [JsonIgnore]

        public ICollection<Batch> Batches { get; set; }
    }
}
