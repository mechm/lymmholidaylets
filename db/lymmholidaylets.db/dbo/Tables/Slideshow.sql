CREATE TABLE [dbo].[Slideshow] (
    [ID]                 TINYINT       IDENTITY (1, 1) NOT NULL,
    [ImagePath]          VARCHAR (255) NOT NULL,
    [ImagePathAlt]       VARCHAR (255) NOT NULL,
    [CaptionTitle]       VARCHAR (500) NULL,
    [Caption]            VARCHAR (500) NULL,
    [ShortMobileCaption] VARCHAR (255) NULL,
    [Link]               VARCHAR (255) NULL,
    [SequenceOrder]      TINYINT       NOT NULL,
    [Visible]            BIT           CONSTRAINT [DF_Slides_Visible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Slides] PRIMARY KEY CLUSTERED ([ID] ASC)
);

