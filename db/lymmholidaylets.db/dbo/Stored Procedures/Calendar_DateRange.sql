CREATE PROCEDURE [dbo].[Calendar_DateRange]  
AS
BEGIN
  	SET NOCOUNT ON;
	BEGIN TRY

	DECLARE @StartDate DATE = (SELECT DATEADD(DAY,1, DATEADD(month, -13, EOMONTH(GETDATE(),-1))))
	DECLARE @EndDate DATE = (SELECT DATEADD(Year, 2, GETDATE()))

	DECLARE @Today DATE = CAST(GETDATE() AS DATE)
	DECLARE @SixMonth DATE = DATEADD(MONTH, 6, GETDATE())

	TRUNCATE table [dbo].CalendarRange
  
	;WITH ListDates(AllDates) AS 
	(    
		SELECT @StartDate AS DATE
		UNION ALL
		SELECT DATEADD(DAY,1,AllDates)
		FROM ListDates 
		WHERE AllDates < @EndDate
	)

	INSERT INTO [dbo].CalendarRange(CalendarDate, Available)
	SELECT AllDates, 
	CASE WHEN AllDates >= @Today AND AllDates < @SixMonth THEN 1 ELSE 0 END as Available
	FROM ListDates WITH (nolock)
	OPTION (MAXRECURSION 0)

BEGIN TRANSACTION 

	DELETE FROM dbo.Calendar WHERE [Date] < @StartDate

	INSERT INTO Calendar 
	           ([PropertyID]
			   ,[Date]
			   ,[Price]
			   ,[MinimumStay]
			   ,[MaximumStay]
			   ,[Available])
	SELECT p.ID, cr.CalendarDate, p.[DefaultNightlyPrice], p.[DefaultMinimumStay]
		  ,p.[DefaultMaximumStay], cr.Available
	FROM
		   [dbo].CalendarRange cr WITH (nolock),
		   [dbo].[Property] p WITH (nolock)
	WHERE CalendarDate NOT IN 
		(SELECT [Date] from [dbo].Calendar c WITH (NOLOCK) WHERE c.PropertyID = p.ID)

COMMIT TRANSACTION;	
	END TRY
	BEGIN CATCH		  
		IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
		THROW; 
	END CATCH;

END