using System;
using System.Windows.Forms;
using AuthMe.NET;
using AuthMe.NET.Models;

namespace AuthMe.Examples.WindowsForms
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Configure AuthMe in Program.cs (replace placeholders with your real credentials)
            var config = new AuthMeConfig
            {
                AppId = "YOUR_APP_ID_HERE",           // Get from AuthMe Dashboard → Applications → Your App → App ID
                AppSecret = "YOUR_APP_SECRET_HERE",   // Get from AuthMe Dashboard → Applications → Your App → App Secret
                AppName = "Your Application Name",     // Your application's display name
                CacheDurationSeconds = 3,               // Keep cache short for security (default is 3)
                HwidMethod = HwidMethod.Comprehensive,
                DiscordSupport = "https://discord.gg/your-server",
            };

            // Validate before constructing the client to avoid constructor exceptions
            if (!config.IsValid())
            {
                MessageBox.Show(
                    "Invalid AuthMe configuration.\n\n" +
                    "Please set AppId (app_...) and AppSecret (sk_...) in Program.cs.",
                    "AuthMe Configuration",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return;
            }

            using var authClient = new AuthMeClient(config);
            Application.Run(new MainForm(authClient));
        }
    }
}
