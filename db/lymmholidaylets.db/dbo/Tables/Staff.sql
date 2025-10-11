CREATE TABLE [dbo].[Staff] (
    [ID]              TINYINT        IDENTITY (1, 1) NOT NULL,
    [Name]            NVARCHAR (256) NOT NULL,
    [YearsExperience] TINYINT        NOT NULL,
    [JobTitle]        VARCHAR (256)  NOT NULL,
    [ProfileBio]      VARCHAR (3000) NULL,
    [LinkedInLink]    VARCHAR (256)  NULL,
    [ImagePath]       VARCHAR (256)  NOT NULL,
    [Visible]         BIT            NOT NULL,
    CONSTRAINT [PK_Staff] PRIMARY KEY CLUSTERED ([ID] ASC)
);

