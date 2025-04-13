using System;
using System.IO;

namespace MemoryGame.Services
{
    public class FileService
    {
        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateDirectory(string path)
        {
            if (!DirectoryExists(path))
                Directory.CreateDirectory(path);
        }

        public string[] GetFiles(string path, string searchPattern)
        {
            if (!DirectoryExists(path))
                return new string[0];

            return Directory.GetFiles(path, searchPattern);
        }

        public void WriteTextToFile(string path, string content)
        {
            try
            {
                File.WriteAllText(path, content);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string ReadTextFromFile(string path)
        {
            try
            {
                return File.ReadAllText(path);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void DeleteFile(string path)
        {
            if (FileExists(path))
                File.Delete(path);
        }

        public void CopyFile(string sourcePath, string destinationPath, bool overwrite = false)
        {
            File.Copy(sourcePath, destinationPath, overwrite);
        }
    }
}