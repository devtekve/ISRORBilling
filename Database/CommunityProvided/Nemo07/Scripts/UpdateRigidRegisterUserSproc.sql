USE [SILKROAD_R_ACCOUNT]
GO
/****** Object:  StoredProcedure [dbo].[_Rigid_Register_User]    Script Date: 11/5/2022 5:35:34 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER procedure [dbo].[_Rigid_Register_User] 
	@szUserID	varchar(128),
	@szPassword	varchar(32),
	@szEmail	varchar(255),
	@szEmailCertificationStatus	varchar(1),
	@szEmailUniqueStatus	varchar(1),
	@szCountry	varchar(2),
	@szRegistrantIP varchar(15),
	--@szVIPUserType int,
	--@szVIPLv int,
	@szVipExpireTime varchar(20)
AS

SET NOCOUNT ON

BEGIN TRANSACTION

	DECLARE @UserJID INT
	DECLARE @RegDate DATETIME
	DECLARE @RegBinIP VARBINARY(15)
	DECLARE @RegCCfromDB VARCHAR(2)
	--DECLARE @JoiningPath NVARCHAR(32)

	SET @RegDate = GETDATE()
	SET @RegBinIP = [GB_JoymaxPortal].[dbo].[F_MakeIPStringToIPBinary] (@szRegistrantIP)

	IF (EXISTS(SELECT * FROM [GB_JoymaxPortal].[dbo].[MU_User] WHERE UserID = @szUserID))
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -1000 AS 'ReturnCode'
		RETURN -1000
	END

	IF (EXISTS(SELECT * FROM [GB_JoymaxPortal].[dbo].[MU_Email] WHERE EmailAddr = @szEmail))
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -2000 AS 'ReturnCode'
		RETURN -2000
	END

	IF (EXISTS(SELECT * FROM [TB_User] WHERE StrUserID = @szUserID))
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -2001 AS 'ReturnCode'
		RETURN -2001
	END

	INSERT INTO [GB_JoymaxPortal].[dbo].[MU_User] (UserID, UserPwd, Gender, Birthday, NickName, CountryCode)
	VALUES(@szUserID, @szPassword, 'M', @RegDate, @szUserID, @szCountry)
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -3000 AS 'ReturnCode'
		RETURN -3000
	END

	SET @UserJID = @@IDENTITY

	/*TB_User insert move next to MU_User insert*/

		-- VIP 유저 관련
	-- 
	-- Based on new/returning/temporary member VIPLevel/grant period
	SET @szVipExpireTime = Convert(varchar(10),DateADD(day, 8 , getdate()), 121) + ' 00:00:00'


	INSERT INTO TB_User (PortalJID, StrUserID, ServiceCompany, [password], Active, UserIP, CountryCode, VisitDate, RegDate, sec_primary, sec_content, sec_grade, AccPlayTime, LatestUpdateTime_ToPlayTime, Email, EmailCertificationStatus, EmailUniqueStatus, VIPUserType, VIPLv, VipExpireTime)
	VALUES (@UserJID, @szUserID, 11, @szPassword, 1, @szRegistrantIP, @szCountry, GETDATE(), @RegDate, 3, 3, 0, 0, 0, @szEmail, @szEmailCertificationStatus, @szEmailUniqueStatus , 2, 1, @szVipExpireTime)
	
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -7000 AS 'ReturnCode'
		RETURN -7000
	END

	INSERT INTO [GB_JoymaxPortal].[dbo].[MUH_AlteredInfo] (JID, AlterationDate, LastName, FirstName, EmailAddr, EmailReceptionStatus, EmailCertificationStatus, UserIP, CountryCode, NickName, ATypeCode, CountryCodeChangingStatus)
	VALUES(@UserJID, @RegDate, @szUserID, @szUserID, @szEmail, 'Y', 'Y', @RegBinIP, @szCountry, @szUserID, 1, 'N')
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -4000 AS 'ReturnCode'
		RETURN -4000
	END
	
	INSERT INTO [GB_JoymaxPortal].[dbo].[APH_ChangedSilk] (InvoiceID, PTInvoiceID, ManagerGiftID, JID, RemainedSilk, ChangedSilk, SilkType, SellingTypeID, ChangeDate, AvailableDate, AvailableStatus)
	VALUES (NULL, NULL, NULL, @UserJID, 0, 0, 3, 2, @RegDate, DATEADD(YEAR, 10, @RegDate), 'N')
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -5000 AS 'ReturnCode'
		RETURN -5000
	END
	
	INSERT INTO [GB_JoymaxPortal].[dbo].[MU_Email] (JID, EmailAddr) VALUES(@UserJID, @szEmail)
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -6000 AS 'ReturnCode'
		RETURN -6000
	END

	INSERT INTO [GB_JoymaxPortal].[dbo].[AUH_AgreedService] ( JID, ServiceCode, StartDate, EndDate, UserIP ) 
	VALUES ( @UserJID, 11, @RegDate, '9999-12-31 00:00:00', @RegBinIP )
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -6001 AS 'ReturnCode'
		RETURN -6001
	END
	
	INSERT INTO [GB_JoymaxPortal].[dbo].[MU_JoiningInfo] ( JID, UserIP, JoiningDate, CountryCode, JoiningPath )
	VALUES ( @UserJID, @RegBinIP, @RegDate, @szCountry, 'JOYMAX' )
	IF (@@ERROR <> 0 OR @@ROWCOUNT <> 1)
	BEGIN
		ROLLBACK TRANSACTION
		SELECT -6002 AS 'ReturnCode'
		RETURN -6002
	END

COMMIT TRANSACTION