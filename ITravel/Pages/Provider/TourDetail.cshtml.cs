using ITravel.Data;
using ITravel.Models;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ITravel.Pages.Provider
{
    public class TourDetailModel : PageModel
    {
        private readonly ITourRepository _tourRepository;

        public TourDetailModel(ITourRepository tourRepository)
        {
            _tourRepository = tourRepository;
        }
        public Tour Tour { get; set; }
        public async Task<IActionResult> OnGetAsync(Guid id)
        {
            Tour = await _tourRepository.GetTourByTourIdAsync(id);

            if (Tour == null)
            {
                return NotFound();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAddTourDateAsync(Guid tourId, DateTime StartDate, DateTime EndDate, int MaxCapacity)
        {
            var tour = await _tourRepository.GetTourByTourIdAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            var newTourDate = new TourDate
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                StartDate = StartDate,
                EndDate = EndDate,
                MaxCapacity = MaxCapacity,
                CurrentCapacity = 0,
                IsDeleted = false
            };

            _tourRepository.AddTourDate(newTourDate);

            return RedirectToPage(new { id = tourId });
        }

        public async Task<IActionResult> OnPostDeleteTourDateAsync(Guid tourId, Guid dateId)
        {
            _tourRepository.DeleteTourDate(dateId);

            return RedirectToPage(new { id = tourId });
        }

        public async Task<IActionResult> OnPostAddActivityAsync(Guid tourId, int DayNumber, string ActivityName, TimeSpan StartTime, TimeSpan EndTime, string Location, string Description)
        {
            var tour = await _tourRepository.GetTourByTourIdAsync(tourId);
            if (tour == null)
            {
                return NotFound();
            }

            var newActivity = new ActivitySchedule
            {
                Id = Guid.NewGuid(),
                Tour = tour,
                DayNumber = DayNumber,
                ActivityName = ActivityName,
                StartTime = StartTime,
                EndTime = EndTime,
                Location = Location,
                Description = Description
            };

            _tourRepository.AddActivitySchedule(newActivity);

            return RedirectToPage(new { id = tourId });
        }

        public async Task<IActionResult> OnPostDeleteActivityAsync(Guid tourId, Guid activityId)
        {
            _tourRepository.DeleteActivitySchedule(activityId);

            return RedirectToPage(new { id = tourId });
        }
    }
}
