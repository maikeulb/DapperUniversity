Dapper University
================

University management system for administering students, courses, departments,
and instructors. Made with reference to the Contoso University sample
application but with Dapper (micro ORM) in lieu of EF Core.  This is mostly
complete but the instructor features are still in progress

Technology
----------
* ASP.NET CORE 2.0
* PostgreSQL
* Flyway
* NLog
* Dapper
* Bootstrap 4 + Material

Screenshots
---------
### About
Displays student body statistics.
![about](/screenshots/about.png?raw=true "About")

### Students - Index
Pagination, sorting, and searching is supported.
![students](/screenshots/students.png?raw=true "Students")

### Courses - Edit
![courses](/screenshots/courses.png?raw=true "Courses")

### Departments - Details
![departments](/screenshots/departments.png?raw=true "Departments")

### Instructors - Index 
Instructors are assigned to courses (office and course assignments are in progress)
![instructors](/screenshots/instructors.png?raw=true "Instructors")

Run
---
If you have docker installed,
```
docker-compose build
docker-compose up
Go to http://localhost:5000
```

Alternatively, you will need the .NET Core 2.0 SDK. If you have the SDK
installed then create a database named 'DapperUniversity', run the migration
scripts (located in `./Migrations`), and edit `appsettings.json` so that the
connection string points to your server. You can ignore the identity connection
string (this feature is not yet implemented).

After all that has been taken care of,
```
dotnet restore
dotnet run
Go to http://localhost:5000
```

TODO
----
Finish instructor feature  
Implement Identity (using Dapper)
