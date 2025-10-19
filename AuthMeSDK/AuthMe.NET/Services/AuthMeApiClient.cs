using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using AuthMe.NET.Models;

namespace AuthMe.NET.Services
{
    /// <summary>
    /// Client for communicating with AuthMe API using app-based authentication
    /// </summary>
    internal class AuthMeApiClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly AuthMeConfig _config;
        private bool _disposed;
        
        // AuthMe API endpoints
        private readonly string _apiBaseUrl;
        private const string VALIDATE_ENDPOINT = "validate";
        private const string HEALTH_ENDPOINT = "/health";
        private const string ANALYTICS_ENDPOINT = "/analytics/validation-stats";
        private const string SECURITY_LOG_ENDPOINT = "/security/log-violation";

        public AuthMeApiClient(AuthMeConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _apiBaseUrl = _config.ApiUrl?.TrimEnd('/') ?? throw new ArgumentException("ApiUrl is required", nameof(config));

            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(_config.TimeoutSeconds)
            };

            // Set AuthMe API headers
            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"AuthMe.NET/2.0.0");
        }

        /// <summary>
        /// Validates a license key with hardware ID binding
        /// </summary>
        public async Task<ValidationResult> ValidateLicenseAsync(string licenseKey, string hardwareId)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
                throw new ArgumentException("License key cannot be null or empty", nameof(licenseKey));
            
            if (string.IsNullOrWhiteSpace(hardwareId))
                throw new ArgumentException("Hardware ID cannot be null or empty", nameof(hardwareId));

            var attempt = 0;
            var maxAttempts = _config.EnableRetry ? _config.MaxRetryAttempts : 1;

            while (attempt < maxAttempts)
            {
                try
                {
                    var requestData = new
                    {
                        app_id = _config.AppId,
                        app_secret = _config.AppSecret,
                        app_name = _config.AppName,
                        license_key = licenseKey,
                        hardware_id = hardwareId
                    };

                    var json = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    
                    var fullUrl = $"{_apiBaseUrl}/{VALIDATE_ENDPOINT}";
                    var response = await _httpClient.PostAsync(fullUrl, content);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode)
                    {
                        var apiResponse = JsonConvert.DeserializeObject<SimpleApiResponse>(responseContent);
                        
                        return new ValidationResult
                        {
                            IsValid = apiResponse?.Valid == true,
                            Message = apiResponse?.Message ?? "Unknown response",
                            ErrorCode = string.Empty,
                            RequestId = string.Empty,
                            KeyData = apiResponse?.KeyData != null ? new LicenseKeyData
                            {
                                Tier = apiResponse.KeyData.Tier ?? "unknown",
                                ExpiresAt = apiResponse.KeyData.ExpiresAt,
                                UsageCount = apiResponse.KeyData.CurrentUses,
                                MaxUses = apiResponse.KeyData.MaxUses,
                                IsRevoked = false,
                                IsSuspended = false,
                                BoundHardwareId = hardwareId,
                                ActivatedAt = null,
                                LastValidatedAt = DateTime.UtcNow,
                                Metadata = new Dictionary<string, object>()
                            } : null,
                            IsCached = false,
                            Timestamp = DateTime.UtcNow
                        };
                    }
                    else
                    {
                        var errorResponse = JsonConvert.DeserializeObject<ApiErrorResponse>(responseContent);
                        return new ValidationResult
                        {
                            IsValid = false,
                            Message = errorResponse?.Error ?? $"HTTP {response.StatusCode}: {response.ReasonPhrase}",
                            ErrorCode = errorResponse?.ErrorCode ?? response.StatusCode.ToString(),
                            RequestId = errorResponse?.RequestId ?? string.Empty,
                            IsCached = false,
                            Timestamp = DateTime.UtcNow
                        };
                    }
                }
                catch (HttpRequestException ex)
                {
                    attempt++;
                    if (attempt >= maxAttempts)
                    {
                        return new ValidationResult
                        {
                            IsValid = false,
                            Message = $"Network error after {maxAttempts} attempts: {ex.Message}",
                            ErrorCode = "NETWORK_ERROR",
                            IsCached = false,
                            Timestamp = DateTime.UtcNow
                        };
                    }
                    
                    // Exponential backoff for retries
                    await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, attempt)));
                }
                catch (TaskCanceledException)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = "Request timeout - please check your internet connection",
                        ErrorCode = "TIMEOUT",
                        IsCached = false,
                        Timestamp = DateTime.UtcNow
                    };
                }
                catch (Exception ex)
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        Message = $"Validation error: {ex.Message}",
                        ErrorCode = "UNKNOWN_ERROR",
                        IsCached = false,
                        Timestamp = DateTime.UtcNow
                    };
                }
            }

            return new ValidationResult
            {
                IsValid = false,
                Message = "Maximum retry attempts exceeded",
                ErrorCode = "MAX_RETRIES_EXCEEDED",
                IsCached = false,
                Timestamp = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Tests connection to AuthMe API
        /// </summary>
        public async Task<(bool Success, string Message)> TestConnectionAsync()
        {
            try
            {
                // Test connection by making a simple POST to validate endpoint with invalid data
                // This will return an error but confirms the endpoint is reachable
                var testData = new
                {
                    app_id = "test",
                    app_secret = "test", 
                    license_key = "test",
                    hardware_id = "test"
                };
                
                var json = JsonConvert.SerializeObject(testData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var fullUrl = $"{_apiBaseUrl}/{VALIDATE_ENDPOINT}";
                var response = await _httpClient.PostAsync(fullUrl, content);
                var responseContent = await response.Content.ReadAsStringAsync();

                // If we get any response (even an error), the API is reachable
                if (response.StatusCode == System.Net.HttpStatusCode.OK || 
                    response.StatusCode == System.Net.HttpStatusCode.BadRequest ||
                    response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (true, "Connection successful. AuthMe API is reachable.");
                }
                else
                {
                    return (false, $"API returned unexpected status {response.StatusCode}. Response: {responseContent}");
                }
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Network error: {ex.Message}. Check if your AuthMe service is running on {_apiBaseUrl}");
            }
            catch (TaskCanceledException)
            {
                return (false, $"Request timeout. Check if your AuthMe service is running on {_apiBaseUrl}");
            }
            catch (Exception ex)
            {
                return (false, $"Connection error: {ex.Message}");
            }
        }

        /// <summary>
        /// Gets analytics data for the application
        /// </summary>
        public async Task<AnalyticsData> GetAnalyticsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_apiBaseUrl}{ANALYTICS_ENDPOINT}?period=30d");
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var analyticsResponse = JsonConvert.DeserializeObject<ApiAnalyticsResponse>(responseContent);
                    
                    return new AnalyticsData
                    {
                        TodayValidations = analyticsResponse?.Data?.TodayValidations ?? 0,
                        WeekValidations = analyticsResponse?.Data?.WeekValidations ?? 0,
                        MonthValidations = analyticsResponse?.Data?.MonthValidations ?? 0,
                        SuccessRate = analyticsResponse?.Data?.SuccessRate ?? 0,
                        ActiveLicenses = analyticsResponse?.Data?.ActiveLicenses ?? 0,
                        RevokedLicenses = analyticsResponse?.Data?.RevokedLicenses ?? 0,
                        ErrorBreakdown = analyticsResponse?.Data?.ErrorBreakdown ?? new Dictionary<string, int>(),
                        DailyStats = analyticsResponse?.Data?.DailyStats ?? new List<DailyStats>()
                    };
                }
                else
                {
                    throw new InvalidOperationException($"Failed to retrieve analytics: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Analytics request failed: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Logs a security violation
        /// </summary>
        public async Task LogSecurityViolationAsync(string errorCode, string message, string hardwareId)
        {
            try
            {
                var requestData = new
                {
                    error_code = errorCode,
                    message = message,
                    hardware_id = hardwareId,
                    timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                await _httpClient.PostAsync($"{_apiBaseUrl}{SECURITY_LOG_ENDPOINT}", content);
                // Fire and forget - don't throw on failure
            }
            catch
            {
                // Silently ignore logging failures to not disrupt main application flow
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }

    // Simple response classes for API parsing
    internal class SimpleApiResponse
    {
        [JsonProperty("valid")]
        public bool Valid { get; set; }
        
        [JsonProperty("message")]
        public string? Message { get; set; }
        
        [JsonProperty("key_data")]
        public SimpleKeyData? KeyData { get; set; }
        
        [JsonProperty("error")]
        public string? Error { get; set; }
    }
    
    internal class SimpleKeyData
    {
        [JsonProperty("tier")]
        public string? Tier { get; set; }
        
        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }
        
        [JsonProperty("current_uses")]
        public int CurrentUses { get; set; }
        
        [JsonProperty("max_uses")]
        public int? MaxUses { get; set; }
    }

    // API Response Models for AuthMe
    internal class ApiValidationResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }

        [JsonProperty("request_id")]
        public string? RequestId { get; set; }

        [JsonProperty("data")]
        public ApiLicenseData? Data { get; set; }
    }

    internal class ApiLicenseData
    {
        [JsonProperty("tier")]
        public string? Tier { get; set; }

        [JsonProperty("expires_at")]
        public DateTime? ExpiresAt { get; set; }

        [JsonProperty("usage_count")]
        public int UsageCount { get; set; }

        [JsonProperty("max_uses")]
        public int? MaxUses { get; set; }

        [JsonProperty("is_unlimited")]
        public bool IsUnlimited { get; set; }

        [JsonProperty("is_expired")]
        public bool IsExpired { get; set; }

        [JsonProperty("is_usage_limit_reached")]
        public bool IsUsageLimitReached { get; set; }

        [JsonProperty("is_revoked")]
        public bool IsRevoked { get; set; }

        [JsonProperty("is_suspended")]
        public bool IsSuspended { get; set; }

        [JsonProperty("bound_hardware_id")]
        public string? BoundHardwareId { get; set; }

        [JsonProperty("activated_at")]
        public DateTime? ActivatedAt { get; set; }

        [JsonProperty("last_validated_at")]
        public DateTime? LastValidatedAt { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, object>? Metadata { get; set; }
    }

    internal class ApiErrorResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public string? Error { get; set; }

        [JsonProperty("error_code")]
        public string? ErrorCode { get; set; }

        [JsonProperty("request_id")]
        public string? RequestId { get; set; }
    }

    internal class ApiHealthResponse
    {
        [JsonProperty("status")]
        public string? Status { get; set; }

        [JsonProperty("message")]
        public string? Message { get; set; }

        [JsonProperty("app_status")]
        public string? AppStatus { get; set; }
    }

    internal class ApiAnalyticsResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("data")]
        public ApiAnalyticsData? Data { get; set; }
    }

    internal class ApiAnalyticsData
    {
        [JsonProperty("today_validations")]
        public int TodayValidations { get; set; }

        [JsonProperty("week_validations")]
        public int WeekValidations { get; set; }

        [JsonProperty("month_validations")]
        public int MonthValidations { get; set; }

        [JsonProperty("success_rate")]
        public double SuccessRate { get; set; }

        [JsonProperty("active_licenses")]
        public int ActiveLicenses { get; set; }

        [JsonProperty("revoked_licenses")]
        public int RevokedLicenses { get; set; }

        [JsonProperty("error_breakdown")]
        public Dictionary<string, int>? ErrorBreakdown { get; set; }

        [JsonProperty("daily_stats")]
        public List<DailyStats>? DailyStats { get; set; }
    }
}
