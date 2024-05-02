using Demo.DAL.Models;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demo.PL.Controllers
{
	//[AllowAnonymous]//default allow anyone to call actions
    public class AccountController : Controller
    {
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly SignInManager<ApplicationUser> _signInManager;

		public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
            )
        {
			_userManager = userManager;
			_signInManager = signInManager;
		}
		#region Sign Up
		public IActionResult SignUp()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpViewModel model)
		{
			if (ModelState.IsValid) //server side validation
			{
				var user = await _userManager.FindByNameAsync(model.UserName);
				if (user is null)
				{
					user = new ApplicationUser()
					{
						UserName = model.UserName,
						Email = model.Email,
						IsAgree = model.IsAgree,
						FName = model.FName,
						LName = model.LName,
						PhoneNumber=model.PhoneNumber
					};
					//Create User
					var result = await _userManager.CreateAsync(user, model.Password);
					if (result.Succeeded)
						return RedirectToAction(nameof(SignIn));
					foreach(var error in result.Errors)
						ModelState.AddModelError(string.Empty,error.Description);
				}
				ModelState.AddModelError(string.Empty, "UserName Is Already Exist");


			}
			return View(model);

		}
		#endregion

		#region Sign In
		public IActionResult SignIn()
		{
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> SignIn(SignInViewModel model)
		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if(user is not null)
				{
					var flag=await _userManager.CheckPasswordAsync(user, model.Password);
					if (flag) //Correct Password 
					{
						var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
						//Third parameter represent IsPresist 
						//Browser has 3 places for storage (Local ,Session ,Cookie)
						//Token(In MVC) stored in cookie storage
						//Ispresistant represent if token  will be exist after browser closed --> based on remember me
						//forth parameter enable Lokout
						if (result.Succeeded)
							return RedirectToAction(nameof(HomeController.Index), "Home");

						}
				}
				ModelState.AddModelError(string.Empty, "Invalid Login");
			}
			return View(model);
		}
		#endregion

		#region SignOut
		public async  new Task<IActionResult> SignOut()
		{
			//New bec Homecontroller inherit SignOut from Controller
			await _signInManager.SignOutAsync(); //remove  token from Cookies
			return RedirectToAction(nameof(SignIn));
		}
		#endregion

		#region Forget Password
		public IActionResult ForgetPassword()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> SendResetPasswordUrl(ForgetPasswordViewModel model)

		{
			if (ModelState.IsValid)
			{
				var user = await _userManager.FindByEmailAsync(model.Email);
				if(user is not null)
				{
					//create token to make this url unique for one user one time
					var token =  await _userManager.GeneratePasswordResetTokenAsync(user);
					var resetPasswordUrl = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token },Request.Scheme);
					//https://localhost:5001/Account/ResetPassword?email=sara@gmail.com&token=sdfgr3fghjkiuy
					var email = new Email
					{
						Subject = "Reset Your Password",
						Recipients = model.Email,
						Body = resetPasswordUrl
					};
					EmailSettings.SendEmail(email);
					return RedirectToAction(nameof(CheckYourInbox));
				}
				ModelState.AddModelError(string.Empty, "Invalid Email");

			}
			return View(model);
		}
		
		public IActionResult CheckYourInbox()
		{
			return View();
		}

		#endregion

		#region Reset Password
		public IActionResult ResetPassword(string email ,string token)
		{
			TempData["email"] = email;
			TempData["token"] = token;
			return View();
		}
		[HttpPost]
		public async Task<IActionResult> ResetPassword(ResetViewModel model)
		{
			if (ModelState.IsValid)
			{
				string email = TempData["email"] as string;
				string token = TempData["token"] as string;
				var user = await _userManager.FindByEmailAsync(email);
				var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
				if (result.Succeeded)
					return RedirectToAction(nameof(SignIn));
				foreach (var error in result.Errors)
					ModelState.AddModelError(string.Empty, error.Description);
			}
			return View(model);
		}
		#endregion

	}
}
