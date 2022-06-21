dotnet ef migrations add Init -p .\Persistence\ -c MistakeDanceDbContext -v
dotnet ef migrations script -p .\Persistence\ -c MistakeDanceDbContext -o .\Persistence\Scripts\V00001.sql -v
dotnet ef migrations script V00001 0 -p .\Persistence\ -c MistakeDanceDbContext -o .\Persistence\Scripts\V00001-V00000-downgrade.sql -v
dotnet ef migrations script V00001 V00002 -p .\Persistence\ -c MistakeDanceDbContext -o .\Persistence\Scripts\V00001-V00002-upgrade.sql -v