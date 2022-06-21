BEGIN TRANSACTION;
GO

DROP TABLE [AspNetRoleClaims];
GO

DROP TABLE [AspNetUserClaims];
GO

DROP TABLE [AspNetUserLogins];
GO

DROP TABLE [AspNetUserRoles];
GO

DROP TABLE [AspNetUserTokens];
GO

DROP TABLE [JwtRefreshTokens];
GO

DROP TABLE [AspNetRoles];
GO

DROP TABLE [AspNetUsers];
GO

DELETE FROM [__EFMigrationsHistory]
WHERE [MigrationId] = N'20220621142907_V00001';
GO

COMMIT;
GO

