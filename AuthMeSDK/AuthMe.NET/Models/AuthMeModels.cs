using System;
using System.Collections.Generic;

namespace AuthMe.NET.Models
{
    /// <summary>
    /// Result of license key validation
    /// </summary>
    public class ValidationResult
    {
        /// <summary>
        /// Whether the license key is valid
        /// </summary>
        public bool IsValid { get; set; }

        /// <summary>
        /// Validation message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error code for failed validations
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// License key data (only populated for valid keys)
        /// </summary>
        public LicenseKeyData? KeyData { get; set; }

        /// <summary>
        /// Whether the result was retrieved from cache
        /// </summary>
        public bool IsCached { get; set; }

        /// <summary>
        /// Whether this is a security violation (key revoked due to HWID mismatch, sharing, etc.)
        /// </summary>
        public bool IsSecurityViolation => 
            ErrorCode.Equals("HWID_MISMATCH", StringComparison.OrdinalIgnoreCase) ||
            ErrorCode.Equals("REVOKED", StringComparison.OrdinalIgnoreCase) ||
            Message.IndexOf("SECURITY VIOLATION", StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Timestamp when the validation was performed
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Request ID for tracking and debugging
        /// </summary>
        public string RequestId { get; set; } = string.Empty;
    }

    /// <summary>
    /// License key data returned from successful validation
    /// </summary>
    public class LicenseKeyData
    {
        /// <summary>
        /// License tier (basic, premium, enterprise)
        /// </summary>
        public string Tier { get; set; } = string.Empty;

        /// <summary>
        /// Expiration date (null if never expires)
        /// </summary>
        public DateTime? ExpiresAt { get; set; }

        /// <summary>
        /// Current usage count
        /// </summary>
        public int UsageCount { get; set; }

        /// <summary>
        /// Maximum allowed uses (null if unlimited)
        /// </summary>
        public int? MaxUses { get; set; }

        /// <summary>
        /// Additional metadata
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Whether the license has unlimited uses
        /// </summary>
        public bool IsUnlimited => MaxUses == null;

        /// <summary>
        /// Whether the license is expired
        /// </summary>
        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

        /// <summary>
        /// Whether the license has reached its usage limit
        /// </summary>
        public bool IsUsageLimitReached => MaxUses.HasValue && UsageCount >= MaxUses.Value;

        /// <summary>
        /// Whether the license key has been revoked
        /// </summary>
        public bool IsRevoked { get; set; }

        /// <summary>
        /// Whether the license key has been suspended
        /// </summary>
        public bool IsSuspended { get; set; }

        /// <summary>
        /// Hardware ID bound to this license
        /// </summary>
        public string BoundHardwareId { get; set; } = string.Empty;

        /// <summary>
        /// When the license was first activated
        /// </summary>
        public DateTime? ActivatedAt { get; set; }

        /// <summary>
        /// Last validation timestamp
        /// </summary>
        public DateTime? LastValidatedAt { get; set; }
    }

    /// <summary>
    /// Authentication result with additional context
    /// </summary>
    public class AuthenticationResult
    {
        /// <summary>
        /// Whether authentication was successful
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Authentication message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Error code for failed authentication
        /// </summary>
        public string ErrorCode { get; set; } = string.Empty;

        /// <summary>
        /// License key data (only populated for successful authentication)
        /// </summary>
        public LicenseKeyData? KeyData { get; set; }

        /// <summary>
        /// Hardware ID used for validation
        /// </summary>
        public string HardwareId { get; set; } = string.Empty;

        /// <summary>
        /// License key that was validated
        /// </summary>
        public string LicenseKey { get; set; } = string.Empty;

        /// <summary>
        /// Whether this is a security violation
        /// </summary>
        public bool IsSecurityViolation => 
            ErrorCode.Equals("HWID_MISMATCH", StringComparison.OrdinalIgnoreCase) ||
            ErrorCode.Equals("REVOKED", StringComparison.OrdinalIgnoreCase) ||
            Message.IndexOf("SECURITY VIOLATION", StringComparison.OrdinalIgnoreCase) >= 0;

        /// <summary>
        /// Request ID for tracking and debugging
        /// </summary>
        public string RequestId { get; set; } = string.Empty;

        /// <summary>
        /// Timestamp when the authentication was performed
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Analytics data for the application
    /// </summary>
    public class AnalyticsData
    {
        /// <summary>
        /// Total validations today
        /// </summary>
        public int TodayValidations { get; set; }

        /// <summary>
        /// Total validations this week
        /// </summary>
        public int WeekValidations { get; set; }

        /// <summary>
        /// Total validations this month
        /// </summary>
        public int MonthValidations { get; set; }

        /// <summary>
        /// Success rate percentage (0-100)
        /// </summary>
        public double SuccessRate { get; set; }

        /// <summary>
        /// Total active license keys
        /// </summary>
        public int ActiveLicenses { get; set; }

        /// <summary>
        /// Total revoked license keys
        /// </summary>
        public int RevokedLicenses { get; set; }

        /// <summary>
        /// Breakdown of error codes
        /// </summary>
        public Dictionary<string, int> ErrorBreakdown { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Daily statistics for the past 30 days
        /// </summary>
        public List<DailyStats> DailyStats { get; set; } = new List<DailyStats>();
    }

    /// <summary>
    /// Daily statistics entry
    /// </summary>
    public class DailyStats
    {
        /// <summary>
        /// Date for these statistics
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Total validations on this date
        /// </summary>
        public int TotalValidations { get; set; }

        /// <summary>
        /// Successful validations on this date
        /// </summary>
        public int SuccessfulValidations { get; set; }

        /// <summary>
        /// Failed validations on this date
        /// </summary>
        public int FailedValidations { get; set; }
    }
}
