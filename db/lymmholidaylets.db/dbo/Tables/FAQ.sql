CREATE TABLE [dbo].[FAQ] (
    [ID]         TINYINT        IDENTITY (1, 1) NOT NULL,
    [PropertyID] TINYINT        NOT NULL,
    [Question]   VARCHAR (2000) NOT NULL,
    [Answer]     VARCHAR (MAX)  NOT NULL,
    [Visible]    BIT            NOT NULL,
    CONSTRAINT [PK_FAQ] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_FAQ_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);

