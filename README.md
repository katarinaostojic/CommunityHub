# CommunityHub — Building & Neighborhood Management System

A desktop application for managing residential buildings and neighborhood-level civic coordination, developed as a project for the course **Software Specification and Modeling (RA)** at the Faculty of Technical Sciences, 2025/2026.

## Team
Nenad Veselinović

Katarina Ostojić

Tamara Rikanović

Nikola Stanojević

## Technology Stack

- **Language:** C#
- **Framework:** .NET 10
- **UI Framework:** WPF (Windows Presentation Foundation)
- **Architecture:** MVVM (Model-View-ViewModel), layered architecture (Domain / Application / UI)
- **Dependency Injection:** Manual DI via a custom `Injector` class
- **Persistence:** PostgreSQL via ADO.NET
- **Reporting:** QuestPDF (PDF report generation)
- **Version Control:** Git / GitHub

## Project Structure
CommunityHub/
├── src/
│ ├── CommunityHub.Application/ # Domain model, business logic, database access
│ │ ├── Database/
│ │ │ ├── Repositories/ # Repository classes (ADO.NET)
│ │ │ └── Scripts/ # database.sql, seed.sql
│ │ ├── Domain/ # Entities and business rules
│ │ └── appsettings.json # Database connection config
│ └── CommunityHub.Ui/ # WPF views, view models
└── CommunityHub.slnx


## How to Run

### Prerequisites

1. **.NET 10 SDK** installed
2. **PostgreSQL** installed and running on `localhost:5432`
3. A database named `communityhub` created
4. Schema and seed data loaded via **pgAdmin** by running `database.sql` and `seed.sql` (in that order)

### Running the Application

1. Open `CommunityHub.slnx` in Visual Studio
2. Set `CommunityHub.Ui` as the startup project
3. Run the application (`F5`)
4. Log in with a test user (see `seed.sql`)

## Features

The system supports four user roles: **Building Manager**, **Tenant**, **Neighborhood Coordinator**, and **Citizen**, each with a dedicated set of functionalities.

### Core Entities
- **Building** – address, neighborhood, location, floors, apartments, images
- **Common Room** – name, description, floor, rental type (single-day / multi-day)
- **Ad** – bulletin board post (offering/requesting help), category, date range, status
- **Problem Report** – description, priority, status, reporting tenant
- **Assembly** – scheduled resident meeting, topics, attendance tracking
- **Neighborhood** – budget categories, donations, community events

### My Contribution — Tenant Role
- **Common room rentals:** browse buildings, submit rental requests for shared facilities, view and manage request status
- **Bulletin board:** create and browse ads (offering/requesting help), match by category and date range, archive ads
- **Problem reporting:** report building issues with priority and description, track resolution status
- **Resident assembly attendance:** view scheduled assemblies, propose topics, confirm attendance
- **PDF export:** generate reports for tenant-facing data using QuestPDF

### Architecture Highlights
- Domain logic (validation, status transitions, availability checks) encapsulated in entity classes
- Clean separation between domain logic and UI-layer display formatting
- DTO layer for data exchange between Application and UI layers
- Manual dependency injection via a custom `Injector` class

## Common Errors & Solutions

### "Connection refused"
- **Cause:** PostgreSQL isn't running, or not on `localhost:5432`
- **Fix:** Start the PostgreSQL service or check the port in `appsettings.json`

### "Relation 'X' does not exist"
- **Cause:** `database.sql` hasn't been run
- **Fix:** Run `database.sql` via pgAdmin
