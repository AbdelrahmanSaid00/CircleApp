using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CircleApp.Services.Implementations
{
    public class StoryService : IStoryService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public StoryService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<List<Story>> GetActiveStoriesAsync()
        {
            DateTime cutoff = DateTime.Now.AddHours(-24);
            return await _context.stories
                .Where(s => s.DateCreated >= cutoff)
                .Include(s => s.User)
                .OrderByDescending(s => s.DateCreated)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Story?> GetStoryByIdAsync(int id)
        {
            return await _context.stories
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task AddStoryAsync(Story story)
        {
            await _context.stories.AddAsync(story);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteStoryAsync(int id)
        {
            var story = await _context.stories.FirstOrDefaultAsync(s => s.Id == id);
            if (story != null)
            {
                if (!string.IsNullOrEmpty(story.ImageUrl))
                {
                    _fileService.DeleteFile(story.ImageUrl);
                }
                _context.stories.Remove(story);
                await _context.SaveChangesAsync();
            }
        }
    }
}
