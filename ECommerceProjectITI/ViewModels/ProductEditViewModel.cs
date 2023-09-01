using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace ECommerceProjectITI.ViewModels;

public class ProductEditViewModel : ProductViewModel {
	[NotMapped]
	[DataType(DataType.ImageUrl)]
	public new IFormFile? Image { get; set; }
}
