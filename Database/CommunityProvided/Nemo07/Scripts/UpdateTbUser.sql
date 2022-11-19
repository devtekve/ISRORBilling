USE SILKROAD_R_ACCOUNT;
ALTER TABLE [dbo].[TB_User] ADD [Email] VARCHAR(50);
ALTER TABLE [dbo].[TB_User] ADD [EmailCertificationStatus] VARCHAR(1);
ALTER TABLE [dbo].[TB_User] ADD [EmailUniqueStatus] VARCHAR(1);
ALTER TABLE [dbo].[TB_User] ADD [VIPUserType] INT;
ALTER TABLE [dbo].[TB_User] ADD [VIPLv] INT;
ALTER TABLE [dbo].[TB_User] ADD [VipExpireTime] datetime;
ALTER TABLE [dbo].[TB_User] ADD [ItemLockPW] VARCHAR(max);