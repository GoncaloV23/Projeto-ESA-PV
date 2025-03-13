using EasyFitHub.Models.Miscalenous;
using Firebase.Storage;
using Microsoft.Extensions.Logging;
using System.Drawing;

namespace EasyFitHub.Services
{
    /// <summary>
    /// AUTHOR: Rui Barroso
    /// Usado para gerir imagens no Firebase
    /// </summary>
    public class FirebaseService
    {
        private readonly FirebaseStorage _storage;

        public FirebaseService(IConfiguration configuration)
        {
            var apiKey = configuration["Firebase:ApiKey"];

            _storage = new FirebaseStorage(configuration["Firebase:StorageUrl"]);
        }/// <summary>
        /// adiciona uma imagem no service Firebase
        /// </summary>
        /// <param name="fileStream">A imagem</param>
        /// <param name="fileName">O nome da imagem</param>
        /// <param name="logger">Logger pra debugging</param>
        /// <returns>O URL da imagem</returns>
        private async Task<string?> UploadFileAsync(Stream fileStream, string fileName, ILogger? logger = null)
        {
            try {
                var fileUrl = await _storage.Child("images").Child(fileName).PutAsync(fileStream);

                return fileUrl;
            }catch (Exception ex)
            {
                if (logger == null) Console.WriteLine($"Error Uploading File in Firebase Storage: {ex.Message}");
                else logger.LogError($"Error Uploading File in Firebase Storage: {ex.Message}");
                return null;
            }
        }/// <summary>
        /// Remove uma imagem no serviço Firebase
        /// </summary>
        /// <param name="fileName">o Nome da imagem</param>
        /// <param name="logger">Logger pra debugging</param>
        /// <returns>true ou false dependendo do nível de sucesso</returns>
        private async Task<bool> DeleteFileAsync(string fileName, ILogger? logger = null)
        {
            try
            {
                await _storage.Child("images").Child(fileName).DeleteAsync();

                return true;
            }
            catch (Exception ex)
            {
                if(logger == null)Console.WriteLine($"Error Deleting File in Firebase Storage: {ex.Message}");
                else logger.LogError($"Error Deleting File in Firebase Storage: {ex.Message}");
                return false;
            }
        }
        /// <summary>
        /// Adiciona uma HubImage ao serviço Firebase
        /// </summary>
        /// <param name="hubImage">A HubImage</param>
        /// <param name="fileStream">A imagem</param>
        /// <param name="logger">Usado pra debugging</param>
        /// <returns>A hubImage </returns>
        public async Task<HubImage> CreateHubImage(HubImage hubImage, Stream fileStream, ILogger? logger = null) 
        {
            hubImage.Path = await UploadFileAsync(fileStream, hubImage.Name, logger);

            return hubImage;
        }
        /// <summary>
        /// Remove uma HubImage no serviço Firebase
        /// </summary>
        /// <param name="hubImage">A HubImage</param>
        /// <param name="logger">Logger pra Debugging</param>
        /// <returns>true ou False</returns>
        public async Task<bool> DeleteHubImage(HubImage hubImage, ILogger? logger = null)
        {
            hubImage.Path = "";
            var name = hubImage.Name;

            return await DeleteFileAsync(name, logger);
        }
    }
}
