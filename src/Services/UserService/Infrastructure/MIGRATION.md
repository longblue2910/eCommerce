 ## Powershell Commands:
- cd src/Services/UserService
- dotnet ef migrations add InitialCreate --project Infrastructure --startup-project API --output-dir Migrations
- Ngắn gọn: dotnet ef migrations add InitialCreate -p Infrastructure -s API -o Migrations

- dotnet ef database update -p Infrastructure -s API

