CREATE TABLE [dbo].[PropertyGuestEmailSchedule] (
    [ID]                    INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID]            TINYINT        NOT NULL,
    [IsEnabled]             BIT            NOT NULL,
    [SendDaysBeforeCheckIn] SMALLINT       NOT NULL,
    [Created]               DATETIME2 (0)  NOT NULL,
    [Updated]               DATETIME2 (0)  NULL,
    CONSTRAINT [PK_PropertyGuestEmailSchedule] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyGuestEmailSchedule_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [CK_PropertyGuestEmailSchedule_SendDaysBeforeCheckIn] CHECK ([SendDaysBeforeCheckIn] >= 0 AND [SendDaysBeforeCheckIn] <= 365)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_PropertyGuestEmailSchedule_Property]
    ON [dbo].[PropertyGuestEmailSchedule]([PropertyID] ASC);
