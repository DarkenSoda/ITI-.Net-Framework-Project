using ECommerceProjectITI.Models;
using ECommerceProjectITI.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceProjectITI.Controllers;
public class AccountController : Controller {
	private readonly UserManager<User> userManager;
	private readonly IWebHostEnvironment webHostEnvironment;
	private readonly SignInManager<User> signInManager;
	private readonly ECommerceContext context;

	public AccountController(UserManager<User> _userManager, IWebHostEnvironment _webHostEnvironment, SignInManager<User> _signInManager, ECommerceContext _context) {
		userManager = _userManager;
		webHostEnvironment = _webHostEnvironment;
		signInManager = _signInManager;
		context = _context;
	}

	public IActionResult Register() {
		ViewBag.Roles = context.Roles.ToList();
		return CheckAuthentication();
	}

	[HttpPost]
	public async Task<IActionResult> Register(RegisterUserViewModel registerUser) {
		if (ModelState.IsValid) {
			User user = new User() {
				Name = registerUser.Name,
				UserName = registerUser.Username,
				Email = registerUser.Email,
				PhoneNumber = registerUser.Phonenumber,
			};

			if (registerUser.Image != null) {
				var uniqueFileName = ImageSaver.SaveImage(registerUser.Image, webHostEnvironment);

				if (uniqueFileName.Result != null) {
					user.Image = uniqueFileName.Result;
				} else {
					ModelState.AddModelError("Image", "Invalid image format. Allowed formats: png, jpg, jpeg.");

					ViewBag.Roles = context.Roles.ToList();
					return View(registerUser);
				}
			}

			IdentityResult result = await userManager.CreateAsync(user, registerUser.Password);
			if (result.Succeeded) {
				await userManager.AddToRoleAsync(user, registerUser.Role);


				if (registerUser.Role == "Seller") {
					context.Seller.Add(new Seller {
						UserID = user.Id
					});
				} else {
					context.Customer.Add(new Customer {
						UserID = user.Id,
						Address = registerUser.Address
					});

					await context.SaveChangesAsync();
				}

				await context.SaveChangesAsync();

				return RedirectToAction("Login");
			}

			foreach (var error in result.Errors) {
				ModelState.AddModelError("", error.Description);
			}
		}

		ViewBag.Roles = context.Roles.ToList();
		return View(registerUser);
	}

	public IActionResult Login() {
		return CheckAuthentication();
	}

	[HttpPost]
	public async Task<IActionResult> Login(LoginUserViewModel loginUser) {
		if (ModelState.IsValid) {
			User? user = await userManager.FindByNameAsync(loginUser.Username);
			if (user == null) {
				ModelState.AddModelError("", "Invalid Username or Password.");
				return View();
			}

			Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, loginUser.Password, false, false);

			if (result.Succeeded) {
				// Check Role
				bool isCustomer = await userManager.IsInRoleAsync(user, "Customer");

				if (isCustomer) {
					return RedirectToAction("Index", "Customer");
				} else {
					return RedirectToAction("Index", "Seller");
				}
			}

			ModelState.AddModelError("", "Invalid Username or Password.");
		}

		return View();
	}

	public async Task<IActionResult> Logout() {
		Response.Cookies.Delete("ShoppingCart");
		await signInManager.SignOutAsync();
		return RedirectToAction("Login");
	}

	public IActionResult AccessDenied() {
		return View();
	}

	public IActionResult ConfirmPassword(string confirmPassword, string password) {
		return Json(confirmPassword == password);
	}
	public IActionResult RequireConfirmPassword(string password, string confirmPassword, string currentPassword) {
		if((confirmPassword == null || currentPassword == null) && password != null) {
			return Json(false);
		}

		return Json(confirmPassword == password);
	}

	[AcceptVerbs("Get", "Post")]
	public IActionResult RepeatedUsername(string username) {
		if(username == User.Identity.Name) {
			return Json(true);
		}
		else {
			var found = context.Users.FirstOrDefault(user=>user.UserName == username);
			return Json(found == null);
		}
	}

	private IActionResult CheckAuthentication() {
		if (User.Identity != null && User.Identity.IsAuthenticated) {
			bool isCustomer = User.IsInRole("Customer");

			if (isCustomer) {
				return RedirectToAction("Index", "Customer");
			} else {
				return RedirectToAction("Index", "Seller");
			}
		}

		return View();
	}
}
