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
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908191932_InitialCreate'
)
BEGIN
    CREATE TABLE [GuildApplications] (
        [Id] int NOT NULL IDENTITY,
        [DiscordId] nvarchar(max) NOT NULL,
        [Age] int NULL,
        [Country] nvarchar(max) NOT NULL,
        [WarcraftLogsLink] nvarchar(max) NOT NULL,
        [Tech] nvarchar(max) NOT NULL,
        [Schedule] nvarchar(max) NOT NULL,
        [Splits] bit NULL,
        [Communication] nvarchar(max) NOT NULL,
        [Screenshot] nvarchar(max) NOT NULL,
        [Vouch] nvarchar(max) NULL,
        [Goals] bit NULL,
        [TimestampUtc] datetime2 NOT NULL,
        [DiscordMessageId] nvarchar(max) NULL,
        [LastAttemptedUtc] datetime2 NULL,
        [Retry] int NOT NULL,
        [Status] int NOT NULL,
        [PostStatus] int NOT NULL,
        CONSTRAINT [PK_GuildApplications] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908191932_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908191932_InitialCreate', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201403_AddOmmitedHistoryColumn'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [History] nvarchar(max) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908201403_AddOmmitedHistoryColumn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908201403_AddOmmitedHistoryColumn', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var nvarchar(max);
    SELECT @var = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'WarcraftLogsLink');
    IF @var IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [WarcraftLogsLink] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var1 nvarchar(max);
    SELECT @var1 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Vouch');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var1 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Vouch] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var2 nvarchar(max);
    SELECT @var2 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Tech');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var2 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Tech] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var3 nvarchar(max);
    SELECT @var3 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Screenshot');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var3 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Screenshot] nvarchar(100) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var4 nvarchar(max);
    SELECT @var4 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Schedule');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var4 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Schedule] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var5 nvarchar(max);
    SELECT @var5 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'History');
    IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var5 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [History] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var6 nvarchar(max);
    SELECT @var6 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'DiscordMessageId');
    IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var6 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [DiscordMessageId] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var7 nvarchar(max);
    SELECT @var7 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'DiscordId');
    IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var7 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [DiscordId] nvarchar(50) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var8 nvarchar(max);
    SELECT @var8 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Country');
    IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var8 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Country] nvarchar(30) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    DECLARE @var9 nvarchar(max);
    SELECT @var9 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Communication');
    IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var9 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Communication] nvarchar(150) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260908202611_SpecifyMaxLengthColumns'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260908202611_SpecifyMaxLengthColumns', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909011445_AddBlogCreatedTimestamp'
)
BEGIN
    DECLARE @var10 nvarchar(max);
    SELECT @var10 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Splits');
    IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var10 + ';');
    EXEC(N'UPDATE [GuildApplications] SET [Splits] = CAST(0 AS bit) WHERE [Splits] IS NULL');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Splits] bit NOT NULL;
    ALTER TABLE [GuildApplications] ADD DEFAULT CAST(0 AS bit) FOR [Splits];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909011445_AddBlogCreatedTimestamp'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909011445_AddBlogCreatedTimestamp', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260909012029_DTOkeychanges'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260909012029_DTOkeychanges', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924232618_AddNewObjectsDtosAndMappings'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [WowClass] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924232618_AddNewObjectsDtosAndMappings'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [WowSpecs] nvarchar(max) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260924232618_AddNewObjectsDtosAndMappings'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260924232618_AddNewObjectsDtosAndMappings', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var11 nvarchar(max);
    SELECT @var11 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Age');
    IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var11 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Age];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var12 nvarchar(max);
    SELECT @var12 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Communication');
    IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var12 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Communication];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var13 nvarchar(max);
    SELECT @var13 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Country');
    IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var13 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Country];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var14 nvarchar(max);
    SELECT @var14 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Vouch');
    IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var14 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Vouch];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var15 nvarchar(max);
    SELECT @var15 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'WarcraftLogsLink');
    IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var15 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [WarcraftLogsLink];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var16 nvarchar(max);
    SELECT @var16 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'WowClass');
    IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var16 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [WowClass];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var17 nvarchar(max);
    SELECT @var17 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'WowSpecs');
    IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var17 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [WowSpecs];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var18 nvarchar(max);
    SELECT @var18 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Tech');
    IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var18 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Tech] nvarchar(1500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var19 nvarchar(max);
    SELECT @var19 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'History');
    IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var19 + ';');
    EXEC(N'UPDATE [GuildApplications] SET [History] = N'''' WHERE [History] IS NULL');
    ALTER TABLE [GuildApplications] ALTER COLUMN [History] nvarchar(1500) NOT NULL;
    ALTER TABLE [GuildApplications] ADD DEFAULT N'' FOR [History];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    DECLARE @var20 nvarchar(max);
    SELECT @var20 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Goals');
    IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var20 + ';');
    EXEC(N'UPDATE [GuildApplications] SET [Goals] = CAST(0 AS bit) WHERE [Goals] IS NULL');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Goals] bit NOT NULL;
    ALTER TABLE [GuildApplications] ADD DEFAULT CAST(0 AS bit) FOR [Goals];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [About] nvarchar(1500) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [AltName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [AltRealm] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [AltRole] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [Extra] nvarchar(1500) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [MainName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [MainRealm] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [MainRole] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260926165056_BackFrontDataRestructure'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260926165056_BackFrontDataRestructure', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927230222_CorrectEntityLengths'
)
BEGIN
    DECLARE @var21 nvarchar(max);
    SELECT @var21 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Tech');
    IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var21 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Tech] nvarchar(2000) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927230222_CorrectEntityLengths'
)
BEGIN
    DECLARE @var22 nvarchar(max);
    SELECT @var22 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'History');
    IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var22 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [History] nvarchar(2000) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927230222_CorrectEntityLengths'
)
BEGIN
    DECLARE @var23 nvarchar(max);
    SELECT @var23 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Extra');
    IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var23 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Extra] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927230222_CorrectEntityLengths'
)
BEGIN
    DECLARE @var24 nvarchar(max);
    SELECT @var24 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'About');
    IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var24 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [About] nvarchar(2000) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260927230222_CorrectEntityLengths'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260927230222_CorrectEntityLengths', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004105521_ReaddCountryField'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [Country] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004105521_ReaddCountryField'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261004105521_ReaddCountryField', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004141459_RemoveDeprecatedScheduleProperty'
)
BEGIN
    DECLARE @var25 nvarchar(max);
    SELECT @var25 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Schedule');
    IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var25 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Schedule];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261004141459_RemoveDeprecatedScheduleProperty'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261004141459_RemoveDeprecatedScheduleProperty', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006164150_RemoveSplitsAndGoalsProperties'
)
BEGIN
    DECLARE @var26 nvarchar(max);
    SELECT @var26 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Goals');
    IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var26 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Goals];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006164150_RemoveSplitsAndGoalsProperties'
)
BEGIN
    DECLARE @var27 nvarchar(max);
    SELECT @var27 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Splits');
    IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var27 + ';');
    ALTER TABLE [GuildApplications] DROP COLUMN [Splits];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006164150_RemoveSplitsAndGoalsProperties'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006164150_RemoveSplitsAndGoalsProperties', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006213357_IntroduceGUID'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [PublicId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261006213357_IntroduceGUID'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261006213357_IntroduceGUID', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007102540_AddUniqueKeyMarkerToGuid'
)
BEGIN
    CREATE UNIQUE INDEX [IX_GuildApplications_PublicId] ON [GuildApplications] ([PublicId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261007102540_AddUniqueKeyMarkerToGuid'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261007102540_AddUniqueKeyMarkerToGuid', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011173024_LastPostErrorcolumn'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [LastPostError] nvarchar(250) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261011173024_LastPostErrorcolumn'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261011173024_LastPostErrorcolumn', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017184054_ScreenshotLengthUpdate'
)
BEGIN
    DECLARE @var28 nvarchar(max);
    SELECT @var28 = QUOTENAME([d].[name])
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[GuildApplications]') AND [c].[name] = N'Screenshot');
    IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [GuildApplications] DROP CONSTRAINT ' + @var28 + ';');
    ALTER TABLE [GuildApplications] ALTER COLUMN [Screenshot] nvarchar(500) NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261017184054_ScreenshotLengthUpdate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261017184054_ScreenshotLengthUpdate', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020183425_AddDiscordNamefield'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [DiscordName] nvarchar(50) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020183425_AddDiscordNamefield'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020183425_AddDiscordNamefield', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020201656_BackfillDiscordNames'
)
BEGIN
    UPDATE GuildApplications SET DiscordName = DiscordId WHERE DiscordName = ''
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261020201656_BackfillDiscordNames'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261020201656_BackfillDiscordNames', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022154930_UniqueKeyToThreadId'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [DiscordThreadId] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022154930_UniqueKeyToThreadId'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_GuildApplications_DiscordThreadId] ON [GuildApplications] ([DiscordThreadId]) WHERE [DiscordThreadId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261022154930_UniqueKeyToThreadId'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261022154930_UniqueKeyToThreadId', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030163013_AddLastOfficerMessageUtc'
)
BEGIN
    ALTER TABLE [GuildApplications] ADD [LastOfficerMessagedUtc] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030163013_AddLastOfficerMessageUtc'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261030163013_AddLastOfficerMessageUtc', N'10.0.13');
END;

COMMIT;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030183402_RenameLastOfficerMessaged'
)
BEGIN
    EXEC sp_rename N'[GuildApplications].[LastOfficerMessagedUtc]', N'LastRecruitmentTeamMessagedUtc', 'COLUMN';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261030183402_RenameLastOfficerMessaged'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261030183402_RenameLastOfficerMessaged', N'10.0.13');
END;

COMMIT;
GO

