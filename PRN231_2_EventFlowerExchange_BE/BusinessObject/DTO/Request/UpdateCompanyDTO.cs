using System.ComponentModel.DataAnnotations;

namespace BusinessObject.DTO.Request
{
    public class UpdateCompanyDTO
    {
        [Required(ErrorMessage = "Company name is required")]
        public string CompanyName { get; set; }

        [Required(ErrorMessage = "Company address is required")]
        public string CompanyAddress { get; set; }

        public string CompanyDescription { get; set; }

        [Required(ErrorMessage = "Tax number is required")]
        public string TaxNumber { get; set; }

        [Required(ErrorMessage = "Postal code is required")]
        public string PostalCode { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        public int UserId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public BusinessObject.Enum.EnumList.Status Status { get; set; }
    }
}
