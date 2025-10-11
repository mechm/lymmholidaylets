CREATE TABLE [dbo].[HouseType] (
    [ID]          TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_HouseTypes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_HouseType] UNIQUE NONCLUSTERED ([Description] ASC)
);

