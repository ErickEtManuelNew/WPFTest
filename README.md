# WPF Content Management System

A modern WPF application built with .NET 8.0 following the MVVM architectural pattern.

## Technologies Used

- .NET 8.0
- WPF (Windows Presentation Foundation)
- Entity Framework Core with SQL Server
- MaterialDesignThemes for UI
- CommunityToolkit.Mvvm for MVVM implementation
- BCrypt for security
- Mailtrap/Gmail for email services

## Features

- User Authentication and Authorization
- Email Verification System
- User Management
- Article Management
- Material Design UI
- Database Integration
- Email Notifications

## Project Structure

```
├── Views/                 # XAML views and code-behind
├── ViewModels/           # MVVM view models
├── Models/               # Domain models
├── Services/             # Business logic services
├── Data/                # Data access layer
├── Converters/          # XAML value converters
└── Infrastructure/      # Cross-cutting concerns
```

## Getting Started

### Prerequisites

- .NET 8.0 SDK
- SQL Server
- Visual Studio 2022 or later (recommended)

### Configuration

1. Update the connection string in `appsettings.json`
2. Configure email settings in `appsettings.json`
3. Run Entity Framework migrations:
   ```
   dotnet ef database update
   ```

## Development

This project follows the MVVM pattern with dependency injection and uses modern .NET features.
