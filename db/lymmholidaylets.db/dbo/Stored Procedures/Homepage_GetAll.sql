-- =============================================
-- Author:		Matt Chambers
-- Create date: 11/05/2024
-- Description:	Everything required for homepage
-- =============================================
CREATE PROCEDURE [dbo].[Homepage_GetAll]	
AS
BEGIN
   SET NOCOUNT ON;

   SELECT TOP 10 p.FriendlyName
      ,[Company]
      ,r.[Description]
      ,[Name]     
      ,[Position]   
      ,[DateTimeAdded]
  FROM [dbo].[Review] r with (nolock)  
  INNER JOIN [dbo].[Property] p with (nolock) on r.PropertyID = p.ID
  WHERE r.ShowOnHomepage = 1 AND Approved = 1
  ORDER BY NEWID()

  SELECT [ImagePath],[ImagePathAlt]
      ,[CaptionTitle],[Caption]
      ,[ShortMobileCaption],[Link]
  FROM [dbo].[Slideshow] with (nolock) 
  WHERE [Visible] = 1
  ORDER BY [SequenceOrder]

END