using CircleApp.Data;
using CircleApp.Data.Models;
using CircleApp.Services.Interfaces;
using CircleApp.ViewModels.Home;
using Microsoft.EntityFrameworkCore;

namespace CircleApp.Services.Implementations
{
    public class ProfileService : IProfileService
    {
        private readonly AppDbContext _db;
        public ProfileService(AppDbContext db)
        {
            _db = db;
        }
        //public async Task<bool> ChangepasswordAsync(ChangePasswordVM model, int userId)
        //{
        //    var user = _db.users.FirstOrDefault(u => u.Id == userId);
        //    if (user != null)
        //    {
        //        if (user.Password == model.CurrentPassword)
        //        {
        //            user.Password = model.NewPassword;
        //            await _db.SaveChangesAsync();
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }

        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        public async Task<User>? GetUserProfileAsync(int userId)
        {
            return await _db.users.FirstOrDefaultAsync(f => f.Id == userId);
        }

        //public Task<bool> UpdateProfileAsync(UpdateProfileVM model, int userId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task UpdateUserProfilePicture(int userId, string profilePictureUrl)
        {
            var user = await _db.users.FirstOrDefaultAsync(f => f.Id == userId);
            if (user != null)
            {
                user.ProfilePictureUrl = profilePictureUrl;
                await _db.SaveChangesAsync();
            }
        }
    }
}
