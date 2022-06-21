IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Branches] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [Abbreviation] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Branches] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Classes] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Classes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DefaultPackages] (
    [Id] int NOT NULL IDENTITY,
    [NumberOfSessions] int NOT NULL,
    [Price] float NOT NULL,
    [Months] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_DefaultPackages] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Trainers] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Trainers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Members] (
    [Id] int NOT NULL IDENTITY,
    [FullName] nvarchar(max) NOT NULL,
    [NormalizedFullName] nvarchar(max) NOT NULL,
    [PhoneNumber] nvarchar(max) NOT NULL,
    [Birthdate] datetime2 NULL,
    [BranchId] int NOT NULL,
    [UserId] nvarchar(max) NOT NULL,
    [UserName] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Members] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Members_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Schedules] (
    [Id] int NOT NULL IDENTITY,
    [Song] nvarchar(max) NULL,
    [OpeningDate] datetime2 NOT NULL,
    [StartTime] time NOT NULL,
    [DaysPerWeek] nvarchar(max) NOT NULL,
    [BranchId] int NOT NULL,
    [ClassId] int NOT NULL,
    [TrainerId] int NOT NULL,
    [TotalSessions] int NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Schedules] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Schedules_Branches_BranchId] FOREIGN KEY ([BranchId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Schedules_Classes_ClassId] FOREIGN KEY ([ClassId]) REFERENCES [Classes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Schedules_Trainers_TrainerId] FOREIGN KEY ([TrainerId]) REFERENCES [Trainers] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Memberships] (
    [MemberId] int NOT NULL,
    [RemainingSessions] int NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Memberships] PRIMARY KEY ([MemberId]),
    CONSTRAINT [FK_Memberships_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Packages] (
    [Id] int NOT NULL IDENTITY,
    [MemberId] int NOT NULL,
    [NumberOfSessions] int NOT NULL,
    [Price] float NOT NULL,
    [Months] int NOT NULL,
    [DefaultPackageId] int NULL,
    [BranchRegisteredId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Packages] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Packages_Branches_BranchRegisteredId] FOREIGN KEY ([BranchRegisteredId]) REFERENCES [Branches] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Packages_DefaultPackages_DefaultPackageId] FOREIGN KEY ([DefaultPackageId]) REFERENCES [DefaultPackages] ([Id]),
    CONSTRAINT [FK_Packages_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Sessions] (
    [Id] int NOT NULL IDENTITY,
    [Date] datetime2 NOT NULL,
    [Number] int NOT NULL,
    [ScheduleId] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Sessions_Schedules_ScheduleId] FOREIGN KEY ([ScheduleId]) REFERENCES [Schedules] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Registrations] (
    [Id] int NOT NULL IDENTITY,
    [SessionId] int NOT NULL,
    [MemberId] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [UpdatedDate] datetime2 NOT NULL,
    [CreatedBy] nvarchar(max) NULL,
    [UpdatedBy] nvarchar(max) NULL,
    CONSTRAINT [PK_Registrations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Registrations_Members_MemberId] FOREIGN KEY ([MemberId]) REFERENCES [Members] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Registrations_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Sessions] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_Members_BranchId] ON [Members] ([BranchId]);
GO

CREATE INDEX [IX_Packages_BranchRegisteredId] ON [Packages] ([BranchRegisteredId]);
GO

CREATE INDEX [IX_Packages_DefaultPackageId] ON [Packages] ([DefaultPackageId]);
GO

CREATE INDEX [IX_Packages_MemberId] ON [Packages] ([MemberId]);
GO

CREATE INDEX [IX_Registrations_MemberId] ON [Registrations] ([MemberId]);
GO

CREATE INDEX [IX_Registrations_SessionId] ON [Registrations] ([SessionId]);
GO

CREATE INDEX [IX_Schedules_BranchId] ON [Schedules] ([BranchId]);
GO

CREATE INDEX [IX_Schedules_ClassId] ON [Schedules] ([ClassId]);
GO

CREATE INDEX [IX_Schedules_TrainerId] ON [Schedules] ([TrainerId]);
GO

CREATE INDEX [IX_Sessions_ScheduleId] ON [Sessions] ([ScheduleId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20220621140102_V00001', N'6.0.4');
GO

COMMIT;
GO

