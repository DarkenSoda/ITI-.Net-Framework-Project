using ECommerceProjectITI.Models;
using ECommerceProjectITI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ECommerceProjectITI.Controllers;

[Authorize(Roles = "Seller")]
public class SellerController : Controller {
	private readonly UserManager<User> userManager;
	private readonly ECommerceContext context;
	private readonly IWebHostEnvironment webHostEnvironment;
	private readonly SignInManager<User> signInManager;


	public SellerController(UserManager<User> _userManager, ECommerceContext _context, IWebHostEnvironment webHostEnvironment, SignInManager<User> signInManager) {
		userManager = _userManager;
		context = _context;
		this.webHostEnvironment = webHostEnvironment;
		this.signInManager = signInManager;
	}

	public IActionResult Index() {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);
		Seller? seller = context.Seller.FirstOrDefault(slr => slr.UserID == userId);

		if (seller == null)
			return View(new List<Product>());

		List<Product> productList = context.Product
			.Where(prod => prod.SellerID == seller.Id)
			.Include(prod => prod.Seller)
			.ToList();

		return View(productList);
	}

	public IActionResult Profile() {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);
		Seller? seller = context.Seller.Include(cust => cust.User).FirstOrDefault(slr => slr.UserID == userId);

		return View(seller);
	}

	public IActionResult EditProfile() {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);
		Seller? seller = context.Seller.Include(cust => cust.User).FirstOrDefault(slr => slr.UserID == userId);
		UserEditViewModel userEdit = new UserEditViewModel() {
			Name = seller.User.Name,
			Email = seller.User.Email,
			Username = seller.User.UserName,
			Phonenumber = seller.User.PhoneNumber,
			Role = "Seller",
			IsCustomer = false,
		};

		ViewBag.UserImage = seller.User.Image;
		return View(userEdit);
	}

	[HttpPost]
	public async Task<IActionResult> EditProfile(UserEditViewModel userEdit) {
		if (ModelState.IsValid) {
			var user = await userManager.GetUserAsync(User);

			if (user != null) {
				user.UserName = userEdit.Username;
				user.Email = userEdit.Email;
				user.PhoneNumber = userEdit.Phonenumber;
				user.Name = userEdit.Name;

				if (!string.IsNullOrEmpty(userEdit.Password)) {
					var result = await userManager.ChangePasswordAsync(user, userEdit.CurrentPassword, userEdit.Password);
					if (!result.Succeeded) {
						foreach (var error in result.Errors) {
							ModelState.AddModelError("", error.Description);
						}

						ViewBag.UserImage = user.Image;
						return View(userEdit);
					}
				}

				string? uniqueName = await ImageSaver.SaveImage(userEdit.Image, webHostEnvironment);
				if (uniqueName != null) {
					user.Image = uniqueName;
				}

				await userManager.UpdateAsync(user);
				await signInManager.SignOutAsync();
				await signInManager.SignInAsync(user, false);
				await context.SaveChangesAsync();
			}

			return RedirectToAction("Profile");
		} else {
			var user = await userManager.GetUserAsync(User);
			ViewBag.UserImage = user?.Image;
			return View(userEdit);
		}
	}
	
	public IActionResult CreateProduct() {
		return View();
	}

	[HttpPost]
	public IActionResult CreateProduct(ProductViewModel productViewModel) {
		if (ModelState.IsValid) {
			Product product = new Product() {
				Name = productViewModel.Name,
				Description = productViewModel.Description,
				Price = productViewModel.Price,
				MaxQuantity = productViewModel.MaxQuantity,
			};

			var uniqueName = ImageSaver.SaveImage(productViewModel.Image, webHostEnvironment);

			if (uniqueName.Result != null) {
				product.Image = uniqueName.Result;
			} else {
				ModelState.AddModelError("Image", "Invalid image format. Allowed formats: png, jpg, jpeg.");
				return View(productViewModel);
			}

			var userIdString = userManager.GetUserId(User);

			_ = int.TryParse(userIdString, out int userId);
			int sellerId = context.Seller.Where(slr => slr.UserID == userId).Select(slr => slr.Id).FirstOrDefault();

			product.SellerID = sellerId;

			context.Product.Add(product);
			context.SaveChanges();

			return RedirectToAction(nameof(Index));
		}

		return View(productViewModel);
	}

	public IActionResult Details(int Id) {
		Product? product = context.Product.Find(Id);

		if (product == null)
			return RedirectToAction("Index");

		IActionResult? authorizationResult = CheckSellerAuthorization(product.SellerID);
		if (authorizationResult != null) {
			return authorizationResult;
		}

		return View(product);
	}

	public IActionResult Edit(int Id) {
		Product? product = context.Product.Find(Id);

		if (product == null)
			return RedirectToAction("Index");

		IActionResult? authorizationResult = CheckSellerAuthorization(product.SellerID);
		if (authorizationResult != null) {
			return authorizationResult;
		}

		ProductEditViewModel productEditViewModel = new ProductEditViewModel() {
			Name = product.Name,
			Description = product.Description,
			Price = product.Price,
			MaxQuantity = product.MaxQuantity,
		};

		ViewBag.ProductId = product.Id;
		ViewBag.ProductImage = product.Image;
		return View(productEditViewModel);
	}

	[HttpPost]
	public IActionResult Edit(ProductEditViewModel productEdit, int Id) {
		Product? product = context.Product.Find(Id);

		if (product == null)
			return RedirectToAction("Index");

		if (productEdit.Image != null) {
			var uniqueFileName = ImageSaver.SaveImage(productEdit.Image, webHostEnvironment);

			if (uniqueFileName.Result != null) {
				product.Image = uniqueFileName.Result;
			} else {
				ModelState.AddModelError("Image", "Invalid image format. Allowed formats: png, jpg, jpeg.");

				ViewBag.ProductId = product.Id;
				ViewBag.ProductImage = product.Image;
				return View(productEdit);
			}
		}

		product.Name = productEdit.Name;
		product.Description = productEdit.Description;
		product.Price = productEdit.Price;
		product.MaxQuantity = productEdit.MaxQuantity;

		context.SaveChanges();
		return RedirectToAction("Details", new { Id = Id });
	}

	[HttpPost]
	public IActionResult Delete(int Id) {
		Product? product = context.Product.Find(Id);

		if (product != null) {
			context.Product.Remove(product);
			context.SaveChanges();
		}

		return RedirectToAction("Index");
	}

	private IActionResult? CheckSellerAuthorization(int sellerID) {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);

		Seller? seller = context.Seller.FirstOrDefault(s => s.UserID == userId);

		if (seller == null || sellerID != seller.Id) {
			return RedirectToAction("AccessDenied", "Account");
		}

		return null;
	}
}
