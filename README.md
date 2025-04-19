# Tripify 

[![.NET Foundation](https://img.shields.io/badge/.NET%20Foundation-blueviolet.svg)](https://www.dotnetfoundation.org/)
[![ASP.NET MVC](https://img.shields.io/badge/ASP.NET_MVC-7.0.20-blueviolet)](https://dotnet.microsoft.com/apps/aspnet/mvc)
[![SQL Server](https://img.shields.io/badge/SQL_Server-2020-red)](https://www.microsoft.com/sql-server)
![License](https://img.shields.io/badge/License-MIT-blue)

## About

**Tripify** is a web application for travel planning, built on **ASP.NET MVC** with **SQL Server**. The project provides an intuitive interface for searching, booking and managing trips.  

## Key Features  
- Tour and route search functionality  
- Ticket and hotel booking system  
- User account management  
- Secure authentication using ASP.NET Identity  
- Admin panel for content management  

## Quick Start  

### 1. Clone the Repository  
```bash
git clone https://github.com/your-username/Tripify.git
cd Tripify
```

### 2. Database Setup  
You can restore the database using either:  
- **BACPAC file**: [Download from Google Drive][bacpacGD]  
- **SQL script**: [Download from Google Drive][sqlScriptGD]  

Update the connection string in `appsettings.json`:  
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=TripifyDB;Trusted_Connection=True;TrustServerCertificate=True"
}
```

### 3. Run the Application  
Open the solution in **Visual Studio** and run the project (F5).  

> **Note**: Entity Framework migrations will be added in future updates.

 ## NuGet packets
- microsoft.aspnetcore.mvc.razor.runtimecompilation
- microsoft.entityframeworkcore
- microsoft.entityframeworkcore.sqlserver
- microsoft.entityframeworkcore.tools


[bacpacGD]: https://drive.google.com/file/d/...  
[sqlScriptGD]: https://drive.google.com/file/d/...  
