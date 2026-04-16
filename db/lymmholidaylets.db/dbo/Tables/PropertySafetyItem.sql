CREATE TABLE [dbo].[PropertySafetyItem] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID]    TINYINT        NOT NULL,
    [ItemText]      VARCHAR (500)  NOT NULL,
    [SequenceOrder] TINYINT        NOT NULL,
    CONSTRAINT [PK_PropertySafetyItem] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertySafetyItem_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_PropertySafetyItem_Property_Sequence]
    ON [dbo].[PropertySafetyItem]([PropertyID] ASC, [SequenceOrder] ASC);
