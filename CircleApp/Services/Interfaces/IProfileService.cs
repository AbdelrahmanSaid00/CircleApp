using CircleApp.Data.Models;
using CircleApp.ViewModels.Home;

namespace CircleApp.Services.Interfaces
{
    public interface IProfileService
    {
        Task<User> GetUserProfileAsync(int userId);
        //Task<bool> UpdateProfileAsync(UpdateProfileVM model, int userId);
        //Task<bool> ChangepasswordAsync(ChangePasswordVM model, int userId);
        Task UpdateUserProfilePicture(int userId, string profilePictureUrl);
    }
}
