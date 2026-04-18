CREATE TABLE [dbo].[GuestEmailDispatchLog] (
    [ID]                      INT             IDENTITY (1, 1) NOT NULL,
    [BookingID]               INT             NOT NULL,
    [EmailType]               VARCHAR (50)    NOT NULL,
    [Status]                  VARCHAR (20)    NOT NULL,
    [ScheduledForUtc]         DATETIME2 (0)   NOT NULL,
    [ReservationExpiresAtUtc] DATETIME2 (0)   NULL,
    [PublishedAtUtc]          DATETIME2 (0)   NULL,
    [SentAtUtc]               DATETIME2 (0)   NULL,
    [FailureMessage]          VARCHAR (1000)  NULL,
    [AttemptCount]            SMALLINT        NOT NULL,
    [Created]                 DATETIME2 (0)   NOT NULL,
    [Updated]                 DATETIME2 (0)   NULL,
    CONSTRAINT [PK_GuestEmailDispatchLog] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_GuestEmailDispatchLog_Booking] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Booking] ([ID]),
    CONSTRAINT [CK_GuestEmailDispatchLog_Status] CHECK ([Status] IN ('Reserved', 'Published', 'Sent', 'Failed')),
    CONSTRAINT [CK_GuestEmailDispatchLog_AttemptCount] CHECK ([AttemptCount] > 0)
);

GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_GuestEmailDispatchLog_Booking_EmailType]
    ON [dbo].[GuestEmailDispatchLog]([BookingID] ASC, [EmailType] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_GuestEmailDispatchLog_Status_ReservationExpiresAtUtc]
    ON [dbo].[GuestEmailDispatchLog]([Status] ASC, [ReservationExpiresAtUtc] ASC)
    INCLUDE ([BookingID], [EmailType], [ScheduledForUtc], [SentAtUtc]);
