CREATE TABLE [dbo].[PropertyNightCoupon] (
    [ID]         INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID] TINYINT        NOT NULL,
    [NoOfNight]  SMALLINT       NOT NULL,
    [Percentage] DECIMAL (5, 2) NOT NULL,
    CONSTRAINT [PK__Property__3214EC27E7FCEC00] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyNightCoupon_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [UQ_PropertyNightCoupon_PropertyID_NoOfNight] UNIQUE NONCLUSTERED ([PropertyID] ASC, [NoOfNight] ASC)
);




