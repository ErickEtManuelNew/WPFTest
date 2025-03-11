using System;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Data;
using WPFTest.Models;
using WPFTest.Services;
using WPFTest.ViewModels;
using WPFTest.Views;

namespace WPFTest
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;
        private static IConfiguration? _configuration;
        public static IConfiguration Configuration 
        { 
            get => _configuration ?? throw new InvalidOperationException("Configuration not initialized");
            private set => _configuration = value;
        }

        public App()
        {
            // Configure services
            var services = new ServiceCollection();
            ConfigureServices(services);
            _serviceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext and Factory
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.AddSingleton<IDbContextFactory, DbContextFactory>();

            // Register Services
            services.AddSingleton<IDatabaseService, DatabaseService>();
            // Créez une instance de EmailService avec les paramètres extraits
            services.AddSingleton<IEmailService>(sp =>
            {
                // Extrayez les paramètres de configuration
                var isProduction = Configuration.GetValue<bool>("IsProduction");
                var emailSettings = isProduction
                    ? Configuration.GetSection("Email:Gmail").Get<EmailSettings>()
                    : Configuration.GetSection("Email:Mailtrap").Get<EmailSettings>();

                // Vérifiez si emailSettings est null
                if (emailSettings == null)
                {
                    throw new InvalidOperationException("Email settings not found in configuration.");
                }

                // Créez une instance de EmailService avec les paramètres extraits
                return new EmailService(emailSettings, isProduction);
            });

            // Register ViewModels
            services.AddTransient<LoginViewModel>(sp => 
                new LoginViewModel(
                    sp.GetRequiredService<IDatabaseService>(),
                    sp));
            services.AddTransient<RegisterViewModel>(sp => 
                new RegisterViewModel(
                    sp.GetRequiredService<IDatabaseService>(),
                    sp.GetRequiredService<IEmailService>()));
            services.AddTransient<VerifyViewModel>(sp => 
                new VerifyViewModel(sp.GetRequiredService<IDatabaseService>()));
            services.AddTransient<MainViewModel>();
            services.AddTransient<UsersViewModel>(sp => 
                new UsersViewModel(sp.GetRequiredService<IDatabaseService>(), sp, null));
            services.AddTransient<UserAccount>();
            services.AddTransient<ArticlesViewModel>();
            services.AddTransient<AddUserViewModel>(sp => 
                new AddUserViewModel(sp.GetRequiredService<IDatabaseService>()));
            services.AddTransient<AddArticleViewModel>();

            // Register Views
            services.AddTransient<MainWindow>();
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<VerifyWindow>();
            services.AddTransient<AddUserWindow>();
            services.AddTransient<AddArticleWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Déterminez l'environnement (par exemple, via une variable d'environnement ou un paramètre en ligne de commande)
            var environment = "Development"; // Par défaut, utilisez "Development"
            if (e.Args.Length > 0)
            {
                environment = e.Args[0]; // Permet de passer l'environnement en ligne de commande
            }

            // Chargez la configuration en fonction de l'environnement
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            // Vous pouvez maintenant accéder à la configuration dans toute l'application
            var isProduction = Configuration.GetValue<bool>("IsProduction");
            if (isProduction)
            {
                var gmailSettings = Configuration.GetSection("Email:Gmail").Get<EmailSettings>();
                if (gmailSettings != null)
                {
                    MessageBox.Show($"Production: Gmail Username: {gmailSettings.Username}, Password: {gmailSettings.Password}");
                }
            }
            else
            {
                var mailtrapSettings = Configuration.GetSection("Email:Mailtrap").Get<EmailSettings>();
                if (mailtrapSettings != null)
                {
                    MessageBox.Show($"Development: Mailtrap Username: {mailtrapSettings.Username}, Password: {mailtrapSettings.Password}");
                }
            }

            base.OnStartup(e);

            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            var loginViewModel = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginWindow.DataContext = loginViewModel;
            loginWindow.Show();
            MainWindow = loginWindow;
        }
    }

    public class EmailSettings
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}