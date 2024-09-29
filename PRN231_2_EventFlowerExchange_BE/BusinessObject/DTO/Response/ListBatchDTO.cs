using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTO.Response
{
    public class ListBatchDTO
    {
        public int BatchId { get; set; }
        public string FlowerType { get; set; }
        public int BatchQuantity { get; set; }
        public string ImgFlower { get; set; }
        public string Description { get; set; }
        public decimal PricePerUnit { get; set; }
        public string Condition { get; set; }
        public string EntryDate { get; set; }
        public string ExpirationDate { get; set; }
        public string BatchStatus { get; set; }    
        public ListCompanyDTO Company { get; set; }        
        public ListFlowerDTO Flower { get; set; }
    }
}
