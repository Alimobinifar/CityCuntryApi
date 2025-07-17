using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CityCuntryApi.Models
{
    public class State 
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string StateName { get; set; }

        [ForeignKey("Country")]
        public int CountryId { get; set; }

        public Country Country { get; set; }
    }
}
