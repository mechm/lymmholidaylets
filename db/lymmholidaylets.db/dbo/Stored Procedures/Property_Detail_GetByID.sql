-- =============================================
-- Author:		Matt Chambers
-- Create date: 26/11/2023
-- Description:	Booking conditions for a property and Availability
-- =============================================
CREATE PROCEDURE [dbo].[Property_Detail_GetByID]
	@PropertyID tinyint
AS
BEGIN
	SET NOCOUNT ON;

    SELECT [ID], [MinimumNumberOfAdult], [MaximumNumberOfAdult], [MaximumNumberOfGuests], 
		   [MaximumNumberOfChildren], [MaximumNumberOfInfants]
    FROM [dbo].[Property] WITH (nolock)
    WHERE [ID] = @PropertyID 

    SELECT [Date]
    FROM [dbo].[Calendar] WITH (nolock)
    WHERE [PropertyID] = @PropertyID AND[Available] = 0 AND
    [Date] BETWEEN DATEADD(DAY,1,EOMONTH(GETDATE(), -1)) AND DATEADD(YEAR,1,EOMONTH(GETDATE()))
    ORDER BY [Date]

	SELECT [Question],[Answer]
	FROM [dbo].[FAQ] WITH (nolock) 
	WHERE [PropertyID] = @PropertyID AND Visible = 1

 	SELECT [Company],R.[Description],[Name],[Position],[Rating]
      ,[Cleanliness],[Accuracy],[Communication]
      ,[Location],[Checkin],[Facilities],[Comfort],[Value]
      ,RT.[Description] as ReviewType,[LinkToView],[DateTimeAdded]
    FROM [dbo].[Review] R WITH (nolock) 
	INNER JOIN [dbo].[ReviewType] RT WITH (nolock) ON RT.ReviewTypeId = R.ReviewTypeId
	WHERE [PropertyID] = @PropertyID AND Approved = 1
	ORDER BY [DateTimeAdded] DESC

END