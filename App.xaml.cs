using System;
using System.Configuration;
using System.IO;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WPFTest.Data;
using WPFTest.Models;
using WPFTest.Repositories;
using WPFTest.Services;
using WPFTest.ViewModels;
using WPFTest.Views;

namespace WPFTest
{
    public partial class App : Application
    {
        private IServiceProvider? _serviceProvider;
        private static IConfiguration? _configuration;
        public static IConfiguration Configuration
        {
            get => _configuration ?? throw new InvalidOperationException("Configuration not initialized");
            private set => _configuration = value;
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Determine environment
            var environment = "Development"; // Default to "Development"
            if (e.Args.Length > 0)
            {
                environment = e.Args[0];
            }

            // Load configuration based on environment
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            Configuration = builder.Build();

            base.OnStartup(e);

            var services = new ServiceCollection();
            ConfigureServices(services);

            _serviceProvider = services.BuildServiceProvider();

            var loginWindow = _serviceProvider.GetRequiredService<LoginWindow>();
            loginWindow.DataContext = _serviceProvider.GetRequiredService<LoginViewModel>();
            loginWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddSingleton<IConfiguration>(Configuration);

            // Database context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Email Settings
            var isProduction = Configuration.GetValue<bool>("IsProduction");
            var emailConfigSection = isProduction ? "Email:Gmail" : "Email:Mailtrap";
            var emailSettings = Configuration.GetSection(emailConfigSection).Get<EmailSettings>() ?? new EmailSettings
            {
                Host = isProduction ? "smtp.gmail.com" : "smtp.mailtrap.io",
                Port = isProduction ? 587 : 2525,
                EnableSsl = true,
                Username = Configuration.GetValue<string>($"{emailConfigSection}:Username") ?? "",
                Password = Configuration.GetValue<string>($"{emailConfigSection}:Password") ?? ""
            };
            
            services.AddSingleton(emailSettings);

            // Repositories and Unit of Work
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Enregistrer EmailService avec une factory
            services.AddTransient<IEmailService>(sp =>
            {
                var emailSettings = sp.GetRequiredService<EmailSettings>();
                return new EmailService(emailSettings, isProduction);
            });

            // Windows
            services.AddTransient<LoginWindow>();
            services.AddTransient<RegisterWindow>();
            services.AddTransient<VerifyWindow>();
            services.AddTransient<MainWindow>();
            services.AddTransient<AddArticleWindow>();
            services.AddTransient<AddUserWindow>();

            // ViewModels
            services.AddTransient(sp => new LoginViewModel(
                sp.GetRequiredService<IUnitOfWork>(),
                sp));

            services.AddTransient(sp => new RegisterViewModel(
                sp.GetRequiredService<IUnitOfWork>(),
                sp.GetRequiredService<IEmailService>()));

            services.AddTransient(sp => new VerifyViewModel(
                sp.GetRequiredService<IUnitOfWork>()));

            services.AddTransient(sp => new MainViewModel(sp));

            services.AddTransient(sp => new ArticlesViewModel(
                sp.GetRequiredService<IUnitOfWork>(),
                null));

            services.AddTransient(sp => new UsersViewModel(
                sp.GetRequiredService<IUnitOfWork>(),
                sp,
                null));

            services.AddTransient(sp => new AddArticleViewModel());
            services.AddTransient(sp => new AddUserViewModel(
                sp.GetRequiredService<IUnitOfWork>()
                ));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (_serviceProvider is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}
