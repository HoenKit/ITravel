using ITravel.Models;
using ITravel.Repository.Interfaces;
using ITravel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages
{
    public class RecommendAIPageModel : PageModel
    {
        [BindProperty]
        public string Gender { get; set; }

        [BindProperty]
        public int Age { get; set; }

        [BindProperty]
        public string Interests { get; set; }

        [BindProperty]
        public string TravelType { get; set; }

        public string Suggestions { get; set; }

        private readonly AimlService _aimlService;
        public List<string> SuggestionList { get; set; } = new List<string>();
        private readonly ITourRepository _tourRepository;
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
        public string? Location { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? Price { get; set; }

        public RecommendAIPageModel(AimlService aimlService, ITourRepository tourRepository)
        {
            _aimlService = aimlService;
            _tourRepository = tourRepository;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            ModelState.Remove(nameof(Page));
            var interestsArray = Interests.Split(',').Select(i => i.Trim()).ToArray();
            if (interestsArray.Length > 2 || interestsArray.Any(i => i.Length > 20))
            {
                ModelState.AddModelError("Interests", "Chỉ được nhập tối đa 2 sở thích, mỗi sở thích không quá 20 ký tự.");
            }

            if (!ModelState.IsValid)
            {
                return Page();
            }
            string userPrompt = $"Tôi là {Gender.ToLower()}, {Age} tuổi, thích {Interests}. Muốn du lịch {TravelType.ToLower()}. Hãy đề xuất các tỉnh thành ở Việt Nam phù hợp, không mô tả, định dạng: Tỉnh thành 1, Tỉnh thành 2.";
            Suggestions = await _aimlService.GetChatResponseAsync(userPrompt);
            if (!string.IsNullOrEmpty(Suggestions))
            {
                SuggestionList = Suggestions.Split(',')
                                            .Select(s => s.Trim())
                                            .Where(s => !string.IsNullOrEmpty(s))
                                            .ToList();
                PagedTours = await _tourRepository.GetToursPagedRecommendAsync(Page, PageSize, Price, StartDate, EndDate, SuggestionList);
            }
            return Page();
        }
    }
}

