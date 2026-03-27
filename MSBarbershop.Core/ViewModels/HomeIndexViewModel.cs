using MSBarbershop.Data.Entities;

namespace MSBarbershop.Core.ViewModels.Home
{
    public class HomeIndexViewModel
    {
        public IEnumerable<Service> Services { get; set; } = new List<Service>();
        public IEnumerable<Barber> Barbers { get; set; } = new List<Barber>();
        public IEnumerable<Review> Reviews { get; set; } = new List<Review>();
    }
}