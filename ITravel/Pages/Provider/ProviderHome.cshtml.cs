using ITravel.Models;
using ITravel.Repository.Implements;
using ITravel.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace ITravel.Pages.Provider
{
    public class ProviderHomeModel : PageModel
    {
        private readonly ITourRepository _tourRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        public ProviderHomeModel(ITourRepository tourRepository, IProviderRepository providerRepository, IUserRepository userRepository, IWebHostEnvironment environment)
        {
            _tourRepository = tourRepository;
            _providerRepository = providerRepository;
            _userRepository = userRepository;
            _environment = environment;
        }
        public int PageSize { get; set; } = 3;
        public PageResult<Tour> PageTours { get; set; }
        [BindProperty]
        public Tour Tours { get; set; }
        [BindProperty]
        public string UserId { get; set; }
        [BindProperty]
        public AppUser Users { get; set; }

        [Required(ErrorMessage = "Please choose at least one file.")]
        [DataType(DataType.Upload)]
        [FileExtensions(Extensions = "png,jpg,jpeg,gif", ErrorMessage = "Only image files (png, jpg, jpeg, gif) are allowed.")]
        [Display(Name = "Choose file(s) to upload")]
        [BindProperty]
        public List<IFormFile>? FileUploads { get; set; }
        public async Task<IActionResult> OnGetAsync(int pageIndex = 1)
        {
            Tours = new Tour();
            UserId = await _userRepository.GetUserIdAsync(User.Identity.Name);
            if (string.IsNullOrEmpty(UserId))
            {
                return NotFound("User not found.");
            }

            var provider = await _providerRepository.GetProviderByUserIdAsync(UserId);
            if (provider == null)
            {
                return NotFound("Provider not found for this UserId.");
            }
            PageTours = await _tourRepository.GetTourByProviderIdPageAsync(pageIndex, PageSize, provider.Id);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteTourAsync(Guid id)
        {
            int pageIndex = 1;
            _tourRepository.DeleteTour(id);
            UserId = await _userRepository.GetUserIdAsync(User.Identity.Name);
            if (string.IsNullOrEmpty(UserId))
            {
                return NotFound("User not found.");
            }

            var provider = await _providerRepository.GetProviderByUserIdAsync(UserId);
            if (provider == null)
            {
                return NotFound("Provider not found for this UserId.");
            }
            PageTours = await _tourRepository.GetTourByProviderIdPageAsync(pageIndex, PageSize, provider.Id);
            
            return await OnGetAsync(pageIndex);
        }

        public async Task<IActionResult> OnPostAddTour()
        {
            int pageIndex = 1;
            string savePath = "Tour-Image";
            string fileName = null;

            UserId = await _userRepository.GetUserIdAsync(User.Identity.Name);
            var provider = await _providerRepository.GetProviderByUserIdAsync(UserId);
            if (provider == null)
            {
                return NotFound("Provider not found.");
            }

            Tours.ProviderId = provider.Id;

            List<Image> tourImages = new List<Image>();

            //if (!ModelState.IsValid)
            //{
            //    return await OnGetAsync(pageIndex);
            //}

            try
            {
                if (FileUploads != null)
                {
                    var directoryPath = Path.Combine(_environment.WebRootPath, savePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    foreach (var FileUpload in FileUploads)
                    {
                        fileName = Path.GetFileName(FileUpload.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await FileUpload.CopyToAsync(fileStream);
                        }

                        var imageUrl = Path.Combine(savePath, fileName).Replace("\\", "/");
                        tourImages.Add(new Image { Id = Guid.NewGuid(), URL = imageUrl, Tour = Tours });
                    }
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileUpload", "Error uploading file: " + ex.Message);
                return await OnGetAsync(pageIndex);
            }



            Tours.Images = tourImages;
            await _tourRepository.AddTour(Tours);
            return await OnGetAsync(pageIndex);
        }


        public async Task<IActionResult> OnPostUpdateTour()
        {
            int pageIndex = 1;
            string savePath = "Tour-Image";
            string fileName = null;

            UserId = await _userRepository.GetUserIdAsync(User.Identity.Name);
            var provider = await _providerRepository.GetProviderByUserIdAsync(UserId);
            if (provider == null)
            {
                return NotFound("Provider not found.");
            }

            var tourexist = await _tourRepository.GetTourByTourIdAsync(Tours.Id);
            if (tourexist == null)
            {
                return NotFound("Tour not found.");
            }
            tourexist.Name = Tours.Name;
            tourexist.Description = Tours.Description;
            tourexist.Location = Tours.Location;
            tourexist.Price = Tours.Price;

            List<Image> tourImages = new List<Image>();

            try
            {
                if (FileUploads != null)
                {
                    var directoryPath = Path.Combine(_environment.WebRootPath, savePath);
                    if (!Directory.Exists(directoryPath))
                    {
                        Directory.CreateDirectory(directoryPath);
                    }

                    foreach (var FileUpload in FileUploads)
                    {
                        fileName = Path.GetFileName(FileUpload.FileName);
                        var filePath = Path.Combine(directoryPath, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await FileUpload.CopyToAsync(fileStream);
                        }

                        var imageUrl = Path.Combine(savePath, fileName).Replace("\\", "/");
                        tourImages.Add(new Image { Id = Guid.NewGuid(), URL = imageUrl, Tour = Tours });
                    }
                    tourexist.Images = tourImages;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("FileUpload", "Error uploading file: " + ex.Message);
                return await OnGetAsync(pageIndex);
            }
            await _tourRepository.Update(tourexist);
            return await OnGetAsync(pageIndex);
        }

      
    }
}
