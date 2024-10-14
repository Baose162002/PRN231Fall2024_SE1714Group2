namespace BusinessObject.DTO.Request
{
    public class CreateCompanyDTO
    {
        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyDescription { get; set; }
        public string TaxNumber { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public int UserId { get; set; }
    }
}
