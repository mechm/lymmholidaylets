CREATE TABLE [dbo].[GeoLocation] (
    [ID]                  TINYINT    IDENTITY (1, 1) NOT NULL,
    [Latitude]            FLOAT (53) NOT NULL,
    [Longitude]           FLOAT (53) NOT NULL,
    [Zoom]                FLOAT (53) NOT NULL,
    [StreetViewLatitude]  FLOAT (53) NOT NULL,
    [StreetViewLongitude] FLOAT (53) NOT NULL,
    [Yaw]                 FLOAT (53) NOT NULL,
    [Pitch]               FLOAT (53) NOT NULL,
    [MapZoom]             TINYINT    NOT NULL,
    CONSTRAINT [PK_GeoLocation] PRIMARY KEY CLUSTERED ([ID] ASC),
    CONSTRAINT [U_GeoLocation] UNIQUE NONCLUSTERED ([Latitude] ASC, [Longitude] ASC, [Zoom] ASC, [Yaw] ASC, [Pitch] ASC, [MapZoom] ASC)
);

