CREATE TABLE [dbo].[ReviewType] (
    [ReviewTypeId] TINYINT      IDENTITY (1, 1) NOT NULL,
    [Description]  VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_ReviewTypes] PRIMARY KEY CLUSTERED ([ReviewTypeId] ASC),
    CONSTRAINT [U_ReviewType] UNIQUE NONCLUSTERED ([Description] ASC)
);

