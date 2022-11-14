CREATE OR ALTER PROCEDURE [dbo].[Update_ItemLock] 
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
