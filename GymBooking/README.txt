Add-Migration InitialCreate
Update-Database


Example (User-Secret):
dotnet user-secrets set "DbPassword" "pass123"

https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json

C:\Users\Peter\AppData\Roaming\Microsoft\UserSecrets

