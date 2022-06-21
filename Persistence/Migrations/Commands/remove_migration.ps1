dotnet ef migrations remove -p .\Persistence\ -c MistakeDanceDbContext -v
Remove-Item .\Persistence\Scripts\V00001.sql
Remove-Item .\Persistence\Scripts\V00001-V00000-downgrade.sql
Remove-Item .\Persistence\Scripts\V00001-V00002-upgrade.sql