-- =============================================
-- Author:		Matt Chambers
-- Create date: 17/09/2023
-- Description:	Get all calendars
-- =============================================
CREATE PROCEDURE [dbo].[ICal_GetAll]
AS
BEGIN	
	SET NOCOUNT ON;

    SELECT ic.[ID],ic.[PropertyID],p.[FriendlyName], ic.[Identifier]
    FROM [dbo].[ICal] ic WITH (NOLOCK) 
	INNER JOIN [dbo].[Property] p WITH (NOLOCK) on p.ID = ic.PropertyID

END