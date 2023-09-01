using ECommerceProjectITI.Validations;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ECommerceProjectITI.ViewModels;

public class UserEditViewModel : RegisterUserViewModel {
	[Display(Name = "Current Password")]
	[DataType(DataType.Password)]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Password Length should be between 6 and 30")]
	public string? CurrentPassword { get; set; }

	[Display(Name = "New Password")]
	[DataType(DataType.Password)]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Password Length should be between 6 and 30")]
	[Remote("RequireConfirmPassword", "Account", AdditionalFields = "ConfirmPassword,CurrentPassword", ErrorMessage = "Current Password and Confirm New Password Required.")]
	public new string? Password { get; set; }

	[DataType(DataType.Password)]
	[Display(Name = "Confirm New Password")]
	[Remote("ConfirmPassword", "Account", AdditionalFields = "Password", ErrorMessage = "Password does not match.")]
	public new string? ConfirmPassword { get; set; }

	

	[Remote("RepeatedUsername", "Account",AdditionalFields = "CurrentUsername", ErrorMessage = "Username is already taken.")]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Username Length should be between 6 and 30")]
	[RegularExpression("^[A-Za-z][A-Za-z0-9_.#!$]{0,}$",
	ErrorMessage = "Username must begin with a letter. Allowed special characters: _ . # ! $")]
	public new string Username { get; set; } = string.Empty;

	public bool IsCustomer { get; set; }
}
