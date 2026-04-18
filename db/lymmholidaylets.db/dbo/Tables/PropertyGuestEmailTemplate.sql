CREATE TABLE [dbo].[PropertyGuestEmailTemplate] (
    [ID]                   INT             IDENTITY (1, 1) NOT NULL,
    [PropertyID]           TINYINT         NOT NULL,
    [SubjectTemplate]      VARCHAR (200)   NULL,
    [PreviewTextTemplate]  VARCHAR (500)   NULL,
    [HtmlBody]             VARCHAR (MAX)   NOT NULL,
    [Created]              DATETIME2 (0)   NOT NULL,
    [Updated]              DATETIME2 (0)   NULL,
    CONSTRAINT [PK_PropertyGuestEmailTemplate] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyGuestEmailTemplate_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_PropertyGuestEmailTemplate_Property]
    ON [dbo].[PropertyGuestEmailTemplate]([PropertyID] ASC);
