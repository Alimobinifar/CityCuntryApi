using System.ComponentModel.DataAnnotations.Schema;

namespace CityCuntryApi.Models
{
    public class City

    {
        public int Id { get; set; }
        public string CityName { get; set; }

        [ForeignKey("State")]
        public int StateId { get; set; }
        public State State { get; set; }    

    }
}
