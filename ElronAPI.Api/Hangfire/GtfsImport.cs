using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ElronAPI.Api.Hangfire
{
    public class GtfsImport : IJob
    {
        private readonly HttpClient _client;
        private readonly string _tempFolderPath;

        public GtfsImport(IHttpClientFactory factory, IConfiguration configuration)
        {
            _client = factory.CreateClient();
            _tempFolderPath = configuration.GetValue<string>("GtfsImportPath");
        }

        public async Task WorkAsync()
        {
            if (string.IsNullOrWhiteSpace(_tempFolderPath)) return;
            
            var tempFolder = Directory.CreateDirectory(_tempFolderPath);
            if (!tempFolder.Exists) return;

            string zipPath = Path.Combine(tempFolder.FullName, "gtfs.zip");
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
            
            string extractPath = Path.Combine(tempFolder.FullName, "gtfs");
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            
            var fileBytes = await _client.GetByteArrayAsync("http://peatus.ee/gtfs/gtfs.zip");
            File.WriteAllBytes(zipPath, fileBytes);
            
            ZipFile.ExtractToDirectory(zipPath, extractPath, true);
        }        
    }
}