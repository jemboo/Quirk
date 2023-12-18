namespace Quirk.UI.W.Core.Contracts.Services;

public interface IFileService
{
    T Read<T>(string folderPath, string fileName);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);

    IEnumerable<string> GetFolders(string folderPath);
}

public interface IFileService2
{
    T Read<T>(string folderPath, string fileName);

    void Save<T>(string folderPath, string fileName, T content);

    void Delete(string folderPath, string fileName);

    IEnumerable<string> GetFolders(string folderPath);
}