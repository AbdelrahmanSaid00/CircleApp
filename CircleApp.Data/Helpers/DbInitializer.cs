using CircleApp.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CircleApp.Data.Helpers
{
    public class DbInitializer
    {
        public static async Task SeedAsync(AppDbContext appDbContext)
        {
            if (!appDbContext.users.Any() &&  !appDbContext.posts.Any())
            {
                var newUser = new User()
                {
                    FullName = "Ervis Trupja",
                    ProfilePictureUrl = "https://img-b.udemycdn.com/user/200_H/16004620_10db_5.jpg"
                };
                await appDbContext.users.AddAsync(newUser);
                await appDbContext.SaveChangesAsync();

                var newPostWithoutImage = new Post()
                {
                    Content = "This is going to be our first post which is being loaded from the database and it has been created using our test user.",
                    ImageUrl = "",
                    NrOfReports = 0,
                    DataCreated = DateTime.UtcNow,
                    DataUpdated = DateTime.UtcNow,

                    UserId = newUser.Id
                };

                var newPostWithImage = new Post()
                {
                    Content = "This is going to be our second post which is being loaded from the database and it has been created using our test user.",
                    ImageUrl = "https://cdn.pixabay.com/photo/2015/04/23/22/00/tree-736885_1280.jpg",
                    NrOfReports = 0,
                    DataCreated = DateTime.UtcNow,
                    DataUpdated = DateTime.UtcNow,
                    UserId = newUser.Id
                };

                await appDbContext.posts.AddRangeAsync(newPostWithoutImage,newPostWithImage);
                await appDbContext.SaveChangesAsync();
            }
        }
    }
}
