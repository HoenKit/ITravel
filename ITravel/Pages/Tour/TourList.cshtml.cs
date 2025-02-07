using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages.Tour
{
    public class TourListModel : PageModel
    {
        private readonly ITourRepository _tourRepository;
        public TourListModel(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }
        public PageResult<TourDate> PagedTours { get; set; }

        [BindProperty(SupportsGet = true)]
        public int Page { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = 5;

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Location { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Price { get; set; }
        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            PagedTours = await _tourRepository.GetToursPagedAsync(pageIndex, PageSize, Price, StartDate, EndDate, Location);
            return Page();
        }
    }
}
