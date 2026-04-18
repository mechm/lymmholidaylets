CREATE TABLE [dbo].[CancellationPolicy] (
    [ID]                INT             IDENTITY (1, 1) NOT NULL,
    [PropertyID]        TINYINT         NOT NULL,
    [DaysBeforeCheckIn] SMALLINT        NOT NULL,
    [PolicyText]        VARCHAR (1000)  NOT NULL,
    [SequenceOrder]     TINYINT         NOT NULL,
    CONSTRAINT [PK_CancellationPolicy] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_CancellationPolicy_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID])
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_CancellationPolicy_Property_Days]
    ON [dbo].[CancellationPolicy]([PropertyID] ASC, [DaysBeforeCheckIn] ASC, [SequenceOrder] ASC);
