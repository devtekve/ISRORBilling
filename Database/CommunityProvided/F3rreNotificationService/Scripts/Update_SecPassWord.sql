USE [SILKROAD_R_ACCOUNT]
GO
/****** Object:  StoredProcedure [dbo].[Update_SecPassWord]    Script Date: 11/16/2022 07:48:30 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER OFF
GO

ALTER PROCEDURE [dbo].[Update_SecPassWord]	/*-- If you already have it just use "ALTER" instead of CREATE --*/
    @JID INT
	,@Email VARCHAR(max)
	,@SecPassWord VARCHAR(max)
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
SET CodeSecPassWord = @SecPassWord
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
