CREATE TABLE [dbo].[PropertyHouseRule] (
    [ID]            INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID]    TINYINT        NOT NULL,
    [RuleText]      VARCHAR (500)  NOT NULL,
    [SequenceOrder] TINYINT        NOT NULL,
    CONSTRAINT [PK_PropertyHouseRule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyHouseRule_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_PropertyHouseRule_Property_Sequence]
    ON [dbo].[PropertyHouseRule]([PropertyID] ASC, [SequenceOrder] ASC);
