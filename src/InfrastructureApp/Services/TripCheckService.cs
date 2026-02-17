using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using InfrastructureApp.ViewModels;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace InfrastructureApp.Services
{
    public class TripCheckService : ITripCheckService
    {
        // dependencies injected by ASP.NET
        private readonly HttpClient _http;
        private readonly IMemoryCache _cache;
        private readonly ILogger<TripCheckService> _logger;
        private readonly TripCheckOptions _options;

        // unique cache key per service instance (important for tests)
        private readonly string _cameraCacheKey;

        public TripCheckService(
            HttpClient http,
            IMemoryCache cache,
            ILogger<TripCheckService> logger,
            TripCheckOptions options)
        {
            _http = http;
            _cache = cache;
            _logger = logger;
            _options = options;

            // Each instance gets its own cache slot
            _cameraCacheKey = $"TripCheck:Cameras:{Guid.NewGuid()}";
        }

        public async Task<IReadOnlyList<RoadCameraViewModel>> GetCamerasAsync()
        {
            // 1) Try cache first
            if (_cache.TryGetValue(_cameraCacheKey, out List<RoadCameraViewModel>? cached))
                return cached!;

            try
            {
                // Call TripCheck API (relative URL so tests can mock it)
                var response = await _http.GetAsync("Roadway/api/v1/cameras");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("TripCheck returned {Status}", response.StatusCode);
                    return new List<RoadCameraViewModel>();
                }

                var json = await response.Content.ReadAsStringAsync();

                // TripCheck returns a JSON ARRAY
                var cameras = JsonSerializer.Deserialize<List<TripCheckCameraDto>>(json,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                var mapped = cameras?
                    .Select(MapCamera)
                    .ToList()
                    ?? new List<RoadCameraViewModel>();

                // store in cache
                _cache.Set(_cameraCacheKey, mapped, TimeSpan.FromMinutes(_options.CacheMinutes));

                return mapped;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TripCheck camera fetch failed.");
                return new List<RoadCameraViewModel>();
            }
        }

        public async Task<RoadCameraViewModel?> GetCameraByIdAsync(string id)
        {
            var cameras = await GetCamerasAsync();
            return cameras.FirstOrDefault(c => c.CameraId == id);
        }

        private static RoadCameraViewModel MapCamera(TripCheckCameraDto dto)
        {
            DateTimeOffset? parsedDate = null;

            if (!string.IsNullOrWhiteSpace(dto.LastUpdated)
                && DateTimeOffset.TryParse(dto.LastUpdated, out var dt))
            {
                parsedDate = dt;
            }

            return new RoadCameraViewModel
            {
                CameraId = dto.Id ?? "",
                Name = dto.Name,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                ImageUrl = dto.ImageUrl,
                LastUpdated = parsedDate
            };
        }

        /// <summary>
        /// DTO that matches TripCheck JSON
        /// </summary>
        private class TripCheckCameraDto
        {
            public string? Id { get; set; }
            public string? Name { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? ImageUrl { get; set; }
            public string? LastUpdated { get; set; }
        }
    }
}
