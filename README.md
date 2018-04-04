Dapper University
================

University management system for administering students, courses, departments,
and instructors. Made with reference to the Contoso University sample
application but with Dapper (micro ORM) in lieu of EF Core.  This is mostly
complete but the instructor features are still in progress

Technology
----------
* ASP.NET Core
* PostgreSQL (with Dapper)
* Bootstrap 4 + Material
* Flyway

Screenshots
---------
### About
![about](/screenshots/about.png?raw=true "About")

### Students - Index (pagination, sorting, and searching is supported)
![students](/screenshots/students.png?raw=true "Students")

### Courses - Edit
![courses](/screenshots/courses.png?raw=true "Courses")

### Departments - Details
![departments](/screenshots/departments.png?raw=true "Departments")

### Instructors - Index (instructor features are in progress)
![instructors](/screenshots/instructors.png?raw=true "Instructors")

Run
---
If you have docker installed,
```
docker-compose build
docker-compose up
Go to http://localhost:5000
```

Otherwise, create a database, run the migration scripts (located in
`./Migrations`), and edit `appsettings.json` so that the connection string
points to your server. You can ignore the identity connection string (this
feature is not yet implemented).

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
