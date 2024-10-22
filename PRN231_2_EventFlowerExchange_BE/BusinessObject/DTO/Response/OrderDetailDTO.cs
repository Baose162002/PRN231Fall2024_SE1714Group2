namespace BusinessObject.DTO.Response
{
    public class OrderDetailDTO
    {
        public int QuantityOrdered { get; set; }
        public double Price { get; set; }
        public double TotalPrice => QuantityOrdered * Price;
        public int OrderId { get; set; } //mapping
        public int BatchId { get; set; } // Thêm thuộc tính BatchId
        public ListFlowerDTO Flower { get; set; } // Display Flower details
    }
}