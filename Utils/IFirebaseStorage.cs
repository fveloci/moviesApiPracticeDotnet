
namespace APIPeliculas.Utils
{
    public interface IFirebaseStorage
    {
        Task DeleteFile(string route, string folderName);
        Task<string> EditFile(IFormFile file, string route, string folderName);
        Task<string> SaveFile(IFormFile file, string folderName);
    }
}