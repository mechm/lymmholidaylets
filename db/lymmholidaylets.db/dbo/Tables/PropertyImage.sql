CREATE TABLE [dbo].[PropertyImage] (
    [ID]            SMALLINT      IDENTITY (1, 1) NOT NULL,
    [PropertyId]    TINYINT       NOT NULL,
    [ImagePath]     VARCHAR (255) NOT NULL,
    [SequenceOrder] TINYINT       CONSTRAINT [DF_PropertyImages_SequenceID] DEFAULT ((1)) NOT NULL,
    [ShowOnSite]    BIT           CONSTRAINT [DF_PropertyImage_Visible] DEFAULT ((1)) NOT NULL,
    [Created]       DATETIME2 (0) NOT NULL,
    [Optimised]     BIT           CONSTRAINT [DF__PropertyI__Optim__113584D1] DEFAULT ((0)) NULL,
    CONSTRAINT [PK_PropertyImage] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [FK_PropertyImage_Property] FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property] ([ID])
);

