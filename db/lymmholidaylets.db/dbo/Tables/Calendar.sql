CREATE TABLE [dbo].[Calendar] (
    [ID]          INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID]  TINYINT        NOT NULL,
    [Date]        DATE           NOT NULL,
    [Price]       DECIMAL (5, 2) NULL,
    [MinimumStay] TINYINT        NOT NULL,
    [MaximumStay] SMALLINT       NULL,
    [Available]   BIT            CONSTRAINT [DF_Calendar_Available] DEFAULT ((0)) NOT NULL,
    [Booked]      BIT            CONSTRAINT [DF_Calendar_Booked] DEFAULT ((0)) NOT NULL,
    [BookingID]   INT            NULL,
    CONSTRAINT [PK__tmp_ms_x__3214EC2739E61898] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CK_Booked_Available] CHECK ([Booked]=(1) AND [Available]=(0) OR [Booked]=(0) AND [Available]=(0) OR [Booked]=(0) AND [Available]=(1)),
    CONSTRAINT [CK_BookingID_Booked] CHECK ([BookingID] IS NULL OR [BookingID] IS NOT NULL AND [Booked]=(1) AND [Available]=(0)),
    CONSTRAINT [CK_Min_Max] CHECK ([MaximumStay] IS NULL OR [MaximumStay]>=[MinimumStay]),
    CONSTRAINT [FK_Calendar_Booking] FOREIGN KEY ([BookingID]) REFERENCES [dbo].[Booking] ([ID]),
    CONSTRAINT [FK_Calendar_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [UQ_Calendar_PropertyID_Date] UNIQUE NONCLUSTERED ([PropertyID] ASC, [Date] ASC)
);





GO
CREATE NONCLUSTERED INDEX [IDX_PropertyID_Date_Available]
    ON [dbo].[Calendar]([PropertyID] ASC, [Date] ASC, [Available] ASC);

