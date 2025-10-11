CREATE TABLE [dbo].[Page] (
    [PageId]          TINYINT        IDENTITY (1, 1) NOT NULL,
    [AliasTitle]      VARCHAR (255)  NOT NULL,
    [MetaDescription] VARCHAR (1000) NOT NULL,
    [Title]           VARCHAR (255)  NOT NULL,
    [MainImage]       VARCHAR (255)  NULL,
    [MainImageAlt]        VARCHAR (255)  NULL,
    [Description]     VARCHAR (MAX)  NOT NULL,
    [TemplateId]      TINYINT        NOT NULL,
    [Visible]         BIT            NOT NULL,
    CONSTRAINT [PK_Pages] PRIMARY KEY CLUSTERED ([PageId] ASC),
    CONSTRAINT [FK_Page_Template] FOREIGN KEY ([TemplateId]) REFERENCES [dbo].[Template] ([TemplateId])
);


GO
CREATE NONCLUSTERED INDEX [AliasTitle_Idx]
    ON [dbo].[Page]([AliasTitle] ASC);


GO
