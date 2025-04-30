How to Run the Project
To run this project, you will need Visual Studio 2022 and Docker Desktop. Follow these steps:
1.	Select the docker-compose startup item in Visual Studio.
2.	Run the project.
The application uses MS SQL as its database. The database will automatically update itself when the application starts for the first time.
Running Tests
To run the tests:
1.	Right-click on the TodoApp.Tests project in Visual Studio.
2.	Select Run Tests.
---
Known Limitations
1.	Time Zone Handling:
The application does not account for time zones. A real-world application should handle time zones properly.
2.	Hardcoded Database Credentials:
The project contains hardcoded database credentials and lacks separate Development/Production configurations. However, for a project of this scope, this was deemed unnecessary.
3.	Database Migration Workaround:
Due to issues with the Update-Database CLI command, a workaround script (ApplicationDbContextFactory.cs) was added. However, this script is not required to start the application, as the database updates automatically on startup.
---
