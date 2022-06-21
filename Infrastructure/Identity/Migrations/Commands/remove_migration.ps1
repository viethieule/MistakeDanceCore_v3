dotnet ef migrations remove -p .\Infrastructure\ -c ApplicationIdentityDbContext -v
Remove-Item .\Infrastructure\Identity\Scripts\V00001.sql
Remove-Item .\Infrastructure\Identity\Scripts\V00001-V00000-downgrade.sql
Remove-Item .\Infrastructure\Identity\Scripts\V00001-V00002-upgrade.sql