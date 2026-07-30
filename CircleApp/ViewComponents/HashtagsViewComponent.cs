using CircleApp.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CircleApp.ViewComponents
{
    public class HashtagsViewComponent : ViewComponent
    {
        private readonly IHashtagService _hashtagService;

        public HashtagsViewComponent(IHashtagService hashtagService)
        {
            _hashtagService = hashtagService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var hashtags = await _hashtagService.GetTrendingHashtagsAsync();
            return View(hashtags);
        }
    }
}
