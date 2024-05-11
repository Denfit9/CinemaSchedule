using CinemaSchedule.Areas.Identity.Data;
using CinemaSchedule.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace CinemaSchedule.Controllers
{
    [Authorize]
    public class ProfileEditController : Controller
    {
        private readonly UserManager<User> _userManager;
        public string errorMessage = "";
        public ProfileEditController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [Route("Profile/Edit")]
        [HttpGet]
        public IActionResult ProfileEdit()
        {
            return View();
        }


        [Route("Profile/Edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProfileEdit(ProfileEditViewModel profileEditViewModel)
        {
            var user = await _userManager.GetUserAsync(User);
            user.FirstName = profileEditViewModel.FirstName;
            user.LastName = profileEditViewModel.LastName;
            if (profileEditViewModel.ProfilePicture == null)
            {

            }
            else
            {
                if (profileEditViewModel.ProfilePicture.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await profileEditViewModel.ProfilePicture.CopyToAsync(memoryStream);

                        // Upload the file if less than 2 MB  
                        if (memoryStream.Length < 2097152)
                        {
                            user.ProfilePicture = memoryStream.ToArray();
                        }
                        else
                        {
                            ModelState.AddModelError("File", "Файл слишком большой");
                        }
                    }
                }
            }
            
            if(profileEditViewModel.FirstName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(ProfileEditViewModel.FirstName), "Необходимо заполнить поле имени хоят бы двумя символами!");
                return View(profileEditViewModel);
            }
            else if (profileEditViewModel.FirstName.Length < 2)
            {
                ModelState.AddModelError(nameof(ProfileEditViewModel.FirstName), "Необходимо заполнить поле имени хоят бы двумя символами!");
                return View(profileEditViewModel);
            }
            if (profileEditViewModel.LastName.IsNullOrEmpty())
            {
                ModelState.AddModelError(nameof(ProfileEditViewModel.LastName), "Необходимо заполнить поле фамилии хоят бы двумя символами!");
                return View(profileEditViewModel);
            }
            else if (profileEditViewModel.LastName.Length < 2)
            {
                ModelState.AddModelError(nameof(ProfileEditViewModel.LastName), "Необходимо заполнить поле фамилии хоят бы двумя символами!");
                return View(profileEditViewModel);
            }


            if(profileEditViewModel.Password != user.Password || profileEditViewModel.Password.Length==0)
            {
                ModelState.AddModelError(nameof(ProfileEditViewModel.Password), "Неправильный пароль!");
                return View(profileEditViewModel);
            }
            else
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Profile", "Profile");
                }
                return View(profileEditViewModel);
            }
            
        }
    }
}
