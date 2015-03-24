USE [$(CS_DB_NAME_PROFILES)]
GO

/********************
	DROP INDEXES
********************/

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserObject]') AND name = N'Idx_UserObject_liveid')
	DROP INDEX [Idx_UserObject_liveid] ON [dbo].[UserObject]
GO


IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserObject]') AND name = N'Idx_UserObject_emailaddress')
	DROP INDEX [Idx_UserObject_emailaddress] ON [dbo].[UserObject]
GO

IF  EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserObject]') AND name = N'IX_UserObject_ExternalId')
	DROP INDEX [IX_UserObject_ExternalId] ON [dbo].[UserObject]
GO

/********************
	DROP COLUMNS
********************/

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userobject' AND COLUMN_NAME = 'u_live_id_make_unique')
BEGIN
   ALTER TABLE [dbo].[UserObject] DROP COLUMN u_live_id_make_unique
END

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userobject' AND COLUMN_NAME = 'u_live_id')
BEGIN
   ALTER TABLE [dbo].[UserObject] DROP COLUMN u_live_id
END


/********************
	ALTER COLUMNS
********************/

IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userobject' AND COLUMN_NAME = 'u_email_address')
BEGIN
   ALTER TABLE [dbo].[UserObject] ALTER COLUMN u_email_address nvarchar(64) NULL
END

/********************
	ADD NEW COLUMNS
********************/

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userobject' AND COLUMN_NAME = 'u_external_id')
BEGIN
	ALTER TABLE dbo.UserObject ADD u_external_id nvarchar(256) NOT NULL
END
GO

IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'userobject' AND COLUMN_NAME = 'u_comment')
BEGIN
	ALTER TABLE dbo.UserObject ADD u_comment nvarchar(256) NULL
END
GO

/********************
	ADD INDEXES
********************/

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[UserObject]') AND name = N'IX_UserObject_ExternalId')
CREATE UNIQUE NONCLUSTERED INDEX [IX_UserObject_ExternalId] ON [dbo].[UserObject]
(
	[u_external_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO