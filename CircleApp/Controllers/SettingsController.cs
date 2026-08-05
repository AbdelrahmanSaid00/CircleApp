using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace CircleApp.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IFileService _fileService;
        private readonly ILogger<SettingsController> _logger;
        public SettingsController(IProfileService profileService, IFileService fileService, ILogger<SettingsController> logger)
        {
            _profileService = profileService;
            _fileService = fileService;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int loggedInUserId = 1;
            var user = await _profileService.GetUserProfileAsync(loggedInUserId);
            var model = new UpdateProfileVM
            {
                FullName = user?.FullName,
                ProfilePictureUrl = user?.ProfilePictureUrl
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfilePicture(IFormFile profilePicture)
        {
            if (profilePicture != null && profilePicture.Length > 0)
            {
                int loggedInUserId = 1;
                var fileUrl = await _fileService.UploadFileAsync(profilePicture , "images/profiles");
                if (!string.IsNullOrEmpty(fileUrl))
                {
                    await _profileService.UpdateUserProfilePicture(loggedInUserId, fileUrl);
                }
            }
            return RedirectToAction("Index");
        }
    }
}
