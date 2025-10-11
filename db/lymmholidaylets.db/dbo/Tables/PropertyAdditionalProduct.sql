CREATE TABLE [dbo].[PropertyAdditionalProduct] (
    [ID]                     TINYINT        IDENTITY (1, 1) NOT NULL,
    [PropertyID]             TINYINT        NOT NULL,
    [StripeProductID]        VARCHAR (100)  NOT NULL,
    [StripeName]             VARCHAR (500)  NOT NULL,
    [StripeDescription]      VARCHAR (500)  NOT NULL,
    [StripeDefaultPriceID]   VARCHAR (100)  NOT NULL,
    [StripeDefaultUnitPrice] DECIMAL (5, 2) NOT NULL,
    [Quantity] TINYINT NOT NULL, 
    CONSTRAINT [PK__Property__3214EC279BF1B6BD] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyAdditionalProduct_Property] FOREIGN KEY ([PropertyID]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [UQ_PropertyAdditionalProduct_PropertyID_StripeProductID] UNIQUE NONCLUSTERED ([PropertyID] ASC, [StripeProductID] ASC)
);


