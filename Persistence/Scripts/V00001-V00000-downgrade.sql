BEGIN TRANSACTION;
GO

DROP TABLE [Memberships];
GO

DROP TABLE [Packages];
GO

DROP TABLE [Registrations];
GO

DROP TABLE [DefaultPackages];
GO

DROP TABLE [Members];
GO

DROP TABLE [Sessions];
GO

DROP TABLE [Schedules];
GO

DROP TABLE [Branches];
GO

DROP TABLE [Classes];
GO

DROP TABLE [Trainers];
GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20220621140102_V00001';
GO

COMMIT;
GO

