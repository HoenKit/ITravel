using ITravel.Models;
using ITravel.Repository.Interfaces;
using ITravel.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ITravel.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ITourRepository _tourRepository;
        private readonly AimlService _aimlService;

        public IndexModel(ILogger<IndexModel> logger, ITourRepository tourRepository, AimlService aimlService)
        {
            _logger = logger;
            _tourRepository = tourRepository;
            _aimlService = aimlService;
        }
        public ICollection<TourDate> TourDateList { get; set; }
        [BindProperty]
        public string UserPrompt { get; set; }

        public string ApiResponse { get; set; }
        public void OnGet()
        {
            TourDateList = _tourRepository.Get5RecentTours();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            TourDateList = _tourRepository.Get5RecentTours();
            UserPrompt = "Tôi là nam, 22 tuổi, thích thể thao, chơi game, muốn du lịch một mình. Hãy đề xuất các tỉnh thành ở Việt Nam phù hợp, không mô tả, định dạng: Tỉnh thành 1, Tỉnh thành 2.";
            if (!string.IsNullOrEmpty(UserPrompt))
            {
                ApiResponse = await _aimlService.GetChatResponseAsync(UserPrompt);
            }
            return Page();
        }
    }
}
