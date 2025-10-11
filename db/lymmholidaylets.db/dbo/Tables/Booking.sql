CREATE TABLE [dbo].[Booking] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [SessionID]  VARCHAR (100)  NOT NULL,
    [EventID]    VARCHAR (100)  NOT NULL,
    [PropertyID] TINYINT        NOT NULL,
    [CheckIn]    DATETIME2 (0)  NOT NULL,
    [CheckOut]   DATETIME2 (0)  NOT NULL,
    [NoAdult]    TINYINT        NULL,
    [NoChildren] TINYINT        NULL,
    [NoInfant]   TINYINT        NULL,
    [NoOfGuests] AS             ((isnull([NoAdult],(0))+isnull([NoChildren],(0)))+isnull([NoInfant],(0))),
    [Name]       VARCHAR (255)  NOT NULL,
    [Email]      NVARCHAR (100) NOT NULL,
    [Telephone]  NVARCHAR (30)  NOT NULL,
    [PostalCode] VARCHAR (10)   NOT NULL,
    [Country]    VARCHAR (100)  NOT NULL,
    [Total]      BIGINT         NULL,
    [Created]    DATETIME2 (0)  NOT NULL,
    [Updated]    DATETIME2 (0)  NULL,
    CONSTRAINT [PK__Booking__3214EC27681479A7] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_Booking_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [UQ_PropertyID_CheckinDate_CheckoutDate] UNIQUE NONCLUSTERED ([PropertyID] ASC, [CheckIn] ASC, [CheckOut] ASC)
);










