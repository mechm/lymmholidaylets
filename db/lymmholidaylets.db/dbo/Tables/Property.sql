CREATE TABLE [dbo].[Property] (
    [ID]                      TINYINT        IDENTITY (1, 1) NOT NULL,
    [FriendlyName]            VARCHAR (255)  NOT NULL,
    [StaffId]                 TINYINT        NOT NULL,
    [AddressId]               TINYINT        NULL,
    [SubHouseTypeId]          SMALLINT       NULL,
    [FurnishingTypeId]        TINYINT        NULL,
    [Bedroom]                 TINYINT        NULL,
    [Bathroom]                FLOAT (53)     NULL,
    [Floor]                   TINYINT        NULL,
    [ReceptionRoom]           TINYINT        NULL,
    [Kitchen]                 TINYINT        NULL,
    [LivingRoom]              TINYINT        NULL,
    [YearBuilt]               SMALLINT       NULL,
    [DisplayAddress]          VARCHAR (500)  NULL,
    [Description]             VARCHAR (MAX)  NULL,
    [DefaultNightlyPrice]     DECIMAL (5, 2) NOT NULL,
    [DefaultMinimumStay]      TINYINT        NOT NULL,
    [DefaultMaximumStay]      SMALLINT       NULL,
    [CheckInTimeAfter]        TIME (0)       NOT NULL,
    [CheckOutTimeBefore]      TIME (0)       NOT NULL,
    [MaximumNumberOfGuests]   TINYINT        NOT NULL,
    [MinimumNumberOfAdult]    TINYINT        NULL,
    [MaximumNumberOfAdult]    TINYINT        NOT NULL,
    [MaximumNumberOfChildren] TINYINT        NOT NULL,
    [MaximumNumberOfInfants]  TINYINT        NOT NULL,
    [ShowGoogleMap]           BIT            NULL,
    [ShowStreetView]          BIT            NULL,
    [GeoLocationId]           TINYINT        NULL,
    [CarSpace]                TINYINT        NULL,
    [Size]                    DECIMAL (8, 2) NULL,
    [SizeUnitTypeId]          TINYINT        NULL,
    [VideoHtml]               VARCHAR (4000) NULL,
    [Disclaimer]              VARCHAR (4000) NULL,
    [ShowOnHomepage]          BIT            NULL,
    [ShowOnSite]              BIT            NULL,
    [Created]                 DATETIME2 (0)  NOT NULL,
    [Updated]                 DATETIME2 (0)  NULL,
    CONSTRAINT [PK__Property__3214EC273A088C05] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [UQ_FriendlyName] UNIQUE NONCLUSTERED ([FriendlyName] ASC)
);














