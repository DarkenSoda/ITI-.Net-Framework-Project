using ECommerceProjectITI.Validations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ECommerceProjectITI.ViewModels;

[Keyless]
public class LoginUserViewModel {
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Username Length should be between 6 and 30")]
	[RegularExpression("^[A-Za-z][A-Za-z0-9_.#!$]{0,}$",
		ErrorMessage = "Username must begin with a letter. Allowed special characters: _ . # ! $")]
	public string Username { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[StringLength(30, MinimumLength = 6, ErrorMessage = "Password Length should be between 6 and 30")]
	public string Password { get; set; } = string.Empty;
}
