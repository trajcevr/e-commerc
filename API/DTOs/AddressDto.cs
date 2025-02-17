using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class AddressDto
    {
        [Required(ErrorMessage = "Address Line 1 is required.")]
        [MaxLength(100, ErrorMessage = "Address Line 1 cannot exceed 100 characters.")]
        public string Line1 { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Address Line 2 cannot exceed 100 characters.")]
        public string? Line2 { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "State is required.")]
        [MaxLength(50, ErrorMessage = "State cannot exceed 50 characters.")]
        public string State { get; set; } = string.Empty;

        [Required(ErrorMessage = "Postal Code is required.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; } = string.Empty;
    } 
}
