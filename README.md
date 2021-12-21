# Integrate Entity Framework into Razor Page

## 1. Install packages

```
dotnet new webapp

dotnet tool install --global dotnet-ef
dotnet tool install --global dotnet-aspnet-codegenerator

dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

## 2. Migrations

Create Models to update database

```
dotnet ef migrations list
dotnet ef migrations add initdb
dotnet ef database update
dotnet ef database drop -f
```

## 3. Auto create CRUD Pages for Article Model

`dotnet aspnet-codegenerator razorpage -m RazorEF.Models.Article -dc RazorEF.Models.MyBlogContext -outDir Pages/Blog -udl --referenceScriptLibraries`
