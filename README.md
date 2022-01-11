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

# Identity

## 5. Migrations addIdentity

5.1 To create new Tables in DB: Roles, RoleClaims, Users, UserClaims,... => `dotnet ef migrations add addIdentity`

5.2 But tableName has prefix "AspNet" that we don't want, so remove migrations and custome it: `dotnet ef migrations remove`

5.3 Migration again and update

```
dotnet ef migrations add addIdentity
dotnet ef database update
```

## 6. Migrations updateUser

```
dotnet ef migrations add updateUser
dotnet ef database update
```

## 8. Setup Identity UI Default

8.1 Use AddDefaultIdentity for Identity UI Default

8.2 Create \_LoginPartial.cshtml (Default Layout)

8.3 Create /Areas/Identity/Pages/\_ViewStart.cshtml to use \_Layout.cshtml

## 10. Generate code Identity Razor Pages

`dotnet aspnet-codegenerator identity -dc RazorEF.Models.MyBlogContext`

## 11. Generate configs for Google & Facebook Login

Google: https://console.cloud.google.com/ \
Facebook: https://developers.facebook.com/apps \
Refs: https://xuanthulab.net/asp-net-razor-su-dung-tai-khoan-google-de-xac-thuc-trong-identity.html
