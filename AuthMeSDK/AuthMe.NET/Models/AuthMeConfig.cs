using System;

namespace AuthMe.NET.Models
{
    /// <summary>
    /// Configuration settings for AuthMe client
    /// </summary>
    public class AuthMeConfig
    {
        /// <summary>
        /// Your application ID from the AuthMe dashboard (format: app_xxxxxxxx)
        /// </summary>
        public string AppId { get; set; } = string.Empty;

        /// <summary>
        /// Your application secret from the AuthMe dashboard (format: sk_xxxxxxxx)
        /// </summary>
        public string AppSecret { get; set; } = string.Empty;

        /// <summary>
        /// Your application name (optional, for identification)
        /// </summary>
        public string AppName { get; set; } = string.Empty;

        /// <summary>
        /// AuthMe API URL (defaults to official AuthMe service)
        /// </summary>
        public string ApiUrl { get; set; } = "https://authme.space/api";

        /// <summary>
        /// Cache duration in seconds for license validation results (default: 3 seconds for security)
        /// </summary>
        public int CacheDurationSeconds { get; set; } = 3;

        /// <summary>
        /// Hardware ID generation method
        /// </summary>
        public HwidMethod HwidMethod { get; set; } = HwidMethod.SystemUuid;

        /// <summary>
        /// Discord support server invite (default: discord.gg/server)
        /// </summary>
        public string DiscordSupport { get; set; } = "discord.gg/server";

        /// <summary>
        /// Request timeout in seconds
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Enable usage analytics and statistics collection (default: false)
        /// </summary>
        public bool EnableAnalytics { get; set; } = false;

        /// <summary>
        /// Show detailed validation information (license key, HWID, method) - default: false for security
        /// </summary>
        public bool ShowValidationDetails { get; set; } = false;

        /// <summary>
        /// Show success message when authentication succeeds - default: true
        /// </summary>
        public bool ShowSuccessMessage { get; set; } = true;

        /// <summary>
        /// Show error messages when authentication fails - default: true
        /// </summary>
        public bool ShowErrorMessages { get; set; } = true;

        /// <summary>
        /// Custom success message (leave empty for default)
        /// </summary>
        public string CustomSuccessMessage { get; set; } = string.Empty;

        /// <summary>
        /// Custom error message (leave empty for default)
        /// </summary>
        public string CustomErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// Show security violation alerts - default: true
        /// </summary>
        public bool ShowSecurityAlerts { get; set; } = true;

        /// <summary>
        /// Enable automatic retry on transient failures (default: true)
        /// </summary>
        public bool EnableRetry { get; set; } = true;

        /// <summary>
        /// Maximum number of retry attempts for failed requests (default: 3)
        /// </summary>
        public int MaxRetryAttempts { get; set; } = 3;

        /// <summary>
        /// Validates the configuration
        /// </summary>
        /// <returns>True if configuration is valid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(AppId) &&
                   !string.IsNullOrWhiteSpace(AppSecret) &&
                   !string.IsNullOrWhiteSpace(ApiUrl) &&
                   AppId.Length >= 8 && // Minimum app ID length
                   AppSecret.Length >= 16 && // Minimum secret length
                   Uri.TryCreate(ApiUrl, UriKind.Absolute, out _) && // Valid URL
                   CacheDurationSeconds >= 0 &&
                   TimeoutSeconds > 0 &&
                   MaxRetryAttempts >= 0;
        }
    }

    /// <summary>
    /// Hardware ID generation methods
    /// </summary>
    public enum HwidMethod
    {
        /// <summary>
        /// Simple hardware ID (fastest, basic uniqueness)
        /// </summary>
        Simple,

        /// <summary>
        /// Basic system information (faster, less unique)
        /// </summary>
        Basic,

        /// <summary>
        /// Comprehensive hardware fingerprinting (recommended for security)
        /// </summary>
        Comprehensive,

        /// <summary>
        /// MAC address only (simple but can change)
        /// </summary>
        MacAddress,

        /// <summary>
        /// System UUID only (reliable on most systems)
        /// </summary>
        SystemUuid,

        /// <summary>
        /// Custom hardware ID (user-provided)
        /// </summary>
        Custom
    }
}
