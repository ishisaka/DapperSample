CREATE PROCEDURE [dbo].[GetLogs]
	@param1 DateTime
AS
	SELECT Id, [TimeStamp], [Description] FROM Log WHERE [TimeStamp] <= @param1
RETURN 0
