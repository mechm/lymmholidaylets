CREATE TABLE [dbo].[EmailEnquiry] (
    [EmailEnquiryId]     SMALLINT       IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (100) NOT NULL,
    [Company]            NVARCHAR (150) NULL,
    [EmailAddress]       NVARCHAR (100) NULL,
    [TelephoneNo]        NVARCHAR (30)  NULL,
    [Subject]            NVARCHAR (200) NULL,
    [Message]            NVARCHAR (MAX) NOT NULL,
    [DateTimeOfEnquiry]  DATETIME2 (0)  NOT NULL,
    CONSTRAINT [PK_dbo.EmailEnquiries] PRIMARY KEY CLUSTERED ([EmailEnquiryId] ASC)
);

