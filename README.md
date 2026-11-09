# Programming-RaceDay-Parts 1,2 and 3 ReadMe
RaceDay is a full-stack event management system designed for road running, walking and cycling events in South Africa. The system allows organisers to create and manage events, categories, participant enrolments and results and participants can register for events and track their own enrolments and results.

USER ROLES

ORGANISER
Organisers can:
- Create, edit and delete events
- Manage event categories
- View participant enrolments for their events
- Capture participant results

PARTICIPANT
Participants can:
- Create an account and log in
- Browse available events
- Enter an event by selecting a category
- View their own event enrolments
- Track their personal results

PART 1 DOCUMENTATION

- [ERD](docs/ProgERD.drawio(1).png).
- [API Endpoint Plan](docs/ProgPart1APIPLANNING.pdf)
- [SQL Database Script](docs/RaceDay.sql)

CONTINUOUS INTERGRATION

GitHub Actions are used to validate the project repository.

<img width="1881" height="1025" alt="Screenshot_4-9-2026_84827_github com" src="https://github.com/user-attachments/assets/e449075e-313c-4468-aa2a-0b4dfb560376" />


YOUTUBE LINK

An unlisted YouTube video explaining the RaceDay planning documents, ERD decisions and API endpoint choices will be provided here.

*YouTube:* https://youtu.be/NcccnE_zzok?si=GODWBdN3KophQbeF

RaceDay – Part 2

Project Overview

RaceDay is an ASP.NET Core Web API developed using C# and .NET 8 to manage sporting events, categories, participant enrolments, results, and user profiles.

Technologies Used

- C# and ASP.NET Core Web API
- Entity Framework Core
- SQL Server LocalDB
- JWT authentication and role-based authorisation
- Swagger / OpenAPI
- Visual Studio 2022
- Git and GitHub

Features Implemented

- User registration and login using JWT authentication.
- Event creation, viewing, updating, and deletion.
- Category management linked to events.
- Participant enrolment and cancellation.
- Result management linked to enrolments.
- User profile viewing and updating.
- Role-based access and ownership checks for protected operations.
- Database access through Entity Framework Core.

API Controllers

- AuthController: registration, login, and protected access.
- EventsController: event management.
- CategoriesController: category management.
- EnrollmentsController: participant enrolments.
- ResultsController: result management.
- UsersController: profile management.

Database

The API uses SQL Server LocalDB and Entity Framework Core. The "RaceDayContext" class provides access to the database entities.

Testing

Swagger is used to explore and test API endpoints, including public requests and authenticated operations.

Repository Documentation

The "docs" folder contains the ERD, Part 1 API planning document, and SQL script. The repository also includes a GitHub Actions workflow for automated build checks.

Video Demonstration

YouTube link: https://youtu.be/TdkqYZ40Ew8?si=pQ4k5RXaDeVNELhG


RaceDay Part 3

1. Project Overview

RaceDay is a web-based Race Event Management System developed using ASP.NET Core. It allows Organisers to manage race events, categories, enrolments and results, while Participants can browse events, enrol and view their results.

The solution consists of two projects:

- RaceDay: ASP.NET Core Web API responsible for database operations and business logic.
- RaceDay.Web: ASP.NET Core MVC application that provides the user interface and communicates with the API.

2. Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- Microsoft SQL Server
- SQL Server LocalDB
- HTML, CSS and Bootstrap
- JavaScript
- JSON Web Tokens (JWT)
- Visual Studio 2022
- Git and GitHub

3. Main Features

Participants:

- Register, log in and log out.
- Browse available events.
- Enrol in events and select categories.
- View and cancel enrolments, subject to validation.
- View recorded results.

Organisers:

- Log in and access the Organiser dashboard.
- Create, edit and delete their own events.
- Create event categories.
- View participant enrolments.
- Record and manage race results.

4. Project Structure

RaceDay (API)

- Controllers – Handle API requests.
- Models – Represent database entities.
- DTOs – Define data transferred through API requests and responses.
- Data – Contains RaceDayContext for database access.
- Program.cs – Configures services and application middleware.

RaceDay.Web (MVC)

- Controllers – Handle website requests.
- Models – Contain view models and request models.
- Services – Contains RaceDayApiService for API communication.
- Views – Contain the website pages.
- wwwroot – Stores static files such as CSS, JavaScript and images.

5. Database and Configuration

The application uses Microsoft SQL Server LocalDB with a database named "RaceDayDB".

The API uses Entity Framework Core through RaceDayContext.

The MVC application communicates with the API using the BaseUrl configured in "appsettings.json".


6. Authentication and Authorisation

The application uses JSON Web Tokens (JWT) to authenticate users. The MVC application stores the authentication token and user information in the session.

Role-based authorisation separates Participant and Organiser functionality. The API validates access to protected operations and checks Organiser ownership of events.

7. Video Demonstration

The YouTube demonstration explains the project structure, main classes, services and controllers followed by a practical demonstration of the application.

YouTube Link: https://youtu.be/e3KT2OLnCc8?si=Ngad-leyyWkAzj4o


