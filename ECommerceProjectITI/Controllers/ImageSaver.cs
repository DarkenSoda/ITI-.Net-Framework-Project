using Microsoft.AspNetCore.Hosting;

namespace ECommerceProjectITI.Controllers;

public static class ImageSaver {
	public async static Task<string?> SaveImage(IFormFile? Image, IWebHostEnvironment webHostEnvironment) {
		if (Image != null && Image.Length > 0) {
			if (IsImageFileValid(Image)) {
				string uniqueFileName = GetUniqueFileName(Image.FileName);
				string imagePath = Path.Combine(webHostEnvironment.WebRootPath, "Images", uniqueFileName);

				using (var stream = new FileStream(imagePath, FileMode.Create)) {
					await Image.CopyToAsync(stream);
				}

				return uniqueFileName;
			}
		}
		return null;
	}

	private static string GetUniqueFileName(string fileName) {
		fileName = Path.GetFileName(fileName);
		return Path.GetFileNameWithoutExtension(fileName)
			+ "_" + Guid.NewGuid().ToString().Substring(0, 4)
			+ Path.GetExtension(fileName);
	}

	private static bool IsImageFileValid(IFormFile file) {
		string[] allowedExtensions = { ".png", ".jpg", ".jpeg" };
		string fileExtension = Path.GetExtension(file.FileName).ToLower();
		return allowedExtensions.Contains(fileExtension);
	}
}
