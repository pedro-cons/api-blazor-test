## The project structure follows an organized layered architecture, divided as follows:

1 - Presentation:
- Api
- Api.Tests
- Web
- Web.Tests
2 - Application:
- Application
- Application.Tests
3 - Domain:
- Domain
- Domain.Tests
4 - Repository:
- Repository
- Repository.Tests
5 - Configuration:
- Configuration

##Setup Instructions
To ensure the proper functioning of the system, follow these steps:

- Install Necessary Packages:
Before running the migrations, make sure you have the required NuGet packages installed. Navigate to each project (especially the Repository layer):

- Run Migrations:
After installing the packages, you need to apply the migrations to configure and update the database. Run the following command in the Package Manager Console or terminal:

Update-Database
This command will apply any pending migrations and ensure that your database schema is up to date.

Run Applications Simultaneously:
To run the system correctly, you need to start both applications in the Presentation layer (Api and Web) simultaneously. This can typically be done by opening two terminal windows or using your IDE's built-in capabilities to run multiple projects.

For example, if using Visual Studio, you can set both projects to start by right-clicking on the solution, selecting Properties, and setting the startup projects under the Startup Project section.

##Final Note
Following these steps will ensure that the migrations are executed properly, and both applications are running together, allowing the system to function as intended.

