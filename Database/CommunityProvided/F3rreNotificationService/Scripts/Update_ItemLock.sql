USE [SILKROAD_R_ACCOUNT]
GO

/****** Object:  StoredProcedure [dbo].[Update_ItemLock]    Script Date: 23.10.2022 21:26:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER OFF
GO

CREATE PROCEDURE [dbo].[Update_ItemLock]	/*-- If you already have it just use "ALTER" instead of CREATE --*/
    @JID INT
	,@Email VARCHAR(max)
	,@LockPw VARCHAR(max)
AS
SET NOCOUNT ON

DECLARE @ReturnCode INT

IF (
		EXISTS (
			SELECT *
			FROM TB_User
			WHERE JID = @JID
			)
		)
BEGIN
	IF (
			EXISTS (
				SELECT *
				FROM TB_User
				WHERE JID = JID
					AND Email = @Email
				)
			)
BEGIN
		SET @ReturnCode = 0

UPDATE TB_User
SET ItemLockPW = @LockPw
WHERE JID = @JID

    GOTO RETURN_PROC
END
ELSE
BEGIN
		SET @ReturnCode = - 1

		GOTO RETURN_PROC
END
END
ELSE
BEGIN
	SET @ReturnCode = - 1

	GOTO RETURN_PROC
END

RETURN_PROC:

SELECT ISNULL(@ReturnCode, - 1)
