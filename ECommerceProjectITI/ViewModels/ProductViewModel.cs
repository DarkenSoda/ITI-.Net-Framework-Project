using ECommerceProjectITI.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace ECommerceProjectITI.ViewModels;

[Keyless]
public class ProductViewModel {
	[Display(Name = "Product Name")]
	[MaxLength(32, ErrorMessage = "Name Length Limit Reached!")]
	public string Name { get; set; } = string.Empty;
	public string? Description { get; set; }

	[Range(10, 9999999, ErrorMessage = "Minimum Price 10 L.E., Maximum Price 9999999 L.E.")]
	[RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Price must be a valid decimal with up to two decimal places.")]
	public float Price { get; set; }

	[Display(Name = "Available Quantity")]
	[Range(0, 1000, ErrorMessage = "Available Quantity must be from 0 to 1000.")]
	[RegularExpression(@"^\d+$", ErrorMessage = "Available Quantity must be a Real Number.")]
	public int MaxQuantity { get; set; }

	[Required]
	[NotMapped]
	[DataType(DataType.ImageUrl)]
	public virtual IFormFile? Image { get; set; }
}
