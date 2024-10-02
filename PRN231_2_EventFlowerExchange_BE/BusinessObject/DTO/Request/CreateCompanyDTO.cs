namespace BusinessObject.DTO.Request
{
    public class CreateCompanyDTO
    {
        public int CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyDescription { get; set; }
        public int SellerId { get; set; }
    }
}
