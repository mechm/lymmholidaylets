CREATE TABLE [dbo].[Address] (
    [ID]             TINYINT       IDENTITY (1, 1) NOT NULL,
    [AddressLineOne] VARCHAR (255) NOT NULL,
    [AddressLineTwo] VARCHAR (255) NULL,
    [TownOrCity]     VARCHAR (100) NULL,
    [County]         VARCHAR (100) NULL,
    [Postcode]       VARCHAR (10)  NULL,
    [Country]        VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Address] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_Address] UNIQUE NONCLUSTERED ([AddressLineOne] ASC, [AddressLineTwo] ASC, [TownOrCity] ASC, [County] ASC, [Postcode] ASC, [Country] ASC)
);

