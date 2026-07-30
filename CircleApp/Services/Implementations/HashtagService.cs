using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CircleApp.Services.Implementations
{
    public class HashtagService : IHashtagService
    {
        private readonly AppDbContext _context;

        public HashtagService(AppDbContext context)
        {
            _context = context;
        }

        public async Task ProcessHashtagsForPostAsync(Post post)
        {
            if (string.IsNullOrWhiteSpace(post.Content)) return;

            var matches = Regex.Matches(post.Content, @"#\w+");
            var extractedTags = matches.Cast<Match>()
                .Select(m => m.Value.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (!extractedTags.Any()) return;

            foreach (var tagText in extractedTags)
            {
                var existingHashtag = await _context.hashtags
                    .Include(h => h.Posts)
                    .FirstOrDefaultAsync(h => h.Tag.ToLower() == tagText.ToLower());

                if (existingHashtag != null)
                {
                    if (!existingHashtag.Posts.Any(p => p.Id == post.Id))
                    {
                        existingHashtag.Posts.Add(post);
                        existingHashtag.UsageCount++;
                    }
                }
                else
                {
                    var newHashtag = new Hashtag
                    {
                        Tag = tagText,
                        UsageCount = 1
                    };
                    newHashtag.Posts.Add(post);
                    await _context.hashtags.AddAsync(newHashtag);
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task RemoveHashtagsForPostAsync(int postId)
        {
            var post = await _context.posts
                .IgnoreQueryFilters()
                .Include(p => p.Hashtags)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null || !post.Hashtags.Any()) return;

            var hashtagsToRemoveFromDb = new List<Hashtag>();

            foreach (var hashtag in post.Hashtags.ToList())
            {
                hashtag.UsageCount--;
                if (hashtag.UsageCount <= 0)
                {
                    hashtagsToRemoveFromDb.Add(hashtag);
                }
            }

            if (hashtagsToRemoveFromDb.Any())
            {
                _context.hashtags.RemoveRange(hashtagsToRemoveFromDb);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<List<HashtagVM>> GetTrendingHashtagsAsync()
        {
            return await _context.hashtags
                .Where(h => h.UsageCount > 0)
                .OrderByDescending(h => h.UsageCount)
                .Select(h => new HashtagVM
                {
                    Tag = h.Tag,
                    PostCount = h.UsageCount
                })
                .Take(5)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsByHashtagAsync(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag)) return new List<Post>();

            string normalizedTag = tag.StartsWith("#") ? tag : "#" + tag;
            int loggedInUserId = 1;

            return await _context.posts
                .Where(p => (!p.IsPrivate || p.UserId == loggedInUserId) && p.Reports.Count < 5)
                .Where(p => p.Hashtags.Any(h => h.Tag.ToLower() == normalizedTag.ToLower()) || p.Content.Contains(normalizedTag))
                .Include(p => p.User)
                .Include(p => p.Likes)
                .Include(p => p.Favorites)
                .Include(p => p.Comments).ThenInclude(c => c.User)
                .Include(p => p.Reports)
                .OrderByDescending(p => p.DataCreated)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
