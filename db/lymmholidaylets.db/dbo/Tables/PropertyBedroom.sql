CREATE TABLE [dbo].[PropertyBedroom] (
    [ID] SMALLINT IDENTITY(1,1) NOT NULL,
    [PropertyId] TINYINT NOT NULL,
    [BedroomNumber] TINYINT NOT NULL,
    [BedroomName] VARCHAR(100) NULL,
    [BedTypeId] TINYINT NOT NULL,
    [NumberOfBeds] TINYINT NOT NULL DEFAULT 1,
    [SequenceOrder] TINYINT NOT NULL DEFAULT 1,
    [ShowOnSite] BIT NOT NULL DEFAULT 1,
    [Created] DATETIME2(0) NOT NULL DEFAULT GETDATE(),
    [Updated] DATETIME2(0) NULL,
    CONSTRAINT [PK_PropertyBedroom] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyBedroom_Property] FOREIGN KEY ([PropertyId]) 
        REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [FK_PropertyBedroom_BedType] FOREIGN KEY ([BedTypeId]) 
        REFERENCES [dbo].[BedType] ([ID])
);

GO

CREATE NONCLUSTERED INDEX [IX_PropertyBedroom_PropertyId] 
    ON [dbo].[PropertyBedroom] ([PropertyId]) 
    INCLUDE ([ShowOnSite], [SequenceOrder]);
