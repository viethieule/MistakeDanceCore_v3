dotnet ef migrations add V00001 -p .\Infrastructure\ -c ApplicationIdentityDbContext -o .\Identity\Migrations -v
dotnet ef migrations script -p .\Infrastructure\ -c ApplicationIdentityDbContext -o .\Infrastructure\Identity\Scripts\V00001.sql -v
dotnet ef migrations script V00001 0 -p .\Infrastructure\ -c ApplicationIdentityDbContext -o .\Infrastructure\Identity\Scripts\V00001-V00000-downgrade.sql -v
dotnet ef migrations script V00001 V00002 -p .\Infrastructure\ -c ApplicationIdentityDbContext -o .\Infrastructure\Identity\Scripts\V00001-V00002-upgrade.sql -v