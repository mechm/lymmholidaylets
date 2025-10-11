CREATE TABLE [dbo].[FeatureType] (
    [ID]          SMALLINT      IDENTITY (1, 1) NOT NULL,
    [Description] VARCHAR (255) NOT NULL,
    CONSTRAINT [PK_FeatureTypes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_FeatureType] UNIQUE NONCLUSTERED ([Description] ASC)
);

