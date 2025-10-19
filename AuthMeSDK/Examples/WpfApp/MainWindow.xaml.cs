using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using AuthMe.NET;
using AuthMe.NET.Models;

namespace AuthMe.Examples.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private AuthMeClient? _authMeClient;
        private bool _isAuthenticated = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeAuthMe();
        }

        private void InitializeAuthMe()
        {
            try
            {
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

                if (!config.IsValid())
                {
                    StatusTextBlock.Text = "❌ Invalid AuthMe configuration";
                    StatusTextBlock.Foreground = Brushes.Red;
                    ValidateButton.IsEnabled = false;
                    AppendOutput("❌ Invalid configuration! Please update your credentials in MainWindow.xaml.cs");
                    return;
                }

                _authMeClient = new AuthMeClient(config);
                StatusTextBlock.Text = $"✅ AuthMe initialized - HWID: {_authMeClient.HardwareId[..8]}...";
                StatusTextBlock.Foreground = Brushes.Green;
            }
            catch (Exception ex)
            {
                StatusTextBlock.Text = $"❌ AuthMe initialization failed: {ex.Message}";
                StatusTextBlock.Foreground = Brushes.Red;
                ValidateButton.IsEnabled = false;
            }
        }

        private async void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            if (_authMeClient == null)
            {
                MessageBox.Show("AuthMe client is not initialized!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var licenseKey = LicenseKeyTextBox.Text.Trim();
            if (string.IsNullOrEmpty(licenseKey))
            {
                MessageBox.Show("Please enter a license key!", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ValidateButton.IsEnabled = false;
            ValidateButton.Content = "Validating...";
            StatusTextBlock.Text = "Validating license key...";
            StatusTextBlock.Foreground = Brushes.Blue;
            OutputTextBlock.Text = "";
            ProtectedContentBorder.Visibility = Visibility.Collapsed;

            try
            {
                // Show validation details in output
                AppendOutput("==================================================");
                AppendOutput("🔍 LICENSE KEY VALIDATION");
                AppendOutput("==================================================");
                AppendOutput($"License Key: {licenseKey}");
                AppendOutput($"Hardware ID: {_authMeClient.HardwareId}");
                AppendOutput($"HWID Method: {_authMeClient.Config.HwidMethod}");
                AppendOutput("");

                var result = await _authMeClient.ValidateKeyAsync(licenseKey);

                if (result.IsValid)
                {
                    AppendOutput("🎉 Authentication successful!");
                    StatusTextBlock.Text = "✅ License validated successfully";
                    StatusTextBlock.Foreground = Brushes.Green;
                    _isAuthenticated = true;

                    // Show protected content
                    ShowProtectedContent(result.KeyData);
                }
                else
                {
                    // Show specific error messages based on the failure reason
                    _isAuthenticated = false;

                    if (result.KeyData != null)
                    {
                        var keyData = result.KeyData;

                        if (keyData.IsExpired)
                        {
                            AppendOutput("⏰ Your license key has expired.");
                            AppendOutput($"📅 Expired on: {keyData.ExpiresAt?.ToString("yyyy-MM-dd")}");
                            AppendOutput("");
                            AppendOutput("💡 To continue using this application, please renew your license.");
                            StatusTextBlock.Text = "⏰ License expired";
                            StatusTextBlock.Foreground = Brushes.Orange;
                        }
                        else if (result.Message.Contains("revoked"))
                        {
                            AppendOutput("🚫 Your license key has been revoked.");
                            AppendOutput("");
                            AppendOutput("This typically happens due to:");
                            AppendOutput("• Violation of terms of service");
                            AppendOutput("• Chargeback or payment dispute");
                            AppendOutput("• Unauthorized key sharing");
                            StatusTextBlock.Text = "🚫 License revoked";
                            StatusTextBlock.Foreground = Brushes.Red;
                        }
                        else if (result.Message.Contains("suspended"))
                        {
                            AppendOutput("⏸️ Your license key has been suspended.");
                            AppendOutput("");
                            AppendOutput("This is usually temporary and may be due to:");
                            AppendOutput("• Pending payment verification");
                            AppendOutput("• Account review in progress");
                            AppendOutput("• Temporary security hold");
                            StatusTextBlock.Text = "⏸️ License suspended";
                            StatusTextBlock.Foreground = Brushes.Orange;
                        }
                        else if (keyData.IsUsageLimitReached)
                        {
                            AppendOutput("📊 Your license key usage limit has been reached.");
                            AppendOutput($"📈 Usage: {keyData.UsageCount}/{keyData.MaxUses}");
                            AppendOutput("");
                            AppendOutput("💡 Please upgrade your license for additional usage.");
                            StatusTextBlock.Text = "📊 Usage limit reached";
                            StatusTextBlock.Foreground = Brushes.Orange;
                        }
                        else
                        {
                            AppendOutput($"❌ License key validation failed: {result.Message}");
                            StatusTextBlock.Text = "❌ License validation failed";
                            StatusTextBlock.Foreground = Brushes.Red;
                        }
                    }
                    else
                    {
                        AppendOutput($"❌ License key validation failed: {result.Message}");
                        StatusTextBlock.Text = "❌ License validation failed";
                        StatusTextBlock.Foreground = Brushes.Red;
                    }

                    if (result.IsSecurityViolation)
                    {
                        AppendOutput("");
                        AppendOutput("🚨 SECURITY VIOLATION DETECTED:");
                        AppendOutput("• Key is being used on multiple devices");
                        AppendOutput("• Hardware ID spoofing detected");
                        AppendOutput("• Unauthorized key sharing");
                    }

                    // Always show support information
                    AppendOutput("");
                    AppendOutput("🆘 Need help? Contact support:");
                    AppendOutput($"💬 Discord: {_authMeClient.Config.DiscordSupport}");
                    AppendOutput("📧 If this is a mistake, please reach out to our support team.");
                }
            }
            catch (Exception ex)
            {
                AppendOutput($"❌ Validation error: {ex.Message}");
                StatusTextBlock.Text = "❌ Validation error occurred";
                StatusTextBlock.Foreground = Brushes.Red;
                _isAuthenticated = false;
            }
            finally
            {
                ValidateButton.IsEnabled = true;
                ValidateButton.Content = "Validate";
            }
        }

        private void ShowProtectedContent(LicenseKeyData? keyData)
        {
            if (keyData != null)
            {
                var info = $"License Information:\n" +
                          $"• Tier: {keyData.Tier.ToUpper()}\n" +
                          $"• Expires: {(keyData.ExpiresAt?.ToString("yyyy-MM-dd") ?? "Never")}\n" +
                          $"• Usage: {keyData.UsageCount}{(keyData.IsUnlimited ? " (unlimited)" : $"/{keyData.MaxUses}")}\n\n" +
                          $"Protected Features:\n" +
                          $"• Advanced Analytics\n" +
                          $"• Premium Support\n" +
                          $"• Unlimited Usage\n\n" +
                          $"Thank you for using our software!";

                LicenseInfoTextBlock.Text = info;
            }

            ProtectedContentBorder.Visibility = Visibility.Visible;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            LicenseKeyTextBox.Clear();
            OutputTextBlock.Text = "";
            ProtectedContentBorder.Visibility = Visibility.Collapsed;
            StatusTextBlock.Text = "Ready";
            StatusTextBlock.Foreground = Brushes.Blue;
            _isAuthenticated = false;
        }

        private void HardwareInfoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_authMeClient == null) return;

            var hwInfo = _authMeClient.GetHardwareInfo();
            var infoText = "Hardware Information:\n\n";
            foreach (var kvp in hwInfo)
            {
                infoText += $"{kvp.Key}: {kvp.Value}\n";
            }

            MessageBox.Show(infoText, "Hardware Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void AppendOutput(string text)
        {
            if (!string.IsNullOrEmpty(OutputTextBlock.Text))
            {
                OutputTextBlock.Text += Environment.NewLine;
            }
            OutputTextBlock.Text += text;
        }

        protected override void OnClosed(EventArgs e)
        {
            _authMeClient?.Dispose();
            base.OnClosed(e);
        }

        private void LicenseKeyTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {

        }
    }
}
