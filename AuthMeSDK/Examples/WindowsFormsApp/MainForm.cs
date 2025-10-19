    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using AuthMe.NET;
    using AuthMe.NET.Models;

    namespace AuthMe.Examples.WindowsForms
    {
        public partial class MainForm : Form
        {
            private readonly AuthMeClient _authMeClient;
            private bool _isAuthenticated = false;

            // UI Controls
            private TextBox _licenseKeyTextBox = null!;
            private Button _validateButton = null!;
            private Button _clearButton = null!;
            private RichTextBox _outputTextBox = null!;
            private Label _statusLabel = null!;
            private Panel _protectedPanel = null!;
            private Label _welcomeLabel = null!;
            private Label? licenseLabel;
            private Label? outputLabel;
            private Button? hwInfoButton;
            private Label _licenseInfoLabel = null!;

            public MainForm(AuthMeClient authMeClient)
            {
                _authMeClient = authMeClient ?? throw new ArgumentNullException(nameof(authMeClient));
                InitializeComponent();
                // Set initial status using provided client
                _statusLabel.Text = $"✅ AuthMe initialized - HWID: {_authMeClient.HardwareId[..8]}...";
                _statusLabel.ForeColor = Color.Green;
                AppendOutput("✅ AuthMe client initialized successfully!");
                AppendOutput($"Hardware ID: {_authMeClient.HardwareId}");
                AppendOutput("");
            }

            private void InitializeComponent()
            {
                licenseLabel = new Label();
                _licenseKeyTextBox = new TextBox();
                _validateButton = new Button();
                _clearButton = new Button();
                _statusLabel = new Label();
                outputLabel = new Label();
                _outputTextBox = new RichTextBox();
                _protectedPanel = new Panel();
                _welcomeLabel = new Label();
                _licenseInfoLabel = new Label();
                hwInfoButton = new Button();
                _protectedPanel.SuspendLayout();
                SuspendLayout();
                // 
                // licenseLabel
                // 
                licenseLabel.AutoSize = true;
                licenseLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
                licenseLabel.Location = new Point(20, 20);
                licenseLabel.Name = "licenseLabel";
                licenseLabel.Size = new Size(75, 15);
                licenseLabel.TabIndex = 0;
                licenseLabel.Text = "License Key:";
                // 
                // _licenseKeyTextBox
                // 
                _licenseKeyTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
                _licenseKeyTextBox.Location = new Point(20, 45);
                _licenseKeyTextBox.Name = "_licenseKeyTextBox";
                _licenseKeyTextBox.PlaceholderText = "Enter your license key (e.g., ABCD-1234-EFGH-5678)";
                _licenseKeyTextBox.Size = new Size(350, 22);
                _licenseKeyTextBox.TabIndex = 1;
                _licenseKeyTextBox.TextChanged += _licenseKeyTextBox_TextChanged;
                // 
                // _validateButton
                // 
                _validateButton.BackColor = Color.FromArgb(0, 120, 215);
                _validateButton.FlatStyle = FlatStyle.Flat;
                _validateButton.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
                _validateButton.ForeColor = Color.White;
                _validateButton.Location = new Point(380, 45);
                _validateButton.Name = "_validateButton";
                _validateButton.Size = new Size(90, 25);
                _validateButton.TabIndex = 2;
                _validateButton.Text = "Validate";
                _validateButton.UseVisualStyleBackColor = false;
                _validateButton.Click += ValidateButton_Click;
                // 
                // _clearButton
                // 
                _clearButton.BackColor = Color.FromArgb(108, 117, 125);
                _clearButton.FlatStyle = FlatStyle.Flat;
                _clearButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                _clearButton.ForeColor = Color.White;
                _clearButton.Location = new Point(480, 45);
                _clearButton.Name = "_clearButton";
                _clearButton.Size = new Size(75, 25);
                _clearButton.TabIndex = 3;
                _clearButton.Text = "Clear";
                _clearButton.UseVisualStyleBackColor = false;
                _clearButton.Click += ClearButton_Click;
                // 
                // _statusLabel
                // 
                _statusLabel.AutoSize = true;
                _statusLabel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                _statusLabel.Location = new Point(20, 85);
                _statusLabel.Name = "_statusLabel";
                _statusLabel.Size = new Size(39, 15);
                _statusLabel.TabIndex = 4;
                _statusLabel.Text = "Ready";
                // 
                // outputLabel
                // 
                outputLabel.AutoSize = true;
                outputLabel.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point);
                outputLabel.Location = new Point(20, 120);
                outputLabel.Name = "outputLabel";
                outputLabel.Size = new Size(50, 15);
                outputLabel.TabIndex = 5;
                outputLabel.Text = "Output:";
                // 
                // _outputTextBox
                // 
                _outputTextBox.BackColor = Color.Black;
                _outputTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
                _outputTextBox.ForeColor = Color.White;
                _outputTextBox.Location = new Point(20, 145);
                _outputTextBox.Name = "_outputTextBox";
                _outputTextBox.ReadOnly = true;
                _outputTextBox.Size = new Size(535, 200);
                _outputTextBox.TabIndex = 6;
                _outputTextBox.Text = "";
                // 
                // _protectedPanel
                // 
                _protectedPanel.BackColor = Color.FromArgb(248, 249, 250);
                _protectedPanel.BorderStyle = BorderStyle.FixedSingle;
                _protectedPanel.Controls.Add(_welcomeLabel);
                _protectedPanel.Controls.Add(_licenseInfoLabel);
                _protectedPanel.Location = new Point(20, 360);
                _protectedPanel.Name = "_protectedPanel";
                _protectedPanel.Size = new Size(535, 80);
                _protectedPanel.TabIndex = 7;
                _protectedPanel.Visible = false;
                // 
                // _welcomeLabel
                // 
                _welcomeLabel.AutoSize = true;
                _welcomeLabel.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point);
                _welcomeLabel.ForeColor = Color.FromArgb(40, 167, 69);
                _welcomeLabel.Location = new Point(10, 10);
                _welcomeLabel.Name = "_welcomeLabel";
                _welcomeLabel.Size = new Size(206, 19);
                _welcomeLabel.TabIndex = 0;
                _welcomeLabel.Text = "🎉 Authentication Successful!";
                // 
                // _licenseInfoLabel
                // 
                _licenseInfoLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
                _licenseInfoLabel.Location = new Point(10, 35);
                _licenseInfoLabel.Name = "_licenseInfoLabel";
                _licenseInfoLabel.Size = new Size(515, 40);
                _licenseInfoLabel.TabIndex = 1;
                _licenseInfoLabel.Text = "License information will appear here...";
                // 
                // hwInfoButton
                // 
                hwInfoButton.BackColor = Color.FromArgb(40, 167, 69);
                hwInfoButton.FlatStyle = FlatStyle.Flat;
                hwInfoButton.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
                hwInfoButton.ForeColor = Color.White;
                hwInfoButton.Location = new Point(480, 80);
                hwInfoButton.Name = "hwInfoButton";
                hwInfoButton.Size = new Size(75, 25);
                hwInfoButton.TabIndex = 8;
                hwInfoButton.Text = "HW Info";
                hwInfoButton.UseVisualStyleBackColor = false;
                hwInfoButton.Click += HwInfoButton_Click;
                // 
                // MainForm
                // 
                AutoScaleDimensions = new SizeF(7F, 15F);
                AutoScaleMode = AutoScaleMode.Font;
                BackColor = Color.White;
                ClientSize = new Size(584, 461);
                Controls.Add(licenseLabel);
                Controls.Add(_licenseKeyTextBox);
                Controls.Add(_validateButton);
                Controls.Add(_clearButton);
                Controls.Add(_statusLabel);
                Controls.Add(outputLabel);
                Controls.Add(_outputTextBox);
                Controls.Add(_protectedPanel);
                Controls.Add(hwInfoButton);
                FormBorderStyle = FormBorderStyle.FixedSingle;
                MaximizeBox = false;
                Name = "MainForm";
                StartPosition = FormStartPosition.CenterScreen;
                Text = "KeyAuth .NET Windows Forms Example";
                Load += MainForm_Load;
                _protectedPanel.ResumeLayout(false);
                _protectedPanel.PerformLayout();
                ResumeLayout(false);
                PerformLayout();
            }

            

            private async void ValidateButton_Click(object? sender, EventArgs e)
            {
                if (_authMeClient == null)
                {
                    MessageBox.Show("AuthMe client is not initialized!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var licenseKey = _licenseKeyTextBox.Text.Trim();
                if (string.IsNullOrEmpty(licenseKey))
                {
                    MessageBox.Show("Please enter a license key!", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                _validateButton.Enabled = false;
                _validateButton.Text = "Validating...";
                _statusLabel.Text = "Validating license key...";
                _statusLabel.ForeColor = Color.Blue;
                _outputTextBox.Clear();
                _protectedPanel.Visible = false;

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
                        AppendOutput("🎉 Authentication successful!", Color.Green);
                        _statusLabel.Text = "✅ License validated successfully";
                        _statusLabel.ForeColor = Color.Green;
                        _isAuthenticated = true;

                        // Show protected content
                        ShowProtectedContent(result.KeyData);
                    }
                    else
                    {
                        // Show specific error messages based on the failure reason
                        _isAuthenticated = false;

                        // Check for global ban first
                        bool isGloballyBanned = result.Message.Contains("Your IP/device combination has been banned", StringComparison.OrdinalIgnoreCase);

                        if (isGloballyBanned)
                        {
                            AppendOutput("🚨 GLOBAL BAN DETECTED", Color.Red);
                            AppendOutput("═══════════════════════", Color.Red);
                            AppendOutput("🚫 Your IP address and device have been globally banned.", Color.Red);
                            AppendOutput("");
                            AppendOutput("This affects ALL license keys from your current location/device.", Color.Red);
                            AppendOutput("");

                            // Extract ban reason from message
                            var reasonStart = result.Message.IndexOf("Reason: ") + 8;
                            var reasonEnd = result.Message.IndexOf(".", reasonStart);
                            if (reasonStart > 7 && reasonEnd > reasonStart)
                            {
                                var banReason = result.Message.Substring(reasonStart, reasonEnd - reasonStart);
                                AppendOutput($"📝 Ban Reason: {banReason}", Color.Red);
                                AppendOutput("");
                            }

                            // Check if there's also a key-specific issue
                            if (result.Message.Contains("Additionally, this license key issue:"))
                            {
                                AppendOutput("Additionally, this specific license key has issues:", Color.Orange);
                                AppendOutput("");
                            }

                            _statusLabel.Text = "🚨 Globally banned";
                            _statusLabel.ForeColor = Color.Red;
                        }

                        if (result.KeyData != null)
                        {
                            var keyData = result.KeyData;

                            if (keyData.IsRevoked)
                            {
                                if (!isGloballyBanned)
                                {
                                    AppendOutput("🚫 Your license key has been revoked.", Color.Red);
                                    _statusLabel.Text = "🚫 License revoked";
                                    _statusLabel.ForeColor = Color.Red;
                                }
                                else
                                {
                                    AppendOutput("🚫 License key status: REVOKED", Color.Red);
                                }

                                AppendOutput("");
                                AppendOutput("This typically happens due to:", Color.Red);
                                AppendOutput("• Violation of terms of service", Color.Red);
                                AppendOutput("• Chargeback or payment dispute", Color.Red);
                                AppendOutput("• Unauthorized key sharing", Color.Red);
                                AppendOutput("");

                                // Extract key-specific reason if it's a combined message
                                if (isGloballyBanned && result.Message.Contains("Additionally, this license key issue:"))
                                {
                                    var keyReasonStart = result.Message.IndexOf("Additionally, this license key issue: ") + 38;
                                    var keyReasonEnd = result.Message.IndexOf(".", keyReasonStart);
                                    if (keyReasonStart > 37 && keyReasonEnd > keyReasonStart)
                                    {
                                        var keyReason = result.Message.Substring(keyReasonStart, keyReasonEnd - keyReasonStart);
                                        AppendOutput($"📝 Revocation Reason: {keyReason}", Color.Red);
                                    }
                                }
                                else
                                {
                                    AppendOutput($"📝 Revocation Reason: {result.Message}", Color.Red);
                                }
                            }
                            else if (keyData.IsSuspended)
                            {
                                if (!isGloballyBanned)
                                {
                                    AppendOutput("⏸️ Your license key has been suspended.", Color.Orange);
                                    _statusLabel.Text = "⏸️ License suspended";
                                    _statusLabel.ForeColor = Color.Orange;
                                }
                                else
                                {
                                    AppendOutput("⏸️ License key status: SUSPENDED", Color.Orange);
                                }

                                AppendOutput("");
                                AppendOutput("This is usually temporary and may be due to:", Color.Orange);
                                AppendOutput("• Pending payment verification", Color.Orange);
                                AppendOutput("• Account review in progress", Color.Orange);
                                AppendOutput("• Temporary security hold", Color.Orange);
                                AppendOutput("");

                                // Extract key-specific reason if it's a combined message
                                if (isGloballyBanned && result.Message.Contains("Additionally, this license key issue:"))
                                {
                                    var keyReasonStart = result.Message.IndexOf("Additionally, this license key issue: ") + 38;
                                    var keyReasonEnd = result.Message.IndexOf(".", keyReasonStart);
                                    if (keyReasonStart > 37 && keyReasonEnd > keyReasonStart)
                                    {
                                        var keyReason = result.Message.Substring(keyReasonStart, keyReasonEnd - keyReasonStart);
                                        AppendOutput($"📝 Suspension Reason: {keyReason}", Color.Orange);
                                    }
                                }
                                else
                                {
                                    AppendOutput($"📝 Suspension Reason: {result.Message}", Color.Orange);
                                }
                            }
                            else if (keyData.IsExpired)
                            {
                                AppendOutput("⏰ Your license key has expired.", Color.Orange);
                                AppendOutput($"📅 Expired on: {keyData.ExpiresAt?.ToString("yyyy-MM-dd")}", Color.Orange);
                                AppendOutput("");
                                AppendOutput("💡 To continue using this application, please renew your license.", Color.Blue);
                                _statusLabel.Text = "⏰ License expired";
                                _statusLabel.ForeColor = Color.Orange;
                            }
                            else if (keyData.IsUsageLimitReached)
                            {
                                AppendOutput("📊 Your license key usage limit has been reached.", Color.Orange);
                                AppendOutput($"📈 Usage: {keyData.UsageCount}/{keyData.MaxUses}", Color.Orange);
                                AppendOutput("");
                                AppendOutput("💡 Please upgrade your license for additional usage.", Color.Blue);
                                _statusLabel.Text = "📊 Usage limit reached";
                                _statusLabel.ForeColor = Color.Orange;
                            }
                            else
                            {
                                AppendOutput($"❌ License key validation failed: {result.Message}", Color.Red);
                                _statusLabel.Text = "❌ License validation failed";
                                _statusLabel.ForeColor = Color.Red;
                            }
                        }
                        else
                        {
                            AppendOutput($"❌ License key validation failed: {result.Message}", Color.Red);
                            _statusLabel.Text = "❌ License validation failed";
                            _statusLabel.ForeColor = Color.Red;
                        }

                        if (result.IsSecurityViolation)
                        {
                            AppendOutput("");
                            AppendOutput("🚨 SECURITY VIOLATION DETECTED:", Color.Red);
                            AppendOutput("• Key is being used on multiple devices", Color.Red);
                            AppendOutput("• Hardware ID spoofing detected", Color.Red);
                            AppendOutput("• Unauthorized key sharing", Color.Red);
                        }

                        // Always show support information
                        AppendOutput("");
                        AppendOutput("🆘 Need help? Contact support:", Color.Blue);
                        AppendOutput($"💬 Discord: {_authMeClient.Config.DiscordSupport}", Color.Blue);
                        AppendOutput("📧 If this is a mistake, please reach out to our support team.", Color.Blue);
                    }
                }
                catch (Exception ex)
                {
                    AppendOutput($"❌ Validation error: {ex.Message}", Color.Red);
                    _statusLabel.Text = "❌ Validation error occurred";
                    _statusLabel.ForeColor = Color.Red;
                    _isAuthenticated = false;
                }
                finally
                {
                    _validateButton.Enabled = true;
                    _validateButton.Text = "Validate License";
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
                            $"• Unlimited Usage";

                    _licenseInfoLabel.Text = info;
                }

                _protectedPanel.Visible = true;
            }

            private void ClearButton_Click(object? sender, EventArgs e)
            {
                _licenseKeyTextBox.Clear();
                _outputTextBox.Clear();
                _protectedPanel.Visible = false;
                _statusLabel.Text = "Ready";
                _statusLabel.ForeColor = Color.Blue;
                _isAuthenticated = false;
            }

            private void HwInfoButton_Click(object? sender, EventArgs e)
            {
                if (_authMeClient == null) return;

                var hwInfo = _authMeClient.GetHardwareInfo();
                var infoText = "Hardware Information:\n\n";
                foreach (var kvp in hwInfo)
                {
                    infoText += $"{kvp.Key}: {kvp.Value}\n";
                }

                MessageBox.Show(infoText, "Hardware Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            private void AppendOutput(string text, Color? color = null)
            {
                if (_outputTextBox.InvokeRequired)
                {
                    _outputTextBox.Invoke(new Action(() => AppendOutput(text, color)));
                    return;
                }

                _outputTextBox.SelectionStart = _outputTextBox.TextLength;
                _outputTextBox.SelectionLength = 0;
                _outputTextBox.SelectionColor = color ?? Color.LightGray;
                _outputTextBox.AppendText(text + Environment.NewLine);
                _outputTextBox.SelectionColor = _outputTextBox.ForeColor;
                _outputTextBox.ScrollToCaret();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    _authMeClient?.Dispose();
                }
                base.Dispose(disposing);
            }

            private void MainForm_Load(object? sender, EventArgs e)
            {

            }

            private void _licenseKeyTextBox_TextChanged(object? sender, EventArgs e)
            {

            }
        }
    }
