namespace OnlineShop.BLL.Services.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName);
    Task DeleteFileAsync(string blobFileName);
    string GetImageSasUri(string blobFileName);
}
