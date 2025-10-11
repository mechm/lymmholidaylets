CREATE TABLE [dbo].[PropertyRestriction] (
    [PropertyId]        TINYINT NOT NULL,
    [RestrictionTypeId] TINYINT NOT NULL,
    CONSTRAINT [PK_PropertyRestrictions] PRIMARY KEY CLUSTERED ([PropertyId] ASC, [RestrictionTypeId] ASC),
    CONSTRAINT [FK_PropertyRestriction_Property] FOREIGN KEY ([PropertyId]) REFERENCES [dbo].[Property] ([ID]),
    CONSTRAINT [FK_PropertyRestrictions_RestrictionTypes] FOREIGN KEY ([RestrictionTypeId]) REFERENCES [dbo].[RestrictionType] ([ID])
);

