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
![about](/screenshots/main.png?raw=true "Main")

### Students (Index)
![students](/screenshots/wall.png?raw=true "Wall")

### Courses (Edit)
![courses](/screenshots/profile.png?raw=true "Profile")

### Departments (Details)
![departments](/screenshots/profile.png?raw=true "Profile")

### Instructors (Index)
![instructors](/screenshots/profile.png?raw=true "Profile")

Run
---
If you have docker installed,
```
docker-compose build
docker-compose up
Go to http://localhost:5000
```

Otherwise, create a database, run the migration scripts, and edit
`appsettings.json` so that the connection string points to your server. You can
ignore the identity connection string (this feature is not yet implemented).

After all that has been taken care of,
```
dotnet build
dotnet run
Go to http://localhost:5000
```

TODO
----
* Finish instructor feature
* Implement Identity (using Dapper)
