CREATE TABLE [dbo].[FurnishingType] (
    [ID]          TINYINT       IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_FurnishingTypes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_FurnishingType] UNIQUE NONCLUSTERED ([Description] ASC)
);

