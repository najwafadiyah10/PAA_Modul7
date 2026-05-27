using Microsoft.EntityFrameworkCore;
using PAA_Modul7.Data;
using PAA_Modul7.Models;
using System.Text.Json;

namespace PAA_Modul7.Services
{
    public class SyncService
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public SyncService(HttpClient httpClient, AppDbContext context, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _context = context;
            _configuration = configuration;
        }

        public async Task<int> SyncMahasiswaAsync()
        {
            var url = _configuration["ExternalApi:MahasiswaApiUrl"];

            if (string.IsNullOrWhiteSpace(url))
            {
                throw new Exception("URL API Mahasiswa belum diatur di appsettings.json.");
            }

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var document = JsonDocument.Parse(json);

            var root = document.RootElement;

            if (root.ValueKind != JsonValueKind.Object ||
                !root.TryGetProperty("data", out var dataElement) ||
                dataElement.ValueKind != JsonValueKind.Array)
            {
                throw new Exception("Format response API Mahasiswa tidak sesuai. Format yang diharapkan: { success, count, data: [...] }.");
            }

            var totalData = 0;

            foreach (var item in dataElement.EnumerateArray())
            {
                var mahasiswa = MapMahasiswa(item);

                if (string.IsNullOrWhiteSpace(mahasiswa.Id))
                {
                    continue;
                }

                var existing = await _context.Mahasiswas
                    .FirstOrDefaultAsync(x => x.Id == mahasiswa.Id);

                if (existing == null)
                {
                    _context.Mahasiswas.Add(mahasiswa);
                }
                else
                {
                    existing.Nama = mahasiswa.Nama;
                    existing.ProgramStudi = mahasiswa.ProgramStudi;
                    existing.MataKuliah = mahasiswa.MataKuliah;
                    existing.StatusAkademik = mahasiswa.StatusAkademik;
                    existing.Version = mahasiswa.Version;
                    existing.RawDataJson = mahasiswa.RawDataJson;
                    existing.CreatedAt = mahasiswa.CreatedAt;
                    existing.SyncedAt = DateTime.UtcNow;
                }

                totalData++;
            }

            await _context.SaveChangesAsync();
            return totalData;
        }

        private static MahasiswaEntity MapMahasiswa(JsonElement item)
        {
            var id = GetStringValue(item, "_id");
            var nama = GetStringValue(item, "nama");
            var programStudi = GetStringValue(item, "programStudi");
            var mataKuliah = GetStringArray(item, "mataKuliah");
            var statusAkademik = GetStringValue(item, "statusAkademik");
            var version = GetIntValue(item, "__v");
            var createdAt = GetDateTimeValue(item, "createdAt") ?? DateTime.UtcNow;

            return new MahasiswaEntity
            {
                Id = id,
                Nama = nama,
                ProgramStudi = programStudi,
                MataKuliah = mataKuliah,
                StatusAkademik = statusAkademik,
                Version = version,
                RawDataJson = item.GetRawText(),
                CreatedAt = createdAt,
                SyncedAt = DateTime.UtcNow
            };
        }

        private static string GetStringValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
            {
                return string.Empty;
            }

            return property.ValueKind switch
            {
                JsonValueKind.String => property.GetString() ?? string.Empty,
                JsonValueKind.Number => property.GetRawText(),
                JsonValueKind.True => "true",
                JsonValueKind.False => "false",
                _ => string.Empty
            };
        }

        private static string[] GetStringArray(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<string>();
            }

            return property
                .EnumerateArray()
                .Where(x => x.ValueKind == JsonValueKind.String)
                .Select(x => x.GetString() ?? string.Empty)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToArray();
        }

        private static int GetIntValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property))
            {
                return 0;
            }

            if (property.ValueKind == JsonValueKind.Number && property.TryGetInt32(out var value))
            {
                return value;
            }

            return 0;
        }

        private static DateTime? GetDateTimeValue(JsonElement element, string propertyName)
        {
            if (!element.TryGetProperty(propertyName, out var property) ||
                property.ValueKind != JsonValueKind.String)
            {
                return null;
            }

            if (DateTime.TryParse(property.GetString(), out var dateTime))
            {
                return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
            }

            return null;
        }
    }
}
