using DeveloperAssessment.DAL.Contracts;
using Microsoft.Extensions.Hosting;
using System.Text.Json; // choice of deserialiser

namespace DeveloperAssessment.DAL
{
    public class GenericRepository<T> : IRepository<T> where T : class, new()
    {
        private readonly IHostEnvironment _env;
        private readonly string _relativeFilePath;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        public GenericRepository(IHostEnvironment env, string relativeFilePath)
        {
            _env = env;
            _relativeFilePath = relativeFilePath;
        }

        private string ResolvePath() => Path.Combine(_env.ContentRootPath, _relativeFilePath);

        public async Task<T> GetAsync()
        {
            var path = ResolvePath();

            if (!File.Exists(path)) {
                return new T();
            }

            var json = await File.ReadAllTextAsync(path);

            if (string.IsNullOrWhiteSpace(json)) {
                return new T();
            }

            return JsonSerializer.Deserialize<T>(json, JsonOptions) ?? new T();
        }

        public async Task SaveAsync(T entity)
        {
            var path = ResolvePath();

            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var json = JsonSerializer.Serialize(entity, JsonOptions);
            await File.WriteAllTextAsync(path, json);
        }
    }
}
