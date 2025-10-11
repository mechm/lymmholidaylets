CREATE TABLE [dbo].[SizeUnitType] (
    [ID]          TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NULL,
    CONSTRAINT [PK__SizeUnit] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_SizeUnitType] UNIQUE NONCLUSTERED ([Description] ASC)
);

