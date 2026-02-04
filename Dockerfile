# build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["src/ExamSystem.API/ExamSystem.API.csproj", "ExamSystem.API/"]
COPY ["src/ExamSystem.Application/ExamSystem.Application.csproj", "ExamSystem.Application/"]
COPY ["src/ExamSystem.Infrastructure/ExamSystem.Infrastructure.csproj", "ExamSystem.Infrastructure/"]
COPY ["src/ExamSystem.Domain/ExamSystem.Domain.csproj", "ExamSystem.Domain/"]

RUN dotnet restore "ExamSystem.API/ExamSystem.API.csproj"
COPY src/ .
RUN dotnet publish "ExamSystem.API/ExamSystem.API.csproj" -c Release -o /Publish

# runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Development
COPY --from=build /Publish .
EXPOSE 80
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "ExamSystem.API.dll"]
