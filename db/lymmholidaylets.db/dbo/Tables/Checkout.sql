CREATE TABLE [dbo].[Checkout] (
    [ID]                          INT            IDENTITY (1, 1) NOT NULL,
    [PropertyID]                  TINYINT        NOT NULL,
    [CheckIn]                     DATETIME2 (0)  NOT NULL,
    [CheckOut]                    DATETIME2 (0)  NOT NULL,
    [StripeNightProductID]        VARCHAR (100)  NOT NULL,
    [StripeNightDefaultPriceID]   VARCHAR (100)  NOT NULL,
    [StripeNightDefaultUnitPrice] DECIMAL (7, 2) NOT NULL,
    [StripeNightCouponID]         VARCHAR (100)  NULL,
    [StripeNightPercentage]       DECIMAL (5, 2) NULL,
    [OverallPrice]                DECIMAL (7, 2) NOT NULL,
    [Created]                     DATETIME2 (0)  NOT NULL,
    [Updated]                     DATETIME2 (0)  NULL,
    CONSTRAINT [PK__Checkout__3214EC273D9CE298] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [CK_CheckIn_CheckOut] CHECK ([CheckOut]>[CheckIn]),
    CONSTRAINT [FK_Checkout_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [UQ_Checkout_PropertyID_CheckIn_CheckOut] UNIQUE NONCLUSTERED ([PropertyID] ASC, [CheckIn] ASC, [CheckOut] ASC)
);





GO
CREATE NONCLUSTERED INDEX [IDX_PropertyID_CheckIn_CheckOut]
    ON [dbo].[Checkout]([PropertyID] ASC, [CheckIn] ASC, [CheckOut] ASC);

