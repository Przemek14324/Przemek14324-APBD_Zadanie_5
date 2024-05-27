namespace TripApi.DTOs
{
    public class TripDto
    {
        public int IdTrip { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
