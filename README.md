# Student Club Management System 

A centralized web-based platform designed to bridge the gap between university students and campus clubs. This application allows students to discover activities, while providing club presidents with tools to manage their organizations efficiently.

## Overview
University campus life is often fragmented across various social media and messaging apps. This project provides a unified hub for event discovery, club memberships, and community management.

## Tech Stack
- **Backend:** ASP.NET Core 8.0 MVC
- **ORM:** Entity Framework Core (Code-First)
- **Database:** SQLite
- **Security:** ASP.NET Core Identity (Role-Based Authorization)
- **Frontend:** Bootstrap 5, HTML5, CSS3, JavaScript

## Key Features

### For Students (Standard Users)
- **Centralized Discovery:** Browse all active clubs and upcoming campus events in one place.
- **Membership Management:** Join clubs and attend events with a single click.
- **Personal Dashboard:** A custom profile to track joined clubs and registered events.
- **Profile Customization:** Manage personal details and security settings.

### For Club Administrators
- **Club Creation:** Users can establish a new club and automatically receive administrative rights.
- **Event Management:** Create, update, and manage events specific to the club.
- **Member Tracking:** Access real-time lists of club members and event attendees.
- **Dynamic Dashboard:** A specialized management interface embedded within the club's detail page.

## Database Architecture
- **Identity System:** Customized `ApplicationUser` model with extended profile fields.
- **Relational Data:** Implemented Many-to-Many relationships for Club Memberships and Event Attendees.
- **Data Seeding:** Includes automated seeding for initial clubs and events to facilitate testing.

## Getting Started
1. Clone the repository.
2. Ensure you have the .NET 9 SDK installed.
3. Run `dotnet restore` to install dependencies.
4. Execute `dotnet run` to start the application.
5. Log in with the test credentials provided in the documentation.
