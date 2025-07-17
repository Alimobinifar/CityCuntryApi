using System.ComponentModel.DataAnnotations;

namespace CityCuntryApi.Models
{
    public class Country
    {
        [Key]
        public int Id { get; set; }
        public string CountryCode { get; set; }

        [Required]
        public string CountryName { get; set; }
    }
}
