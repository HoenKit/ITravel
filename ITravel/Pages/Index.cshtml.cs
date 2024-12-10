using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITourRepository _tourRepository;

        public IndexModel(ILogger<IndexModel> logger, ITourRepository tourRepository)
        {
            _logger = logger;
            _tourRepository = tourRepository;
        }
        public ICollection<TourDate> TourDateList { get; set; }
        public void OnGet()
        {
            TourDateList = _tourRepository.Get5RecentTours();
        }
    }
}
