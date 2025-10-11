CREATE TABLE [dbo].[Template] (
    [TemplateId]  TINYINT       IDENTITY (1, 1) NOT NULL,
    [Description] NVARCHAR (50) NOT NULL,
    CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED ([TemplateId] ASC),
    CONSTRAINT [U_Template] UNIQUE NONCLUSTERED ([Description] ASC)
);

