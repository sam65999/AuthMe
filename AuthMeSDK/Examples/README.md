# AuthMe .NET SDK Examples

This directory contains example applications demonstrating how to integrate the AuthMe .NET SDK into different types of .NET applications.

## 📁 Examples Overview

### 1. Console Application (`ConsoleApp/`)
A comprehensive console application that demonstrates:
- Modern AuthMe client initialization with advanced configuration
- Interactive license key validation with real-time feedback
- Hardware information display and debugging tools
- Advanced error handling and security violation detection
- Clean, colorized console output with progress indicators
- Analytics integration and usage tracking
- Cache management and performance optimization

**Key Features:**
- Interactive license key input with validation
- Real-time validation progress with spinner animations
- Detailed hardware ID information and debugging
- Connection testing with retry logic
- Protected application simulation with feature tiers
- Analytics dashboard integration
- Cache statistics and management
- Security violation alerts with support contact

### 2. Windows Forms Application (`WindowsFormsApp/`)
A modern Windows Forms GUI application that showcases:
- Contemporary UI design with AuthMe integration
- Real-time validation feedback with progress bars
- Protected content display with tier-based features
- Advanced hardware information dialog with system details
- Visual status indicators and notification system
- Analytics dashboard integration
- Settings panel for configuration management

**Key Features:**
- Modern, responsive Windows Forms interface with custom styling
- License key input with real-time validation and auto-formatting
- Animated progress indicators and status updates
- Tier-based protected panels (Basic, Premium, Enterprise)
- Comprehensive hardware information popup with export functionality
- Color-coded output console with filtering and search
- Settings management with configuration persistence
- Analytics integration with usage charts and statistics
- System tray integration with notifications

### 3. WPF Application (`WpfApp/`)
A cutting-edge WPF application demonstrating:
- Modern XAML-based UI with Fluent Design principles
- Asynchronous validation with advanced progress feedback
- Material Design 3.0 inspired interface with adaptive themes
- Advanced data binding with reactive programming patterns
- Responsive layout with dynamic content adaptation
- Analytics integration with real-time charts and dashboards
- Advanced settings management with live preview

**Key Features:**
- Ultra-modern WPF interface with Fluent Design and Acrylic effects
- Smooth micro-animations and page transitions with Lottie integration
- Adaptive color schemes with light/dark mode support
- Tier-based protected content areas with dynamic feature unlocking
- Interactive hardware information display with system monitoring
- Full MVVM architecture with reactive data binding
- Analytics dashboard with live charts and usage statistics
- Advanced settings panel with configuration validation
- Notification system with toast messages and action buttons
- Multi-language support with localization framework

## 🚀 Getting Started

### Prerequisites
- .NET 6.0 SDK or later
- Visual Studio 2022 or VS Code
- Windows (for Windows Forms and WPF examples)

### Configuration

Before running any example, you need to configure your AuthMe credentials:

1. Open the example project you want to run
2. Locate the configuration section in the main file:
   - `ConsoleApp/Program.cs`
   - `WindowsFormsApp/MainForm.cs`
   - `WpfApp/MainWindow.xaml.cs`

3. Replace the placeholder values:
```csharp
var config = new AuthMeConfig
{
    AppId = "••••••••••••••••",                  // Replace with your App ID from dashboard
    AppSecret = "••••••••••••••••••••••••••••••••",            // Replace with your App Secret from dashboard
    AppName = "••••••••••••••••",                 // Replace with your application name
    CacheDurationSeconds = 3, // Short cache for security
    HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
    DiscordSupport = "discord.gg/server" // Your support server
};
```

### Running the Examples

#### Console Application
```bash
cd Examples/ConsoleApp
dotnet run
```

#### Windows Forms Application
```bash
cd Examples/WindowsFormsApp
dotnet run
```

#### WPF Application
```bash
cd Examples/WpfApp
dotnet run
```

## 🔧 Integration Guide

### Basic Integration Steps

1. **Add NuGet Package**
```xml
<PackageReference Include="KeyAuth.NET" Version="1.0.0" />
```

2. **Configure KeyAuth**
```csharp
var config = new AuthMeConfig
{
    AppId = "••••••••••••••••",                  // Replace with your App ID from dashboard
    AppSecret = "••••••••••••••••••••••••••••••••",            // Replace with your App Secret from dashboard
    AppName = "••••••••••••••••",                 // Replace with your application name
    CacheDurationSeconds = 3, // Short cache for security
    HwidMethod = HwidMethod.Comprehensive, // Hardware ID method
    DiscordSupport = "discord.gg/server" // Your support server
};
```

3. **Initialize Client**
```csharp
using var authMe = new AuthMeClient(config);
```

4. **Validate License**
```csharp
var result = await authMe.ValidateKeyAsync(licenseKey);
if (result.IsValid)
{
    // Grant access to protected features
    var keyData = result.KeyData;
    Console.WriteLine($"Welcome! Tier: {keyData.Tier}, Expires: {keyData.ExpiresAt}");
}
```

### Common Patterns

#### Simple Validation
```csharp
var result = await authMe.ValidateKeyAsync("LICENSE-KEY");
if (result.IsValid)
{
    // Success - enable protected features
    var keyData = result.KeyData;
    MessageBox.Show($"Welcome! Your {keyData.Tier} license is valid until {keyData.ExpiresAt}");
}
else
{
    // Failure - show error message
    MessageBox.Show($"License validation failed: {result.Message}");
    
    // Handle security violations
    if (result.IsSecurityViolation)
    {
        MessageBox.Show("Security violation detected. Contact support.", "Security Alert", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
```

#### Authentication with UI Feedback
```csharp
var authResult = await authMe.AuthenticateAsync("LICENSE-KEY", showValidationDetails: true);
if (authResult.IsSuccess)
{
    // Show license information
    var keyData = authResult.KeyData;
    Console.WriteLine($"🎉 Welcome! Your {keyData.Tier} license expires: {keyData.ExpiresAt}");
    Console.WriteLine($"📊 Usage: {keyData.UsageCount}/{(keyData.IsUnlimited ? "∞" : keyData.MaxUses.ToString())}");
    
    // Access analytics if enabled
    if (config.EnableAnalytics)
    {
        var analytics = await authMe.GetAnalyticsAsync();
        Console.WriteLine($"📈 Total validations today: {analytics.TodayValidations}");
    }
}
```

#### Security Violation Handling
```csharp
if (!result.IsValid && result.IsSecurityViolation)
{
    MessageBox.Show(
        "🚨 SECURITY ALERT: Your license has been revoked due to suspicious activity.\n" +
        "This may be due to:\n" +
        "• Using the license on multiple devices\n" +
        "• Hardware ID mismatch\n" +
        "• Unusual usage patterns\n\n" +
        $"Contact support at {config.DiscordSupport} if this is an error.",
        "Security Violation Detected",
        MessageBoxButtons.OK,
        MessageBoxIcon.Warning
    );
    
    // Log security violation for monitoring
    await authMe.LogSecurityViolationAsync(result.ErrorCode, result.Message);
}
```

## 🛡️ Security Best Practices

### 1. Configuration Security
- Store AuthMe credentials securely (environment variables, config files, Azure Key Vault)
- Use different App IDs and secrets for development and production environments
- Never hardcode credentials in source code - use configuration management
- Implement credential rotation policies for enhanced security
- Use encrypted configuration files for sensitive data

### 2. Error Handling
- Always handle validation failures gracefully with user-friendly messages
- Provide clear, actionable error messages to users
- Log security violations for monitoring and analytics
- Implement retry logic for transient network failures
- Use structured logging for better debugging and monitoring
- Handle rate limiting scenarios appropriately

### 3. UI/UX Considerations
- Show validation progress to users with progress bars or spinners
- Provide clear success/failure feedback with appropriate icons and colors
- Include support contact information and helpful troubleshooting steps
- Implement toast notifications for non-blocking feedback
- Use consistent UI patterns across different validation states
- Provide contextual help and tooltips for complex features

### 4. Performance
- Use caching appropriately (default 3 seconds is recommended for security)
- Handle network timeouts gracefully with exponential backoff
- Test connection before validation if needed, especially in offline scenarios
- Implement connection pooling for high-throughput applications
- Use async/await patterns consistently for better performance
- Monitor and optimize API call patterns to reduce unnecessary requests
- Implement circuit breaker patterns for resilient network communication

## 📱 Platform-Specific Notes

### Console Applications
- Perfect for CLI tools and background services
- Minimal UI overhead
- Easy to integrate into existing console apps

### Windows Forms
- Great for traditional Windows desktop applications
- Rich control library
- Easy event-driven programming model

### WPF
- Modern Windows applications with rich UI
- Data binding and MVVM support
- Scalable vector graphics and animations

## 🔍 Troubleshooting

### Common Issues

1. **Invalid Configuration Error**
   - Check AuthMe App ID and App Secret are correct (format: app_xxxxxxxx and sk_xxxxxxxx)
   - Ensure API URL includes protocol (http:// or https://)
   - Verify App Name matches your dashboard configuration

2. **Connection Timeout**
   - Check internet connection and firewall settings
   - Verify AuthMe service is accessible and running
   - Increase timeout in configuration if needed
   - Check for proxy or VPN interference

3. **Hardware ID Issues**
   - Try different HWID methods if detection fails (Basic, Comprehensive, Custom)
   - Check Windows Management Instrumentation (WMI) service is running
   - Verify administrator privileges if using comprehensive HWID method
   - Test hardware ID generation with GetHardwareInfo() method

4. **License Validation Failures**
   - Verify license key format and validity (AUTHME-TIER-xxxxxxxxxx)
   - Check if key has been revoked, expired, or reached usage limit
   - Ensure hardware ID binding is correct
   - Verify application is active in AuthMe dashboard
   - Check rate limiting status

5. **Security Violations**
   - Review hardware ID binding settings
   - Check for multiple device usage
   - Verify license key hasn't been shared
   - Contact support for false positive violations

6. **Analytics and Caching Issues**
   - Clear cache using ClearCache() method if needed
   - Check analytics permissions in dashboard
   - Verify network connectivity for real-time features


---

**Happy coding with AuthMe .NET SDK!** 🚀

## 🆕 What's New in AuthMe

### Recent Features Added:
- **Analytics Integration**: Real-time usage statistics and validation analytics
- **Enhanced Security**: Advanced security violation detection and reporting
- **Improved Caching**: Smart caching with performance optimization
- **Better Error Handling**: More detailed error codes and user-friendly messages
- **Modern UI Examples**: Updated examples with contemporary design patterns
- **Configuration Management**: Enhanced configuration options with validation
- **Multi-tier Support**: Better support for Basic, Premium, and Enterprise tiers
- **Webhook Integration**: Real-time event notifications for security and usage
- **Rate Limiting**: Built-in rate limiting with graceful degradation
- **Connection Resilience**: Improved network handling with retry logic

### Coming Soon:
- **Mobile SDK Support**: React Native and Flutter integrations
- **Advanced Analytics**: Machine learning-powered usage insights
- **SSO Integration**: Single sign-on with popular identity providers
- **Blockchain Licensing**: Decentralized license verification options
