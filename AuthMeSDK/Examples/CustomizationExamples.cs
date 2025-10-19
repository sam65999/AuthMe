using AuthMe.NET;
using AuthMe.NET.Models;

namespace AuthMe.Examples
{
    /// <summary>
    /// Ready-to-use configuration examples for different scenarios and use cases
    /// Copy and paste these configurations into your application!
    /// Updated for AuthMe with modern features and enhanced security
    /// </summary>
    public static class CustomizationExamples
    {
        /// <summary>
        /// 🔇 Silent Mode - No console output at all
        /// Perfect for: Background services, APIs, microservices, when you handle UI yourself
        /// Features: Minimal overhead, maximum performance, custom error handling
        /// </summary>
        public static AuthMeConfig SilentMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// ✨ Minimal Mode - Clean, simple output (RECOMMENDED)
        /// Perfect for: Most applications, production use, desktop applications
        /// Features: Clean UI feedback, essential notifications, optimized performance
        /// </summary>
        public static AuthMeConfig MinimalMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// 🔍 Debug Mode - Show all details and diagnostics
        /// Perfect for: Development, troubleshooting, testing, integration debugging
        /// Features: Verbose logging, detailed error messages, performance metrics
        /// </summary>
        public static AuthMeConfig DebugMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// 🎮 Gaming Style - Fun, engaging messages with gaming terminology
        /// Perfect for: Games, entertainment apps, gaming utilities, mod tools
        /// Features: Gaming-themed messages, achievement-style notifications, player-friendly UI
        /// </summary>
        public static AuthMeConfig GamingMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// 🏢 Enterprise Mode - Professional, formal messages with compliance features
        /// Perfect for: Business applications, corporate software, enterprise solutions
        /// Features: Professional language, audit logging, compliance reporting, security focus
        /// </summary>
        public static AuthMeConfig EnterpriseMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// 🖥️ GUI Application Mode - Optimized for graphical user interfaces
        /// Perfect for: Windows Forms, WPF, desktop applications, visual tools
        /// Features: UI-friendly notifications, non-blocking operations, visual feedback integration
        /// </summary>
        public static AuthMeConfig GuiMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 3, // Short cache for security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };

        /// <summary>
        /// 🛡️ Security-First Mode - Maximum security awareness and monitoring
        /// Perfect for: High-security applications, sensitive software, financial tools, healthcare apps
        /// Features: Enhanced security monitoring, detailed violation reporting, audit trails
        /// </summary>
        public static AuthMeConfig SecurityMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",                  // Get from AuthMe Dashboard → Applications → Your App → App ID
            AppSecret = "YOUR_APP_SECRET_HERE",            // Get from AuthMe Dashboard → Applications → Your App → App Secret
            AppName = "Your Application Name",                 // Your application's display name
            CacheDurationSeconds = 1, // Very short cache for maximum security
            HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
            DiscordSupport = "discord.gg/server" // Your support server
        };
    }

    /// <summary>
        /// 🚀 Performance Mode - Optimized for high-throughput applications
        /// Perfect for: High-volume applications, server applications, batch processing
        /// Features: Optimized caching, connection pooling, minimal overhead
        /// </summary>
        public static AuthMeConfig PerformanceMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",
            AppSecret = "YOUR_APP_SECRET_HERE",
            AppName = "Your Application Name",
            CacheDurationSeconds = 30, // Longer cache for performance
            HwidMethod = HwidMethod.Basic, // Faster HWID method
            DiscordSupport = "discord.gg/server"
        };

        /// <summary>
        /// 📱 Mobile-Friendly Mode - Optimized for mobile and cross-platform apps
        /// Perfect for: Xamarin, MAUI, mobile applications, cross-platform tools
        /// Features: Network-aware caching, offline support, mobile-optimized UI
        /// </summary>
        public static AuthMeConfig MobileMode => new AuthMeConfig
        {
            AppId = "YOUR_APP_ID_HERE",
            AppSecret = "YOUR_APP_SECRET_HERE",
            AppName = "Your Application Name",
            CacheDurationSeconds = 60, // Longer cache for mobile networks
            HwidMethod = HwidMethod.Comprehensive,
            DiscordSupport = "discord.gg/server"
        };
    }

    /// <summary>
    /// Example usage of the preset configurations with modern AuthMe features
    /// </summary>
    public class UsageExamples
    {
        public static async Task ExampleUsage()
        {
            // 1. Use a preset configuration
            using var authMe = new AuthMeClient(CustomizationExamples.MinimalMode);
            
            // 2. Or use a preset for gaming applications
            using var gamingAuthMe = new AuthMeClient(CustomizationExamples.GamingMode);
            
            // 3. Advanced authentication with full feature demonstration
            var result = await authMe.AuthenticateAsync("AUTHME-PREM-abc123def456");
            
            if (result.IsSuccess)
            {
                Console.WriteLine("🎉 User is authenticated!");
                var keyData = result.KeyData;
                
                // Display license information
                Console.WriteLine($"License Tier: {keyData.Tier}");
                Console.WriteLine($"Expires: {keyData.ExpiresAt?.ToString("yyyy-MM-dd") ?? "Never"}");
                Console.WriteLine($"Usage: {keyData.UsageCount}/{(keyData.IsUnlimited ? "∞" : keyData.MaxUses.ToString())}");
                
                // Check for warnings
                if (keyData.IsExpired)
                    Console.WriteLine("⚠️ Warning: License is expired!");
                if (keyData.IsUsageLimitReached)
                    Console.WriteLine("⚠️ Warning: Usage limit reached!");
                

                
                // Your protected application logic here
                await RunProtectedApplication(keyData);
            }
            else
            {
                Console.WriteLine($"❌ Authentication failed: {result.Message}");
                
                // Handle security violations
                if (result.IsSecurityViolation)
                {
                    Console.WriteLine("🚨 Security violation detected!");
                    Console.WriteLine("This may be due to:");
                    Console.WriteLine("• Hardware ID mismatch");
                    Console.WriteLine("• License key sharing");
                    Console.WriteLine("• Suspicious usage patterns");
                    Console.WriteLine($"Contact support: {gamingConfig.DiscordSupport}");
                }
            }
        }
        
        /// <summary>
        /// Example of tier-based feature access
        /// </summary>
        private static async Task RunProtectedApplication(LicenseKeyData keyData)
        {
            Console.WriteLine("\n🚀 Starting protected application...");
            
            // Basic features available to all tiers
            Console.WriteLine("✅ Basic features unlocked");
            
            // Premium features
            if (keyData.Tier.Equals("premium", StringComparison.OrdinalIgnoreCase) || 
                keyData.Tier.Equals("enterprise", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("✅ Premium features unlocked");
                await Task.Delay(100); // Simulate premium feature initialization
            }
            
            // Enterprise features
            if (keyData.Tier.Equals("enterprise", StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("✅ Enterprise features unlocked");
                Console.WriteLine("✅ Advanced analytics enabled");
                Console.WriteLine("✅ Priority support activated");
                await Task.Delay(100); // Simulate enterprise feature initialization
            }
            
            Console.WriteLine("🎯 Application ready!");
        }
    }
}
