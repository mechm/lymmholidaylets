-- =============================================
-- Author:		Matt Chambers
-- Create date: 24/09/2023
-- Description:	Whether a date is available for a date for a property
-- =============================================
CREATE PROCEDURE [dbo].[Available_GetByPropertyID]
	@PropertyID tinyint,
	@date date	
AS
BEGIN
	  SET NOCOUNT ON;

      SELECT [Available]
	  FROM [dbo].[Calendar] WITH (nolock)
	  WHERE PropertyID = @PropertyID AND [Date] = @date	

END