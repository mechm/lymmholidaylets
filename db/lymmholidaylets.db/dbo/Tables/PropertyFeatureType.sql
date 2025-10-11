CREATE TABLE [dbo].[PropertyFeatureType] (
    [PropertyId]    TINYINT  NOT NULL,
    [FeatureTypeId] SMALLINT NOT NULL,
    [ShowOnSite]    BIT      NOT NULL,
    CONSTRAINT [PK_PropertyFeatures] PRIMARY KEY CLUSTERED ([PropertyId] ASC, [FeatureTypeId] ASC),
    CONSTRAINT [FK_PropertyFeatures_FeatureTypes] FOREIGN KEY ([FeatureTypeId]) REFERENCES [dbo].[FeatureType] ([ID]),
    CONSTRAINT [FK_PropertyFeatureType_Property] FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property] ([ID])
);

