namespace BusinessObject.DTO.Response
{
    public class OrderDetailDTO
    {
        public ListBatchDTO Batch { get; set; }
        public int BatchId { get; set; } // Thêm thuộc tính BatchId
        public ListFlowerDTO Flower { get; set; } // Display Flower details

        public int QuantityOrdered { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice => QuantityOrdered * Price;
        public int OrderId { get; set; }
    }
}