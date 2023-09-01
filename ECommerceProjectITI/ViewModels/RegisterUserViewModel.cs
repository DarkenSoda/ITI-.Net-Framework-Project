using ECommerceProjectITI.Validations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerceProjectITI.ViewModels;

[Keyless]
public class RegisterUserViewModel {
	[UniqueUsername(ErrorMessage = "Username is already taken.")]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Username Length should be between 6 and 30")]
	[RegularExpression("^[A-Za-z][A-Za-z0-9_.#!$]{0,}$",
		ErrorMessage = "Username must begin with a letter. Allowed special characters: _ . # ! $")]
	public string Username { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Password Length should be between 6 and 30")]
	public string Password { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[Display(Name = "Confirm Password")]
	[Remote("ConfirmPassword", "Account", AdditionalFields = "Password", ErrorMessage = "Password does not match.")]
	public string ConfirmPassword { get; set; } = string.Empty;

	public string Name { get; set; } = string.Empty;

	[Display(Name ="Email Address")]
	[RegularExpression("^[\\w-\\.]+@([\\w]+\\.)+[\\w]{2,3}$", ErrorMessage = "Invalid Email Address")]
	public string? Email { get; set; }

	[Display(Name ="Phone Number")]
	public string? Phonenumber { get; set; }
	public string? Address{ get; set; }

	[NotMapped]
	[DataType(DataType.ImageUrl)]
	public IFormFile? Image { get; set; }

	public string Role { get; set; } = string.Empty;
}
