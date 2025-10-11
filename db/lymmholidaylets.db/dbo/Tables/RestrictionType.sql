CREATE TABLE [dbo].[RestrictionType] (
    [ID]          TINYINT       IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_RestrictionTypes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_RestrictionType] UNIQUE NONCLUSTERED ([Description] ASC)
);

