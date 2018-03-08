FROM boxfuse/flyway
COPY . /app
WORKDIR /app
RUN chmod +x ./Migrations/start.sh
CMD /bin/bash ./Migrations/start.sh

FROM microsoft/aspnetcore-build:2.0
COPY . /app
WORKDIR /app
RUN ["dotnet", "restore"]
RUN ["dotnet", "build"]
EXPOSE 80/tcp
ENV ASPNETCORE_ENVIRONMENT=Development
RUN chmod +x ./entrypoint.sh
CMD /bin/bash ./entrypoint.sh
