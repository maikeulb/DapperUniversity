FROM boxfuse/flyway
COPY . /app
WORKDIR /app
RUN chmod +x ./Migrations/entrypoint.sh
CMD /bin/bash ./Migrations/entrypoint.sh

FROM microsoft/aspnetcore-build:2.0
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80/tcp
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ConnectionStrings__DapperUniversity="Server=db;Port=5432;Database=DapperUniversity;User ID=postgres;Password=P@ssw0rd!;"
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
