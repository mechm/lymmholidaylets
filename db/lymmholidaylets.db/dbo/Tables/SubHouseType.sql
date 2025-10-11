CREATE TABLE [dbo].[SubHouseType] (
    [ID]           SMALLINT     IDENTITY (1, 1) NOT NULL,
    [SubHouseType] VARCHAR (50) NOT NULL,
    [HouseTypeId]  TINYINT      CONSTRAINT [DF_SubHouseTypes_HouseTypeFK] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_SubHouseTypes] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_SubHouseTypes_HouseTypes] FOREIGN KEY ([HouseTypeId]) REFERENCES [dbo].[HouseType] ([ID]),
    CONSTRAINT [U_SubHouseType] UNIQUE NONCLUSTERED ([SubHouseType] ASC, [HouseTypeId] ASC)
);

