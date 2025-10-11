CREATE TABLE [dbo].[ICal] (
    [ID]         TINYINT          IDENTITY (1, 1) NOT NULL,
    [PropertyID] TINYINT          NOT NULL,
    [Identifier] UNIQUEIDENTIFIER NOT NULL,
    CONSTRAINT [PK__ICal__3214EC27280F7101] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_ICal_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);







