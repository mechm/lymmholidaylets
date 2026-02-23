using System.ComponentModel.DataAnnotations;

namespace LymmHolidayLets.Api.Models.Email;

public class EmailEnquiryRequest
{
    [Display(Name = "Your Name")]
    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "The {0} must be a maximum of {1} characters long")]
    public required string Name { get; set; }

    [Display(Name = "Company Name")]
    [StringLength(150, ErrorMessage = "{0} must be a maximum of {1} characters long")]
    public string? Company { get; set; }

    [Display(Name = "Email Address")]
    [Required(ErrorMessage = "Email Address is required")]
    [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 5)]
    [EmailAddress(ErrorMessage = "The {0} must be a valid email address")]
    public string? EmailAddress { get; set; }

    [Display(Name = "Telephone Number")]
    [RegularExpression(@"^\(?(?:(?:0(?:0|11)\)?[\s-]?\(?|\+)44\)?[\s-]?\(?(?:0\)?[\s-]?\(?)?|0)(?:\d{2}\)?[\s-]?\d{4}[\s-]?\d{4}|\d{3}\)?[\s-]?\d{3}[\s-]?\d{3,4}|\d{4}\)?[\s-]?(?:\d{5}|\d{3}[\s-]?\d{3})|\d{5}\)?[\s-]?\d{4,5}|8(?:00[\s-]?11[\s-]?11|45[\s-]?46[\s-]?4\d))(?:(?:[\s-]?(?:x|ext\.?\s?|\#)\d+)?)$", ErrorMessage = "The {0} must be in the correct format")]
    [StringLength(30, ErrorMessage = "{0} must be at least {2} characters long and maximum {1} characters long", MinimumLength = 5)]
    public string? TelephoneNo { get; set; }

    [StringLength(200)]
    public string? Subject { get; set; }

    [Required(ErrorMessage = "Message is required")]
    [StringLength(5000)]
    public required string Message { get; set; }

    public string? ReCaptchaToken { get; set; }
}