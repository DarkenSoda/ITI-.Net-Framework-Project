using ECommerceProjectITI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ECommerceProjectITI.Validations;

public class UniqueUsernameAttribute : ValidationAttribute {
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) {
		if (value == null)
			return new ValidationResult("Invalid");

		var username = value.ToString();

		var context = (ECommerceContext?)validationContext.GetService(typeof(ECommerceContext));
		
		if (context == null) {
			// Handle the case where the context is not available
			return new ValidationResult("Context not available.");
		}

		var existingUser = context.Users.FirstOrDefault(user => user.UserName == username);

		if (existingUser != null) {
			return new ValidationResult("Username is already taken.");
		}

		return ValidationResult.Success;
	}
}