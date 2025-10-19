using System;
using System.Threading.Tasks;
using AuthMe.NET;
using AuthMe.NET.Models;

namespace AuthMe.Examples.Console
{
    /// <summary>
    /// Example console application demonstrating AuthMe.NET integration
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            System.Console.WriteLine("🔒 AuthMe .NET Console Example");
            System.Console.WriteLine("===============================");
            System.Console.WriteLine();

            // Initialize AuthMe with your app credentials
            // IMPORTANT: Replace these placeholder values with your actual credentials from the AuthMe dashboard
            var config = new AuthMeConfig
            {
                AppId = "YOUR_APP_ID_HERE", // Get from AuthMe Dashboard → Applications → Your App → App ID
                AppSecret = "YOUR_APP_SECRET_HERE", // Get from AuthMe Dashboard → Applications → Your App → App Secret
                AppName = "Your Application Name", // Your application's display name
                CacheDurationSeconds = 3, // Cache duration for license validation (recommended: 3-5 seconds)
                HwidMethod = HwidMethod.Comprehensive, // Hardware ID collection method
                DiscordSupport = "discord.gg/your-server", // Your Discord support server (optional)
            };

            using var authMe = new AuthMeClient(config);



            // Validate configuration
            if (!config.IsValid())
            {
                System.Console.WriteLine("❌ Invalid configuration!");
                System.Console.WriteLine("Please replace the placeholder values in Program.cs with your actual credentials:");
                System.Console.WriteLine("");
                System.Console.WriteLine("Replace these placeholders:");
                System.Console.WriteLine("• (ownerid) → Your actual Owner ID");
                System.Console.WriteLine("• (appsecret) → Your actual App Secret");
                System.Console.WriteLine("• namehere → Your actual application name");
                System.Console.WriteLine("");
                System.Console.WriteLine("To get your credentials:");
                System.Console.WriteLine("1. Start your AuthMe admin dashboard: npm run dev");
                System.Console.WriteLine("2. Go to your AuthMe dashboard (usually http://localhost:3000)");
                System.Console.WriteLine("3. Select your application");
                System.Console.WriteLine("4. Navigate to the 'Credentials' tab");
                System.Console.WriteLine("5. Copy your Owner ID and App Secret");
                System.Console.WriteLine("");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
                return;
            }

            try
            {
                // Test connection first
                System.Console.WriteLine("Testing connection to AuthMe service...");
                var (success, message) = await authMe.TestConnectionAsync();
                
                if (!success)
                {
                    System.Console.WriteLine($" Connection failed: {message}");
                    System.Console.WriteLine("Please check your internet connection and configuration.");
                    System.Console.WriteLine("Press any key to exit...");
                    System.Console.ReadKey();
                    return;
                }

                System.Console.WriteLine("✅ Connection successful!");
                System.Console.WriteLine();

                // Show hardware information
                System.Console.WriteLine("Hardware Information:");
                System.Console.WriteLine("--------------------");
                var hwInfo = authMe.GetHardwareInfo();
                foreach (var kvp in hwInfo)
                {
                    System.Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
                System.Console.WriteLine();

                // 🔐 STEP 1: AUTHENTICATION GATE - This happens FIRST, before main application
                System.Console.WriteLine("🔐 AuthMe License Validation");
                System.Console.WriteLine("============================");
                System.Console.Write("Enter your license key: ");
                var licenseKey = System.Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(licenseKey))
                {
                    System.Console.WriteLine("❌ No license key provided");
                    System.Console.WriteLine("Press any key to exit...");
                    System.Console.ReadKey();
                    return;
                }

                // Authenticate using config settings for output customization
                var authResult = await authMe.AuthenticateAsync(licenseKey);

                if (authResult.IsSuccess)
                {
                    // ✅ STEP 2: AUTHENTICATION SUCCESSFUL - START MAIN APPLICATION
                    await RunMainApplication(authResult.KeyData!);
                }
                else
                {
                    // ❌ STEP 3: AUTHENTICATION FAILED - DENY ACCESS TO MAIN APPLICATION
                    System.Console.WriteLine();

                    // Check for global ban first
                    bool isGloballyBanned = authResult.Message.Contains("Your IP/device combination has been banned", StringComparison.OrdinalIgnoreCase);

                    if (isGloballyBanned)
                    {
                        System.Console.WriteLine("🚨 GLOBAL BAN DETECTED");
                        System.Console.WriteLine("═══════════════════════");
                        System.Console.WriteLine("🚫 Your IP address and device have been globally banned.");
                        System.Console.WriteLine();
                        System.Console.WriteLine("This affects ALL license keys from your current location/device.");
                        System.Console.WriteLine();

                        // Extract ban reason from message
                        var reasonStart = authResult.Message.IndexOf("Reason: ") + 8;
                        var reasonEnd = authResult.Message.IndexOf(".", reasonStart);
                        if (reasonStart > 7 && reasonEnd > reasonStart)
                        {
                            var banReason = authResult.Message.Substring(reasonStart, reasonEnd - reasonStart);
                            System.Console.WriteLine($"📝 Ban Reason: {banReason}");
                            System.Console.WriteLine();
                        }

                        // Check if there's also a key-specific issue
                        if (authResult.Message.Contains("Additionally, this license key issue:"))
                        {
                            System.Console.WriteLine("Additionally, this specific license key has issues:");
                            System.Console.WriteLine();
                        }
                    }

                    // Show specific error messages based on the failure reason
                    if (authResult.KeyData != null)
                    {
                        var keyData = authResult.KeyData;

                        if (keyData.IsRevoked)
                        {
                            if (!isGloballyBanned) System.Console.WriteLine("🚫 Your license key has been revoked.");
                            else System.Console.WriteLine("🚫 License key status: REVOKED");
                            System.Console.WriteLine();
                            System.Console.WriteLine("This typically happens due to:");
                            System.Console.WriteLine("• Violation of terms of service");
                            System.Console.WriteLine("• Chargeback or payment dispute");
                            System.Console.WriteLine("• Unauthorized key sharing");
                            System.Console.WriteLine();

                            // Extract key-specific reason if it's a combined message
                            if (isGloballyBanned && authResult.Message.Contains("Additionally, this license key issue:"))
                            {
                                var keyReasonStart = authResult.Message.IndexOf("Additionally, this license key issue: ") + 38;
                                var keyReasonEnd = authResult.Message.IndexOf(".", keyReasonStart);
                                if (keyReasonStart > 37 && keyReasonEnd > keyReasonStart)
                                {
                                    var keyReason = authResult.Message.Substring(keyReasonStart, keyReasonEnd - keyReasonStart);
                                    System.Console.WriteLine($"📝 Revocation Reason: {keyReason}");
                                }
                            }
                            else
                            {
                                System.Console.WriteLine($"📝 Revocation Reason: {authResult.Message}");
                            }
                        }
                        else if (keyData.IsSuspended)
                        {
                            if (!isGloballyBanned) System.Console.WriteLine("⏸️ Your license key has been suspended.");
                            else System.Console.WriteLine("⏸️ License key status: SUSPENDED");
                            System.Console.WriteLine();
                            System.Console.WriteLine("This is usually temporary and may be due to:");
                            System.Console.WriteLine("• Pending payment verification");
                            System.Console.WriteLine("• Account review in progress");
                            System.Console.WriteLine("• Temporary security hold");
                            System.Console.WriteLine();

                            // Extract key-specific reason if it's a combined message
                            if (isGloballyBanned && authResult.Message.Contains("Additionally, this license key issue:"))
                            {
                                var keyReasonStart = authResult.Message.IndexOf("Additionally, this license key issue: ") + 38;
                                var keyReasonEnd = authResult.Message.IndexOf(".", keyReasonStart);
                                if (keyReasonStart > 37 && keyReasonEnd > keyReasonStart)
                                {
                                    var keyReason = authResult.Message.Substring(keyReasonStart, keyReasonEnd - keyReasonStart);
                                    System.Console.WriteLine($"📝 Suspension Reason: {keyReason}");
                                }
                            }
                            else
                            {
                                System.Console.WriteLine($"📝 Suspension Reason: {authResult.Message}");
                            }
                        }
                        else if (keyData.IsExpired)
                        {
                            System.Console.WriteLine("⏰ Your license key has expired.");
                            System.Console.WriteLine($"📅 Expired on: {keyData.ExpiresAt?.ToString("yyyy-MM-dd")}");
                            System.Console.WriteLine();
                            System.Console.WriteLine("💡 To continue using this application, please renew your license.");
                        }
                        else if (keyData.IsUsageLimitReached)
                        {
                            System.Console.WriteLine("📊 Your license key usage limit has been reached.");
                            System.Console.WriteLine($"📈 Usage: {keyData.UsageCount}/{keyData.MaxUses}");
                            System.Console.WriteLine();
                            System.Console.WriteLine("💡 Please upgrade your license for additional usage.");
                        }
                        else
                        {
                            System.Console.WriteLine("❌ License key validation failed.");
                            System.Console.WriteLine($"📝 Reason: {authResult.Message}");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("❌ License key validation failed.");
                        System.Console.WriteLine($"📝 Reason: {authResult.Message}");
                    }

                    if (authResult.IsSecurityViolation)
                    {
                        System.Console.WriteLine();
                        System.Console.WriteLine("🚨 SECURITY VIOLATION DETECTED:");
                        System.Console.WriteLine("• Key is being used on multiple devices");
                        System.Console.WriteLine("• Hardware ID spoofing detected");
                        System.Console.WriteLine("• Unauthorized key sharing");
                    }

                    // Always show support information
                    System.Console.WriteLine();
                    System.Console.WriteLine("🆘 Need help? Contact support:");
                    System.Console.WriteLine($"💬 Discord: {authMe.Config.DiscordSupport}");
                    System.Console.WriteLine("📧 If this is a mistake, please reach out to our support team.");

                    System.Console.WriteLine();
                    System.Console.WriteLine("Main application will not start.");
                }

                System.Console.WriteLine();
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Application error: {ex.Message}");
                System.Console.WriteLine("Press any key to exit...");
                System.Console.ReadKey();
            }
        }

        /// <summary>
        /// 🚀 MAIN APPLICATION - This only runs AFTER successful authentication
        /// Replace this method with your actual application logic
        /// </summary>
        static async Task RunMainApplication(LicenseKeyData keyData)
        {
            // Clear the authentication screen
            System.Console.Clear();

            // Welcome to the ACTUAL application
            System.Console.WriteLine("╔══════════════════════════════════════╗");
            System.Console.WriteLine("║        YOUR APPLICATION NAME        ║");
            System.Console.WriteLine("╚══════════════════════════════════════╝");
            System.Console.WriteLine();
            System.Console.WriteLine($"🎉 Welcome! License: {keyData.Tier.ToUpper()}");
            System.Console.WriteLine($"📅 Expires: {(keyData.ExpiresAt?.ToString("yyyy-MM-dd") ?? "Never")}");
            System.Console.WriteLine($"📊 Usage: {keyData.UsageCount}{(keyData.IsUnlimited ? " (unlimited)" : $"/{keyData.MaxUses}")}");
            System.Console.WriteLine();

            // Show warnings if needed
            if (keyData.IsExpired)
            {
                System.Console.WriteLine("⚠️  Warning: License is expired!");
            }

            if (keyData.IsUsageLimitReached)
            {
                System.Console.WriteLine("⚠️  Warning: Usage limit reached!");
            }

            // 🎯 YOUR ACTUAL APPLICATION FEATURES GO HERE
            System.Console.WriteLine("Available Features:");
            System.Console.WriteLine("==================");

            while (true)
            {
                System.Console.WriteLine();
                System.Console.WriteLine("1. 📊 Advanced Analytics");
                System.Console.WriteLine("2. 🎮 Premium Features");
                System.Console.WriteLine("3. 🔧 Settings");
                System.Console.WriteLine("4. ❓ Help");
                System.Console.WriteLine("5. 🚪 Exit");
                System.Console.WriteLine();
                System.Console.Write("Choose an option (1-5): ");

                var choice = System.Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        System.Console.WriteLine("\n📊 Running Advanced Analytics...");
                        System.Console.WriteLine("Analytics data loaded successfully!");
                        await Task.Delay(1000); // Simulate work
                        break;

                    case "2":
                        System.Console.WriteLine("\n🎮 Accessing Premium Features...");
                        System.Console.WriteLine("Premium features unlocked!");
                        await Task.Delay(1000); // Simulate work
                        break;

                    case "3":
                        System.Console.WriteLine("\n🔧 Opening Settings...");
                        System.Console.WriteLine("Settings panel opened!");
                        await Task.Delay(1000); // Simulate work
                        break;

                    case "4":
                        System.Console.WriteLine("\n❓ Help Information:");
                        System.Console.WriteLine("This is your protected application!");
                        System.Console.WriteLine("Contact support: your-support@email.com");
                        break;

                    case "5":
                        System.Console.WriteLine("\n👋 Thank you for using our software!");
                        System.Console.WriteLine("Press any key to exit...");
                        System.Console.ReadKey();
                        return;

                    default:
                        System.Console.WriteLine("\n❌ Invalid option. Please choose 1-5.");
                        break;
                }
            }
        }
    }
}
