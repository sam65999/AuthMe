using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AuthMe.NET.Models;
using AuthMe.NET.Services;

namespace AuthMe.NET
{
    /// <summary>
    /// Main AuthMe client for license validation and authentication
    /// </summary>
    public class AuthMeClient : IDisposable
    {
        private readonly AuthMeConfig _config;
        private readonly AuthMeApiClient _apiClient;
        private readonly string _hardwareId;
        private readonly Dictionary<string, CachedResult> _cache;
        private bool _disposed;

        /// <summary>
        /// Gets the hardware ID for this device
        /// </summary>
        public string HardwareId => _hardwareId;

        /// <summary>
        /// Gets the configuration used by this client
        /// </summary>
        public AuthMeConfig Config => _config;

        /// <summary>
        /// Initializes a new AuthMe client with the specified configuration
        /// </summary>
        /// <param name="config">AuthMe configuration</param>
        /// <exception cref="ArgumentNullException">Thrown when config is null</exception>
        /// <exception cref="ArgumentException">Thrown when config is invalid</exception>
        public AuthMeClient(AuthMeConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            if (!_config.IsValid())
            {
                throw new ArgumentException("Invalid AuthMe configuration. Please check AppId and AppSecret from your AuthMe dashboard.", nameof(config));
            }

            _apiClient = new AuthMeApiClient(_config);
            _hardwareId = HardwareIdGenerator.Generate(_config.HwidMethod);
            _cache = new Dictionary<string, CachedResult>();
        }

        /// <summary>
        /// Validates a license key with hardware ID binding
        /// </summary>
        /// <param name="licenseKey">License key to validate</param>
        /// <param name="useCache">Whether to use cached results (default: true)</param>
        /// <param name="forceHwidBinding">Whether to enforce hardware ID binding (default: true)</param>
        /// <returns>Validation result</returns>
        public async Task<ValidationResult> ValidateKeyAsync(string licenseKey, bool useCache = true, bool forceHwidBinding = true)
        {
            if (string.IsNullOrWhiteSpace(licenseKey))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    Message = "License key cannot be empty",
                    IsCached = false
                };
            }

            // Check cache first
            var cacheKey = GetCacheKey(licenseKey);
            if (useCache && _cache.TryGetValue(cacheKey, out var cachedResult) && IsCacheValid(cachedResult))
            {
                var result = cachedResult.Result;
                result.IsCached = true;
                return result;
            }

            // Validate with server
            var hardwareId = forceHwidBinding ? _hardwareId : _hardwareId; // Always use HWID for security
            var validationResult = await _apiClient.ValidateLicenseAsync(licenseKey, hardwareId);

            // Handle caching
            if (useCache)
            {
                if (validationResult.IsValid)
                {
                    // Cache successful validations
                    _cache[cacheKey] = new CachedResult
                    {
                        Result = validationResult,
                        Timestamp = DateTime.UtcNow
                    };
                }
                else
                {
                    // Remove from cache if validation fails (key might be revoked)
                    _cache.Remove(cacheKey);
                }
            }

            return validationResult;
        }

        /// <summary>
        /// Authenticates a user with a license key and returns detailed result
        /// </summary>
        /// <param name="licenseKey">License key to authenticate</param>
        /// <param name="showValidationDetails">Whether to display validation details (overrides config setting if specified)</param>
        /// <returns>Authentication result</returns>
        public async Task<AuthenticationResult> AuthenticateAsync(string licenseKey, bool? showValidationDetails = null)
        {
            // Show validation details if requested
            bool shouldShowDetails = showValidationDetails ?? _config.ShowValidationDetails;
            if (shouldShowDetails)
            {
                Console.WriteLine("AuthMe License Validation");
                Console.WriteLine("============================");
                Console.WriteLine($"License Key: {licenseKey}");
                Console.WriteLine($"Hardware ID: {_hardwareId}");
                Console.WriteLine($"HWID Method: {_config.HwidMethod}");
            }

            var validationResult = await ValidateKeyAsync(licenseKey);

            var authResult = new AuthenticationResult
            {
                IsSuccess = validationResult.IsValid,
                Message = validationResult.Message,
                ErrorCode = validationResult.ErrorCode,
                KeyData = validationResult.KeyData,
                HardwareId = _hardwareId,
                LicenseKey = licenseKey
            };

            // Handle success messages
            if (authResult.IsSuccess && _config.ShowSuccessMessage)
            {
                string successMsg = !string.IsNullOrEmpty(_config.CustomSuccessMessage)
                    ? _config.CustomSuccessMessage
                    : "Key Authenticated";
                Console.WriteLine($"\n{successMsg}");
            }

            // Handle error messages
            if (!authResult.IsSuccess && _config.ShowErrorMessages)
            {
                string errorMsg = !string.IsNullOrEmpty(_config.CustomErrorMessage)
                    ? _config.CustomErrorMessage
                    : $"Authentication failed: {authResult.Message}";
                Console.WriteLine($"\n{errorMsg}");

                // Handle security violations
                if (authResult.IsSecurityViolation && _config.ShowSecurityAlerts)
                {
                    Console.WriteLine("\nSECURITY ALERT:");
                    Console.WriteLine("Your license key has been revoked due to suspicious activity.");
                    Console.WriteLine($"Contact support at {_config.DiscordSupport} if this is an error.");
                }
            }

            return authResult;
        }

        /// <summary>
        /// Tests the connection to the AuthMe service
        /// </summary>
        /// <returns>Connection test result</returns>
        public async Task<(bool Success, string Message)> TestConnectionAsync()
        {
            return await _apiClient.TestConnectionAsync();
        }

        /// <summary>
        /// Gets analytics data for the current application (if analytics are enabled)
        /// </summary>
        /// <returns>Analytics data</returns>
        public async Task<AnalyticsData> GetAnalyticsAsync()
        {
            if (!_config.EnableAnalytics)
            {
                throw new InvalidOperationException("Analytics are not enabled. Set EnableAnalytics to true in your configuration.");
            }

            return await _apiClient.GetAnalyticsAsync();
        }

        /// <summary>
        /// Logs a security violation for monitoring purposes
        /// </summary>
        /// <param name="errorCode">The error code of the violation</param>
        /// <param name="message">The violation message</param>
        /// <returns>Task</returns>
        public async Task LogSecurityViolationAsync(string errorCode, string message)
        {
            await _apiClient.LogSecurityViolationAsync(errorCode, message, _hardwareId);
        }

        /// <summary>
        /// Clears the validation cache
        /// </summary>
        public void ClearCache()
        {
            _cache.Clear();
        }

        /// <summary>
        /// Gets cache information
        /// </summary>
        /// <returns>Cache statistics</returns>
        public (int TotalEntries, int ValidEntries, int ExpiredEntries) GetCacheInfo()
        {
            var validEntries = 0;
            var expiredEntries = 0;

            foreach (var entry in _cache.Values)
            {
                if (IsCacheValid(entry))
                    validEntries++;
                else
                    expiredEntries++;
            }

            return (_cache.Count, validEntries, expiredEntries);
        }

        /// <summary>
        /// Gets detailed hardware information for debugging
        /// </summary>
        /// <returns>Hardware information dictionary</returns>
        public Dictionary<string, string> GetHardwareInfo()
        {
            return HardwareIdGenerator.GetHardwareInfo();
        }

        private string GetCacheKey(string licenseKey)
        {
            return $"{licenseKey}:{_hardwareId}".GetHashCode().ToString();
        }

        private bool IsCacheValid(CachedResult cachedResult)
        {
            var age = DateTime.UtcNow - cachedResult.Timestamp;
            return age.TotalSeconds < _config.CacheDurationSeconds;
        }

        /// <summary>
        /// Disposes the AuthMe client and releases resources
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _apiClient?.Dispose();
                _cache?.Clear();
                _disposed = true;
            }
        }

        private class CachedResult
        {
            public ValidationResult Result { get; set; } = new();
            public DateTime Timestamp { get; set; }
        }
    }
}
