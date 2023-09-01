using ECommerceProjectITI.Models;
using ECommerceProjectITI.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ECommerceProjectITI.Controllers;

[Authorize(Roles = "Customer")]
public class CustomerController : Controller {
	private readonly ECommerceContext context;
	private readonly UserManager<User> userManager;
	private readonly SignInManager<User> signInManager;
	private readonly IWebHostEnvironment webHostEnvironment;

	public CustomerController(ECommerceContext _context, UserManager<User> _userManager, IWebHostEnvironment _webHostEnvironment, SignInManager<User> _signInManager) {
		context = _context;
		userManager = _userManager;
		signInManager = _signInManager;
		webHostEnvironment = _webHostEnvironment;
	}

	public IActionResult Index() {
		var productList = context.Product.Include(prod => prod.Seller).Include(prod => prod.Seller.User).ToList();

		List<CartItem>? cartItems = GetCookieShoppingCart();
		ViewBag.CartCount = cartItems?.Count;
		return View(productList);
	}

	public IActionResult Details(int Id) {
		var product = context.Product.Where(prod => prod.Id == Id).Include(prod => prod.Seller).Include(prod => prod.Seller.User).First();

		List<CartItem>? cartItems = GetCookieShoppingCart();
		ViewBag.CartCount = cartItems?.Count;
		ViewBag.TotalPrice = GetTotalPrice(cartItems);
		return View(product);
	}

	public IActionResult Profile() {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);
		Customer? customer = context.Customer.Include(cust => cust.User).FirstOrDefault(slr => slr.UserID == userId);

		List<CartItem>? cartItems = GetCookieShoppingCart();
		ViewBag.CartCount = cartItems?.Count;
		return View(customer);
	}

	public IActionResult EditProfile() {
		var userIdString = userManager.GetUserId(User);

		_ = int.TryParse(userIdString, out int userId);
		Customer? customer = context.Customer.Include(cust => cust.User).FirstOrDefault(slr => slr.UserID == userId);
		UserEditViewModel userEdit = new UserEditViewModel() {
			Name = customer.User.Name,
			Email = customer.User.Email,
			Username = customer.User.UserName,
			Phonenumber = customer.User.PhoneNumber,
			Address = customer.Address,
			Role = "Customer",
			IsCustomer = true,
		};

		List<CartItem>? cartItems = GetCookieShoppingCart();
		ViewBag.CartCount = cartItems?.Count;
		ViewBag.UserImage = customer.User.Image;
		return View(userEdit);
	}

	[HttpPost]
	public async Task<IActionResult> EditProfile(UserEditViewModel userEdit) {
		if (ModelState.IsValid) {
			var userIdString = userManager.GetUserId(User);

			_ = int.TryParse(userIdString, out int userId);
			Customer? customer = context.Customer.FirstOrDefault(slr => slr.UserID == userId);

			customer.Address = userEdit.Address;

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

						List<CartItem>? cartItems = GetCookieShoppingCart();
						ViewBag.CartCount = cartItems?.Count;
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
			List<CartItem>? cartItems = GetCookieShoppingCart();
			ViewBag.CartCount = cartItems?.Count;
			ViewBag.UserImage = user?.Image;
			return View(userEdit);
		}
	}

	[HttpPost]
	public IActionResult AddToCart(int Id) {
		var product = context.Product.Find(Id);

		List<CartItem>? cartItems = GetCookieShoppingCart();

		var existingItem = cartItems?.FirstOrDefault(item => item.Product?.Id == Id);
		if (existingItem == null) {
			cartItems?.Add(new CartItem { Product = product, Quantity = 1 });
		}

		SaveCookieShoppingCart(cartItems);

		return Json(new {
			success = true,
			cartCount = cartItems?.Count
		});
	}

	[HttpPost]
	public IActionResult UpdateCartItemQuantity(int Id, int value) {
		List<CartItem>? cartItems = GetCookieShoppingCart();

		var existingItem = cartItems?.FirstOrDefault(item => item.Product?.Id == Id);
		if (existingItem != null) {
			if (existingItem.Quantity + value <= 0 || existingItem.Quantity + value > existingItem.Product?.MaxQuantity) {
				return Json(new { success = false });
			}

			existingItem.Quantity += value;
		}

		SaveCookieShoppingCart(cartItems);

		return Json(new {
			success = true,
			quantity = existingItem?.Quantity,
			price = existingItem?.Quantity * existingItem?.Product?.Price,
			totalPrice = GetTotalPrice(cartItems),
		});
	}

	[HttpPost]
	public IActionResult DeleteCartItem(int Id) {
		List<CartItem>? cartItems = GetCookieShoppingCart();

		var itemToRemove = cartItems?.FirstOrDefault(item => item.Product?.Id == Id);
		if (itemToRemove != null) {
			cartItems?.Remove(itemToRemove);
			SaveCookieShoppingCart(cartItems);


			return Json(new {
				success = true,
				totalPrice = GetTotalPrice(cartItems),
				cartCount = cartItems?.Count,
			});
		} else {
			return Json(new {
				success = false,
			});
		}
	}

	public IActionResult ShoppingCart() {
		List<CartItem>? cartItems = GetCookieShoppingCart();

		ViewBag.CartCount = cartItems?.Count;
		ViewBag.TotalPrice = GetTotalPrice(cartItems);
		return View(cartItems);
	}

	public IActionResult CheckOut() {
		List<CartItem>? cartItems = GetCookieShoppingCart();

		for (int i = 0; i < cartItems.Count; i++) {
			Product product = context.Product.Find(cartItems[i].Product.Id);
			product.MaxQuantity -= cartItems[i].Quantity;
		}

		context.SaveChanges();

		Response.Cookies.Delete("ShoppingCart");
		return RedirectToAction("ShoppingCart");
	}

	private List<CartItem>? GetCookieShoppingCart() {
		List<CartItem>? cartItems = new List<CartItem>();

		var existingCart = Request.Cookies["ShoppingCart"];

		if (!string.IsNullOrEmpty(existingCart)) {
			cartItems = JsonConvert.DeserializeObject<List<CartItem>>(existingCart);
		}

		return cartItems;
	}

	private void SaveCookieShoppingCart(List<CartItem>? cartItems) {
		var serializedCart = JsonConvert.SerializeObject(cartItems);
		Response.Cookies.Append("ShoppingCart", serializedCart);
	}

	private float GetTotalPrice(List<CartItem>? cartItems) {
		float totalPrice = 0;
		if (cartItems != null) {
			foreach (var item in cartItems) {
				totalPrice += item.Product.Price * item.Quantity;
			}
		}
		return totalPrice;
	}
}
