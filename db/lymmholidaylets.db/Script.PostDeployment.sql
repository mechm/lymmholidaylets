/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]		

 EXEC dbo.sp_generate_merge @schema = 'dbo', @table_name ='County'
 //https://github.com/readyroll/generate-sql-merge
--------------------------------------------------------------------------------------
*/
GO

MERGE INTO [dbo].[AspNetRoles] AS [Target]
USING (VALUES
  (N'4e6784b9-ee1c-43bc-8fa7-776a3cf898bc',N'13a272f6-f8a6-4222-9c7d-d44c04bf8820',N'Sitewide administrator',N'Admin',N'ADMIN')
 ,(N'bf946c8d-1295-4af8-90d5-854de29376cc',N'35a89aa2-a97e-4b22-a715-7b0eedeebf17',N'Top level administrator',N'SuperAdmin',N'SUPERADMIN')
) AS [Source] ([Id],[ConcurrencyStamp],[Description],[Name],[NormalizedName])
ON ([Target].[Id] = [Source].[Id])
WHEN MATCHED AND (
	NULLIF([Source].[ConcurrencyStamp], [Target].[ConcurrencyStamp]) IS NOT NULL OR NULLIF([Target].[ConcurrencyStamp], [Source].[ConcurrencyStamp]) IS NOT NULL OR 
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL OR 
	NULLIF([Source].[Name], [Target].[Name]) IS NOT NULL OR NULLIF([Target].[Name], [Source].[Name]) IS NOT NULL OR 
	NULLIF([Source].[NormalizedName], [Target].[NormalizedName]) IS NOT NULL OR NULLIF([Target].[NormalizedName], [Source].[NormalizedName]) IS NOT NULL) THEN
 UPDATE SET
  [ConcurrencyStamp] = [Source].[ConcurrencyStamp], 
  [Description] = [Source].[Description], 
  [Name] = [Source].[Name], 
  [NormalizedName] = [Source].[NormalizedName]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[ConcurrencyStamp],[Description],[Name],[NormalizedName])
 VALUES([Source].[Id],[Source].[ConcurrencyStamp],[Source].[Description],[Source].[Name],[Source].[NormalizedName])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;

GO

MERGE INTO [dbo].[ASPNetUsers] AS [Target]
USING (VALUES
  (N'36c2170d-19c1-4253-b841-607a3c63874d',0,N'3cc3dc75-5c9e-4033-84ca-092efe87486f',N'matthew@chmit.co.uk',1,1,NULL,N'MATTHEW@CHMIT.CO.UK',N'MATTHEW@CHMIT.CO.UK',N'AQAAAAEAACcQAAAAENZAjndf+mC8gBYrwqaGYC4TD0lNz6bx6oKCsITpO06kvUH0nranY5rHaulrc2GcqQ==',NULL,0,N'GMFGQZMWBGUP2PAJ4SSXFKYWNYYUHWZO',0,N'matthew@chmit.co.uk')
 ,(N'd1a46617-31f2-483e-ab58-3f4249d03390',0,N'fc4ffe4a-35e5-453b-8b94-bcf5a9140f00',N'lettings@cheshire-housing.co.uk',1,1,NULL,N'LETTINGS@CHESHIRE-HOUSING.CO.UK',N'LETTINGS@CHESHIRE-HOUSING.CO.UK',N'AQAAAAEAACcQAAAAEJA3xhHcJKbuaXJzTI0BN7AGmmJ/OdcCdiUgrDzBVh7cbAbQGW+HE9mQe5zw/Buonw==',NULL,0,N'6JDWZH3OZD4E4ZDOUIHAGYUPTECFLPBJ',0,N'lettings@cheshire-housing.co.uk')
) AS [Source] ([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])
ON ([Target].[Id] = [Source].[Id])
WHEN MATCHED AND (
	NULLIF([Source].[AccessFailedCount], [Target].[AccessFailedCount]) IS NOT NULL OR NULLIF([Target].[AccessFailedCount], [Source].[AccessFailedCount]) IS NOT NULL OR 
	NULLIF([Source].[ConcurrencyStamp], [Target].[ConcurrencyStamp]) IS NOT NULL OR NULLIF([Target].[ConcurrencyStamp], [Source].[ConcurrencyStamp]) IS NOT NULL OR 
	NULLIF([Source].[Email], [Target].[Email]) IS NOT NULL OR NULLIF([Target].[Email], [Source].[Email]) IS NOT NULL OR 
	NULLIF([Source].[EmailConfirmed], [Target].[EmailConfirmed]) IS NOT NULL OR NULLIF([Target].[EmailConfirmed], [Source].[EmailConfirmed]) IS NOT NULL OR 
	NULLIF([Source].[LockoutEnabled], [Target].[LockoutEnabled]) IS NOT NULL OR NULLIF([Target].[LockoutEnabled], [Source].[LockoutEnabled]) IS NOT NULL OR 
	NULLIF([Source].[LockoutEnd], [Target].[LockoutEnd]) IS NOT NULL OR NULLIF([Target].[LockoutEnd], [Source].[LockoutEnd]) IS NOT NULL OR 
	NULLIF([Source].[NormalizedEmail], [Target].[NormalizedEmail]) IS NOT NULL OR NULLIF([Target].[NormalizedEmail], [Source].[NormalizedEmail]) IS NOT NULL OR 
	NULLIF([Source].[NormalizedUserName], [Target].[NormalizedUserName]) IS NOT NULL OR NULLIF([Target].[NormalizedUserName], [Source].[NormalizedUserName]) IS NOT NULL OR 
	NULLIF([Source].[PasswordHash], [Target].[PasswordHash]) IS NOT NULL OR NULLIF([Target].[PasswordHash], [Source].[PasswordHash]) IS NOT NULL OR 
	NULLIF([Source].[PhoneNumber], [Target].[PhoneNumber]) IS NOT NULL OR NULLIF([Target].[PhoneNumber], [Source].[PhoneNumber]) IS NOT NULL OR 
	NULLIF([Source].[PhoneNumberConfirmed], [Target].[PhoneNumberConfirmed]) IS NOT NULL OR NULLIF([Target].[PhoneNumberConfirmed], [Source].[PhoneNumberConfirmed]) IS NOT NULL OR 
	NULLIF([Source].[SecurityStamp], [Target].[SecurityStamp]) IS NOT NULL OR NULLIF([Target].[SecurityStamp], [Source].[SecurityStamp]) IS NOT NULL OR 
	NULLIF([Source].[TwoFactorEnabled], [Target].[TwoFactorEnabled]) IS NOT NULL OR NULLIF([Target].[TwoFactorEnabled], [Source].[TwoFactorEnabled]) IS NOT NULL OR 
	NULLIF([Source].[UserName], [Target].[UserName]) IS NOT NULL OR NULLIF([Target].[UserName], [Source].[UserName]) IS NOT NULL) THEN
 UPDATE SET
  [AccessFailedCount] = [Source].[AccessFailedCount], 
  [ConcurrencyStamp] = [Source].[ConcurrencyStamp], 
  [Email] = [Source].[Email], 
  [EmailConfirmed] = [Source].[EmailConfirmed], 
  [LockoutEnabled] = [Source].[LockoutEnabled], 
  [LockoutEnd] = [Source].[LockoutEnd], 
  [NormalizedEmail] = [Source].[NormalizedEmail], 
  [NormalizedUserName] = [Source].[NormalizedUserName], 
  [PasswordHash] = [Source].[PasswordHash], 
  [PhoneNumber] = [Source].[PhoneNumber], 
  [PhoneNumberConfirmed] = [Source].[PhoneNumberConfirmed], 
  [SecurityStamp] = [Source].[SecurityStamp], 
  [TwoFactorEnabled] = [Source].[TwoFactorEnabled], 
  [UserName] = [Source].[UserName]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([Id],[AccessFailedCount],[ConcurrencyStamp],[Email],[EmailConfirmed],[LockoutEnabled],[LockoutEnd],[NormalizedEmail],[NormalizedUserName],[PasswordHash],[PhoneNumber],[PhoneNumberConfirmed],[SecurityStamp],[TwoFactorEnabled],[UserName])
 VALUES([Source].[Id],[Source].[AccessFailedCount],[Source].[ConcurrencyStamp],[Source].[Email],[Source].[EmailConfirmed],[Source].[LockoutEnabled],[Source].[LockoutEnd],[Source].[NormalizedEmail],[Source].[NormalizedUserName],[Source].[PasswordHash],[Source].[PhoneNumber],[Source].[PhoneNumberConfirmed],[Source].[SecurityStamp],[Source].[TwoFactorEnabled],[Source].[UserName])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE; 

GO

MERGE INTO [dbo].[AspNetUserRoles] AS [Target]
USING (VALUES
  (N'd1a46617-31f2-483e-ab58-3f4249d03390',N'4e6784b9-ee1c-43bc-8fa7-776a3cf898bc')
 ,(N'36c2170d-19c1-4253-b841-607a3c63874d',N'bf946c8d-1295-4af8-90d5-854de29376cc')
) AS [Source] ([UserId],[RoleId])
ON ([Target].[RoleId] = [Source].[RoleId] AND [Target].[UserId] = [Source].[UserId])
WHEN NOT MATCHED BY TARGET THEN
 INSERT([UserId],[RoleId])
 VALUES([Source].[UserId],[Source].[RoleId])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;

GO

SET IDENTITY_INSERT [dbo].[Template] ON 
GO
MERGE INTO [dbo].[Template] AS [Target]
USING (VALUES
  (1,N'Detail')
) AS [Source] ([TemplateId],[Description])
ON ([Target].[TemplateId] = [Source].[TemplateId])
WHEN MATCHED AND (
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = [Source].[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([TemplateId],[Description])
 VALUES([Source].[TemplateId],[Source].[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
SET IDENTITY_INSERT [dbo].[Template] OFF

GO

SET IDENTITY_INSERT [dbo].[Page] ON
MERGE INTO [dbo].[Page] AS [Target]
USING (VALUES
  (1,N'about-us',N'<p>We are a small local business based in Lymm Village, Cheshire specialising in property lettings. Our focus is to ensure the properties we offer exceed the expectations of our customers, so they will continue to return again and again. Our established team live and work in the area and have years of experience in ensuring customers enjoy a positive self-catering experience.</p>
<p>We have local knowledge of the area and are on hand to help with any questions you may have.</p>
<h3>Hosts</h3>
<div class="row mt-3 mb-3">
    <div class="col">
        <img src="/uploads/images/pages/kath.jpg" alt="Kathryn" class="img-fluid rounded d-block">
        <h4 class="title-small-text drak-grey-color py-3">Kathryn</h4>
        <p>I was born in Lymm and have lived in Lymm my whole life. I have my own property letting business - <a href="https://cheshire-housing.co.uk/about-us">Cheshire Housing Management</a>, based in the centre of Lymm Village which I have ran for 30 years. I have 4 children and 2 grandchildren. I enjoy weekends away in Wales and gardening. I am happy to help if you have any questions.</p>
    </div>
    <div class="col">
        <img src="/uploads/images/pages/matthew.jpg" alt="Matthew" class="img-fluid rounded d-block">
        <h4 class="title-small-text drak-grey-color py-3">Matthew</h4>
        <p>I was born in Lymm and currently work as a computer programmer. I have lived in the local area for 30 years. I enjoy scuba diving, Rugby League - Warrington Wolves and the NRL. I am always on hand to help.</p>
    </div>
</div>
<p>We also continually monitor our customer''s satisfaction with feedback from our customers. Please feel free to view our reviews - on our property pages or on <a href="https://www.google.com/maps/place/Cheshire+Housing+Management/@53.381605,-2.477359,17z/data=!4m8!3m7!1s0x487b000e06f3f83b:0xd1e655c26f4c51d8!8m2!3d53.381605!4d-2.477359!9m1!1b1!16s%2Fg%2F1tfhz3hw?entry=ttu">Google</a>.</p>
<p>We look forward to you becoming one of our many satisfied customers.</p>
<p>For friendly advice and more information about our services, please <a href="https://lymmholidaylets.com/contact-us">contact us</a>.</p>'
  ,1,1)
 ,(2,N'about-lymm',
 N'Lymm Village is situated in  Cheshire in the North West of England UK close to Manchester and Liverpool.',
  N'About Lymm',NULL,NULL,
  N'<h2>Lymm Cheshire Village</h2>
  <h3>A place of running water</h3>
  <p>Lymm of Celtic origins set in the heart of Cheshire in the North West of England literally means as a "place of running water". 
  This probably stems from the fact that a stream runs through the village centre, although the name is even more apt today because the Bridgewater Canal also passes through, which is an easy detour from the Trans Pennine Trail. 
  The charming village is home (or has been home) to numerous famous celebrities and was once mentioned in the Domesday Book of 1086 where the name appears as "Lime". 
  The village, which has great transport links, is located about 24 Km south west of Manchester, 8 Km from Warrington and 49 Km from Liverpool. 
  It has the atmosphere of a small town, boasting a population of over 12000. Lymm has a Twin Town; Meung-sur-Loire, in central France.</p>',1,1)
) AS [Source] ([PageId],[AliasTitle],[MetaDescription],[Title],[MainImage],[MainImageAlt],[Description],[TemplateId],[Visible])
ON ([Target].[PageId] = [Source].[PageId])
WHEN MATCHED AND (
	NULLIF([Source].[AliasTitle], [Target].[AliasTitle]) IS NOT NULL OR NULLIF([Target].[AliasTitle], [Source].[AliasTitle]) IS NOT NULL OR 
	NULLIF([Source].[MetaDescription], [Target].[MetaDescription]) IS NOT NULL OR NULLIF([Target].[MetaDescription], [Source].[MetaDescription]) IS NOT NULL OR 
	NULLIF([Source].[Title], [Target].[Title]) IS NOT NULL OR NULLIF([Target].[Title], [Source].[Title]) IS NOT NULL OR 
	NULLIF([Source].[MainImage], [Target].[MainImage]) IS NOT NULL OR NULLIF([Target].[MainImage], [Source].[MainImage]) IS NOT NULL OR 
	NULLIF([Source].[MainImageAlt], [Target].[MainImageAlt]) IS NOT NULL OR NULLIF([Target].[MainImageAlt], [Source].[MainImageAlt]) IS NOT NULL OR 
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL OR 
	NULLIF([Source].[TemplateId], [Target].[TemplateId]) IS NOT NULL OR NULLIF([Target].[TemplateId], [Source].[TemplateId]) IS NOT NULL OR 
	NULLIF([Source].[Visible], [Target].[Visible]) IS NOT NULL OR NULLIF([Target].[Visible], [Source].[Visible]) IS NOT NULL) THEN
 UPDATE SET
  [AliasTitle] = [Source].[AliasTitle], 
  [MetaDescription] = [Source].[MetaDescription], 
  [Title] = [Source].[Title], 
  [MainImage] = [Source].[MainImage], 
  [MainImageAlt] = [Source].[MainImageAlt], 
  [Description] = [Source].[Description], 
  [TemplateId] = [Source].[TemplateId], 
  [Visible] = [Source].[Visible]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([PageId],[AliasTitle],[MetaDescription],[Title],[MainImage],[MainImageAlt],[Description],[TemplateId],[Visible])
 VALUES([Source].[PageId],[Source].[AliasTitle],[Source].[MetaDescription],[Source].[Title],[Source].[MainImage],[Source].[MainImageAlt],[Source].[Description],[Source].[TemplateId],[Source].[Visible]);

GO
SET IDENTITY_INSERT [dbo].[Page] OFF

GO

SET IDENTITY_INSERT [dbo].[ReviewType] ON 
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 1, 'Booking.com'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Booking.com')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 2, 'Airbnb'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Airbnb')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 3, 'VRBO'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'VRBO')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 4, 'Trip Advisor'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Trip Advisor')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 5, 'Google'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Google')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 6, 'Facebook'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Facebook')
GO
INSERT INTO [dbo].[ReviewType]  ([ReviewTypeId], [Description])        
SELECT 7, 'Website'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[ReviewType]  WHERE [Description] = 'Website')
GO
SET IDENTITY_INSERT [dbo].[ReviewType] OFF

GO

SET IDENTITY_INSERT [dbo].[RestrictionType] ON 
GO
INSERT INTO [dbo].[RestrictionType] ([ID], [Description])        
SELECT 1, 'Sorry No Smokers'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[RestrictionType] WHERE [Description] = 'Sorry No Smokers')   
GO
INSERT INTO [dbo].[RestrictionType] ([ID], [Description])        
SELECT 2, 'Sorry No Pets'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[RestrictionType] WHERE [Description] = 'Sorry No Pets')  
GO
INSERT INTO [dbo].[RestrictionType] ([ID], [Description])        
SELECT 3, 'Strictly No Smokers'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[RestrictionType] WHERE [Description] = 'Strictly No Smokers')  
GO
INSERT INTO [dbo].[RestrictionType] ([ID], [Description])        
SELECT 4, 'Strictly No Pets'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[RestrictionType] WHERE [Description] = 'Strictly No Pets')  
GO
SET IDENTITY_INSERT [dbo].[RestrictionType] OFF

GO

SET IDENTITY_INSERT [dbo].[FurnishingType] ON 
GO
INSERT INTO [dbo].[FurnishingType] ([ID], [Description])        
SELECT 1, 'Furnished'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[FurnishingType] WHERE [Description] = 'Furnished')   
GO
INSERT INTO [dbo].[FurnishingType] ([ID], [Description])        
SELECT 2, 'Part-furnished'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[FurnishingType] WHERE [Description] = 'Part-furnished')
GO
INSERT INTO [dbo].[FurnishingType] ([ID], [Description])        
SELECT 3, 'Unfurnished'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[FurnishingType] WHERE [Description] = 'Unfurnished')
GO
INSERT INTO [dbo].[FurnishingType] ([ID], [Description])        
SELECT 4, 'Furnished/Unfurnished/Part-furnished'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[FurnishingType] WHERE [Description] = 'Furnished/Unfurnished/Part-furnished')
GO
INSERT INTO [dbo].[FurnishingType] ([ID], [Description])        
SELECT 5, 'Fully Furnished Or Unfurnished'    
WHERE NOT EXISTS(SELECT 1 FROM [dbo].[FurnishingType] WHERE [Description] = 'Fully Furnished Or Unfurnished')
GO
SET IDENTITY_INSERT [dbo].[FurnishingType] OFF

GO

SET IDENTITY_INSERT [dbo].[HouseType] ON 
GO
MERGE INTO [dbo].[housetype] AS [Target]
USING (VALUES
  (1,N'House')
 ,(2,N'Flat/Apartment')
 ,(3,N'Bungalow')
 ,(4,N'Character Property')
 ,(5,N'Land')
 ,(6,N'Guest House/Hotel')
 ,(7,N'Mobile/Park Home')
 ,(8,N'Commercial Property')
 ,(9,N'Retirement Property')
 ,(10,N'House/Flat Share')
 ,(11,N'Garage/Parking')
) AS [Source] ([ID],[Description])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = [Source].[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[Description])
 VALUES([Source].[ID],[Source].[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
SET IDENTITY_INSERT [dbo].[HouseType] OFF

GO

SET IDENTITY_INSERT [dbo].[SubHouseType] ON
GO
MERGE INTO [dbo].[SubHouseType] AS [Target]
USING (VALUES
  (1,N'Chalet',1)
 ,(2,N'Cluster House',1)
 ,(3,N'Cottage',1)
 ,(4,N'Detached',1)
 ,(5,N'End of Terrace',1)
 ,(6,N'Finca',1)
 ,(7,N'House',1)
 ,(8,N'Link Detached House',1)
 ,(9,N'Mews',1)
 ,(10,N'Semi-Detached',1)
 ,(11,N'Terraced',1)
 ,(12,N'Town House',1)
 ,(13,N'Villa',1)
 ,(14,N'Village House',1)
 ,(15,N'Semi-detached Villa',1)
 ,(16,N'Detached Villa',1)
 ,(17,N'Apartment',2)
 ,(18,N'Flat',2)
 ,(19,N'Ground Flat',2)
 ,(20,N'Ground Maisonette',2)
 ,(21,N'Maisonette',2)
 ,(22,N'Penthouse',2)
 ,(23,N'Serviced Apartments',2)
 ,(24,N'Studio',2)
 ,(25,N'Duplex',2)
 ,(26,N'Triplex',2)
 ,(27,N'Hotel Room',2)
 ,(28,N'Block of Apartments',2)
 ,(29,N'Bungalow',3)
 ,(30,N'Detached Bungalow',3)
 ,(31,N'Semi-Detached Bungalow',3)
 ,(32,N'Terraced Bungalow',3)
 ,(33,N'Character Property',4)
 ,(34,N'Barn Conversion',4)
 ,(35,N'Equestrian',4)
 ,(36,N'Riad',4)
 ,(37,N'Farm House',4)
 ,(38,N'Longere',4)
 ,(39,N'Gite',4)
 ,(40,N'Barn',4)
 ,(41,N'Trulli',4)
 ,(42,N'Mill',4)
 ,(43,N'Ruins',4)
 ,(44,N'Castle',4)
 ,(45,N'Cortijo',4)
 ,(46,N'Cave House',4)
 ,(47,N'Country House',4)
 ,(48,N'Stone House',4)
 ,(49,N'Lodge',4)
 ,(50,N'Log Cabin',4)
 ,(51,N'Manor House',4)
 ,(52,N'Stately Home',4)
 ,(53,N'House Boat',4)
 ,(54,N'Land',5)
 ,(55,N'Farm Land',5)
 ,(56,N'Plot',5)
 ,(57,N'Off-Plan',5)
 ,(58,N'Guest House',6)
 ,(59,N'Hotel',6)
 ,(60,N'Mobile Home',7)
 ,(61,N'Park Home',7)
 ,(62,N'Caravan',7)
 ,(63,N'Commercial Property',8)
 ,(64,N'Restaurant',8)
 ,(65,N'Cafe',8)
 ,(66,N'Mill',8)
 ,(67,N'Trulli',8)
 ,(68,N'Bar',8)
 ,(69,N'Shop',8)
 ,(70,N'Retirement Property',9)
 ,(71,N'Sheltered Housing',9)
 ,(72,N'Flat Share',10)
 ,(73,N'House Share',10)
 ,(74,N'Garages',11)
 ,(75,N'Parking',11)
) AS [Source] ([ID],[SubHouseType],[HouseTypeId])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[SubHouseType], [Target].[SubHouseType]) IS NOT NULL OR NULLIF([Target].[SubHouseType], [Source].[SubHouseType]) IS NOT NULL OR 
	NULLIF([Source].[HouseTypeId], [Target].[HouseTypeId]) IS NOT NULL OR NULLIF([Target].[HouseTypeId], [Source].[HouseTypeId]) IS NOT NULL) THEN
 UPDATE SET
  [SubHouseType] = [Source].[SubHouseType], 
  [HouseTypeId] = [Source].[HouseTypeId]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[SubHouseType],[HouseTypeId])
 VALUES([Source].[ID],[Source].[SubHouseType],[Source].[HouseTypeId])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE; 
SET IDENTITY_INSERT [dbo].[SubHouseType] OFF

GO

SET IDENTITY_INSERT [dbo].[FeatureType] ON 
MERGE INTO [dbo].[FeatureType] AS [Target]
USING (VALUES
  (1,N'Desirable Location')
 ,(2,N'Solid Oak Flooring')
 ,(3,N'Spacious Lounge')
 ,(4,N'Spacious Study/Living Space')
 ,(5,N'Fully Fitted Kitchen')
 ,(6,N'Separate Dining Room')
 ,(7,N'Conservatory')
 ,(9,N'Family Bathroom')
 ,(10,N'Gates & Double Garages')
 ,(11,N'Alarm')
 ,(12,N'Central Heating')
 ,(13,N'Dining Room')
 ,(14,N'Dishwasher')
 ,(15,N'Double Glazed')
 ,(16,N'Garage')
 ,(17,N'Garden')
 ,(18,N'Laminate Flooring')
 ,(19,N'Lounge')
 ,(20,N'Parking')
 ,(21,N'Washing Machine')
 ,(22,N'White Goods')
 ,(24,N'Over Looking Canal')
 ,(26,N'Ground Floor')
 ,(27,N'Yard & Front Garden')
 ,(28,N'Close to Village Centre')
 ,(29,N'Cottage Style Property')
 ,(30,N'Fitted Kitchen')
 ,(31,N'South Facing Garden')
 ,(32,N'Sky TV')
 ,(33,N'Vendor Will Pay Stamp Duty')
 ,(34,N'Cellar')
) AS [Source] ([ID],[Description])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = [Source].[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[Description])
 VALUES([Source].[ID],[Source].[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
SET IDENTITY_INSERT [dbo].[FeatureType] OFF

GO

SET IDENTITY_INSERT [dbo].[Staff] ON
GO
MERGE INTO [dbo].[Staff] AS [Target]
USING (VALUES
  (1,N'Kath',30, N'Letting Agent',N'',N'https://www.linkedin.com/in/kathryn-chambers-67969a175/',N'kath.jpg',1)
 ,(2,N'Matthew',10, N'IT Consultant',N'',N'https://www.linkedin.com/in/matt-chambers-2bbba716/',N'matthew.jpg',1)
) AS [Source] ([ID],[Name],[YearsExperience],[JobTitle],[ProfileBio],[LinkedInLink],[ImagePath],[Visible])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[Name], [Target].[Name]) IS NOT NULL OR NULLIF([Target].[Name], [Source].[Name]) IS NOT NULL OR 
	NULLIF([Source].[YearsExperience], [Target].[YearsExperience]) IS NOT NULL OR NULLIF([Target].[YearsExperience], [Source].[YearsExperience]) IS NOT NULL OR 
	NULLIF([Source].[JobTitle], [Target].[JobTitle]) IS NOT NULL OR NULLIF([Target].[JobTitle], [Source].[JobTitle]) IS NOT NULL OR 
	NULLIF([Source].[ProfileBio], [Target].[ProfileBio]) IS NOT NULL OR NULLIF([Target].[ProfileBio], [Source].[ProfileBio]) IS NOT NULL OR 
	NULLIF([Source].[LinkedInLink], [Target].[LinkedInLink]) IS NOT NULL OR NULLIF([Target].[LinkedInLink], [Source].[LinkedInLink]) IS NOT NULL OR 
	NULLIF([Source].[ImagePath], [Target].[ImagePath]) IS NOT NULL OR NULLIF([Target].[ImagePath], [Source].[ImagePath]) IS NOT NULL OR 
	NULLIF([Source].[Visible], [Target].[Visible]) IS NOT NULL OR NULLIF([Target].[Visible], [Source].[Visible]) IS NOT NULL) THEN
 UPDATE SET
  [Name] = [Source].[Name], 
  [YearsExperience] = [Source].[YearsExperience], 
  [JobTitle] = [Source].[JobTitle], 
  [ProfileBio] = [Source].[ProfileBio], 
  [LinkedInLink] = [Source].[LinkedInLink], 
  [ImagePath] = [Source].[ImagePath], 
  [Visible] = [Source].[Visible]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[Name],[YearsExperience],[JobTitle],[ProfileBio],[LinkedInLink],[ImagePath],[Visible])
 VALUES([Source].[ID],[Source].[Name],[Source].[YearsExperience],[Source].[JobTitle],[Source].[ProfileBio],[Source].[LinkedInLink],[Source].[ImagePath],[Source].[Visible]);
SET IDENTITY_INSERT [dbo].[Staff] OFF

GO

SET IDENTITY_INSERT [dbo].[SizeUnitType] ON
GO
MERGE INTO [dbo].[SizeUnitType] AS [Target]
USING (VALUES
  (1,N'sq. metres')
 ,(2,N'sq. yards')
 ,(3,N'hectares')
 ,(4,N'acres')
) AS [Source] ([ID],[Description])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL) THEN
 UPDATE SET
  [Description] = [Source].[Description]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[Description])
 VALUES([Source].[ID],[Source].[Description])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
SET IDENTITY_INSERT [dbo].[SizeUnitType] OFF

GO

SET IDENTITY_INSERT [dbo].[Slideshow] ON
GO
MERGE INTO [dbo].[Slideshow] AS [Target]
USING (VALUES
  (1,N'lymm-house.jpg','Lymm House','Lymm House',N'Entire home in Lymm, Cheshire.',
  N'Entire home in Lymm, Cheshire.',N'/property/detail/2',2,1),
  (2,N'lymm-village-apartment.jpg','Lymm Village Apartment','Lymm Village Apartment',N'A spacious apartment located in the heart of Lymm Village.',
  N'A spacious apartment located in the heart of Lymm Village.',N'/property/detail/1',1,1),
  (3,N'lymm-cottage.jpg','Lymm Cottage','Lymm Cottage',N'Entire cottage in Lymm, Cheshire.',
  N'Entire cottage in Lymm, Cheshire.',N'/property/detail/3',3,1)
) AS [Source] ([ID],[ImagePath],[ImagePathAlt],[Caption],[CaptionTitle],[ShortMobileCaption],[Link],[SequenceOrder],[Visible])
ON ([Target].[Id] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[ImagePath], [Target].[ImagePath]) IS NOT NULL OR NULLIF([Target].[ImagePath], [Source].[ImagePath]) IS NOT NULL OR 
	NULLIF([Source].[ImagePathAlt], [Target].[ImagePathAlt]) IS NOT NULL OR NULLIF([Target].[ImagePathAlt], [Source].[ImagePathAlt]) IS NOT NULL OR 
	NULLIF([Source].[Caption], [Target].[Caption]) IS NOT NULL OR NULLIF([Target].[Caption], [Source].[Caption]) IS NOT NULL OR 
	NULLIF([Source].[CaptionTitle], [Target].[CaptionTitle]) IS NOT NULL OR NULLIF([Target].[CaptionTitle], [Source].[CaptionTitle]) IS NOT NULL OR 
	NULLIF([Source].[ShortMobileCaption], [Target].[ShortMobileCaption]) IS NOT NULL OR NULLIF([Target].[ShortMobileCaption], [Source].[ShortMobileCaption]) IS NOT NULL OR 
	NULLIF([Source].[Link], [Target].[Link]) IS NOT NULL OR NULLIF([Target].[Link], [Source].[Link]) IS NOT NULL OR 
	NULLIF([Source].[SequenceOrder], [Target].[SequenceOrder]) IS NOT NULL OR NULLIF([Target].[SequenceOrder], [Source].[SequenceOrder]) IS NOT NULL OR 
	NULLIF([Source].[Visible], [Target].[Visible]) IS NOT NULL OR NULLIF([Target].[Visible], [Source].[Visible]) IS NOT NULL) THEN
 UPDATE SET
  [ImagePath] = [Source].[ImagePath], 
  [ImagePathAlt] = [Source].[ImagePathAlt], 
  [Caption] = [Source].[Caption], 
  [CaptionTitle] = [Source].[CaptionTitle], 
  [ShortMobileCaption] = [Source].[ShortMobileCaption],
  [Link] = [Source].[Link], 
  [SequenceOrder] = [Source].[SequenceOrder],  
  [Visible] = [Source].[Visible]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[ImagePath],[ImagePathAlt],[Caption],[CaptionTitle],[ShortMobileCaption],[Link],[SequenceOrder],[Visible])
 VALUES([Source].[ID],[Source].[ImagePath],[Source].[ImagePathAlt],[Source].[Caption],[Source].[CaptionTitle],[Source].[ShortMobileCaption],[Source].[Link],[Source].[SequenceOrder],[Source].[Visible]);
 SET IDENTITY_INSERT [dbo].[Slideshow] OFF

GO

SET IDENTITY_INSERT [dbo].[GeoLocation] ON
GO
MERGE INTO [dbo].[GeoLocation] AS [Target]
USING (VALUES
  (1,53.380915699999996,-2.4791106999999997,19,53.38090722730504,-2.4794449534595806,93.16289611265945,-0.19483784909857604,3.2246421403255385)
 ,(2,53.379527854036276,-2.4858714660912127,19,53.379527854036276,-2.4858714660912127,343.3876913518914,-1.2166272444616482,1.224642140325539)
) AS [Source] ([ID],[Latitude],[Longitude],[MapZoom],[StreetViewLatitude],[StreetViewLongitude],[Yaw],[Pitch],[Zoom])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[Latitude], [Target].[Latitude]) IS NOT NULL OR NULLIF([Target].[Latitude], [Source].[Latitude]) IS NOT NULL OR 
	NULLIF([Source].[Longitude], [Target].[Longitude]) IS NOT NULL OR NULLIF([Target].[Longitude], [Source].[Longitude]) IS NOT NULL OR 
	NULLIF([Source].[Zoom], [Target].[Zoom]) IS NOT NULL OR NULLIF([Target].[Zoom], [Source].[Zoom]) IS NOT NULL OR 
	NULLIF([Source].[MapZoom], [Target].[MapZoom]) IS NOT NULL OR NULLIF([Target].[MapZoom], [Source].[MapZoom]) IS NOT NULL OR 
	NULLIF([Source].[Yaw], [Target].[Yaw]) IS NOT NULL OR NULLIF([Target].[Yaw], [Source].[Yaw]) IS NOT NULL OR 
	NULLIF([Source].[Pitch], [Target].[Pitch]) IS NOT NULL OR NULLIF([Target].[Pitch], [Source].[Pitch]) IS NOT NULL) THEN
 UPDATE SET
  [Latitude] = [Source].[Latitude], 
  [Longitude] = [Source].[Longitude], 
  [Zoom] = [Source].[Zoom], 
  [Yaw] = [Source].[Yaw], 
  [Pitch] = [Source].[Pitch],
  [MapZoom] = [Source].[MapZoom]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[Latitude],[Longitude],[MapZoom],[StreetViewLatitude],[StreetViewLongitude],[Yaw],[Pitch],[Zoom])
 VALUES([Source].[ID],[Source].[Latitude],[Source].[Longitude],[Source].[MapZoom],[Source].[StreetViewLatitude],[Source].[StreetViewLongitude],[Source].[Yaw],[Source].[Pitch],[Source].[Zoom]);
SET IDENTITY_INSERT [dbo].[GeoLocation] OFF

GO

SET IDENTITY_INSERT [dbo].[Address] ON
GO
MERGE INTO [dbo].[Address] AS [Target]
USING (VALUES
  (1,N'2 Bridgewater Street', NULL, N'Lymm', N'Cheshire', N'WA13 0AB', N'England'),
  (2,N'5 Church Road', NULL, N'Lymm', N'Cheshire', N'WA13 0QG', N'England'),
  (3,N'53 Chaise Meadow', NULL, N'Lymm', N'Cheshire', N'WA13 9NX', N'England')
) AS [Source] ([ID],[AddressLineOne],[AddressLineTwo],[TownOrCity],[County],[Postcode],[Country])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[AddressLineOne], [Target].[AddressLineOne]) IS NOT NULL OR NULLIF([Target].[AddressLineOne], [Source].[AddressLineOne]) IS NOT NULL OR 
	NULLIF([Source].[AddressLineTwo], [Target].[AddressLineTwo]) IS NOT NULL OR NULLIF([Target].[AddressLineTwo], [Source].[AddressLineTwo]) IS NOT NULL OR 
	NULLIF([Source].[TownOrCity], [Target].[TownOrCity]) IS NOT NULL OR NULLIF([Target].[TownOrCity], [Source].[TownOrCity]) IS NOT NULL OR 
	NULLIF([Source].[County], [Target].[County]) IS NOT NULL OR NULLIF([Target].[County], [Source].[County]) IS NOT NULL OR 
	NULLIF([Source].[Postcode], [Target].[Postcode]) IS NOT NULL OR NULLIF([Target].[Postcode], [Source].[Postcode]) IS NOT NULL OR 
	NULLIF([Source].[Country], [Target].[Country]) IS NOT NULL OR NULLIF([Target].[Country], [Source].[Country]) IS NOT NULL) THEN
 UPDATE SET
  [AddressLineOne] = [Source].[AddressLineOne], 
  [AddressLineTwo] = [Source].[AddressLineTwo], 
  [County] = [Source].[County], 
  [Postcode] = [Source].[Postcode], 
  [Country] = [Source].[Country]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[AddressLineOne],[AddressLineTwo],[TownOrCity],[County],[Postcode],[Country])
 VALUES([Source].[ID],[Source].[AddressLineOne],[Source].[AddressLineTwo],[Source].[TownOrCity],[Source].[County],[Source].[Postcode],[Source].[Country]);
SET IDENTITY_INSERT [dbo].[Address] OFF

GO

SET IDENTITY_INSERT [dbo].[Property] ON
GO
MERGE INTO [dbo].[Property] AS [Target]
USING (VALUES
  (1, N'Lymm Village Apartment', 1, 1, 17, 1, 2 ,1,	2, 1, 1, 1, NULL, NULL, NULL,
  100.00, 3, NULL, '16:00:00', '10:00:00', 5, 1, 5, 4, 5, 1, 1, 1, NULL, NULL, NULL, NULL, NULL, 1 ,1, '2023-10-05 19:40:50',NULL),
    (2, N'Lymm House', 2, 2, 7, 1,	3, 1.5,	2, 1, 1, 1,	NULL, NULL,	NULL, 100.00, 3, NULL, '16:00:00', '10:00:00', 5, 1, 5, 4, 5, 
    0, 0, NULL,	2, NULL, NULL, NULL, NULL, 1, 1, '2023-10-05 19:40:50',NULL),
  (3, N'Lymm Cottage', 2, 3,	3, 1, 2, 1,	2, 1, 1, 1,	NULL, NULL,	NULL, 100.00, 3, NULL, '16:00:00', '10:00:00', 5, 1, 5, 4, 5, 
	1, 1, 2, NULL, NULL,	NULL, NULL,	NULL, 1, 1, '2023-10-05 19:40:50', NULL)
) AS [Source] ([ID],[FriendlyName],[StaffId],[AddressId],[SubHouseTypeId],[FurnishingTypeId]
      ,[Bedroom],[Bathroom],[Floor],[ReceptionRoom],[Kitchen],[LivingRoom]
      ,[YearBuilt],[DisplayAddress],[Description],[DefaultNightlyPrice],[DefaultMinimumStay],[DefaultMaximumStay],
	 [CheckInTimeAfter],[CheckOutTimeBefore],[MaximumNumberOfGuests],[MinimumNumberOfAdult],
	[MaximumNumberOfAdult],[MaximumNumberOfChildren],[MaximumNumberOfInfants],[ShowGoogleMap]
      ,[ShowStreetView],[GeoLocationId],[CarSpace],[Size],[SizeUnitTypeId],[VideoHtml]
      ,[Disclaimer],[ShowOnHomepage],[ShowOnSite],[Created],[Updated])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
	NULLIF([Source].[FriendlyName], [Target].[FriendlyName]) IS NOT NULL OR NULLIF([Target].[FriendlyName], [Source].[FriendlyName]) IS NOT NULL OR 
	NULLIF([Source].[StaffId], [Target].[StaffId]) IS NOT NULL OR NULLIF([Target].[StaffId], [Source].[StaffId]) IS NOT NULL OR 
    NULLIF([Source].[AddressId], [Target].[AddressId]) IS NOT NULL OR NULLIF([Target].[AddressId], [Source].[AddressId]) IS NOT NULL OR 
	NULLIF([Source].[SubHouseTypeId], [Target].[SubHouseTypeId]) IS NOT NULL OR NULLIF([Target].[SubHouseTypeId], [Source].[SubHouseTypeId]) IS NOT NULL OR 
	NULLIF([Source].[FurnishingTypeId], [Target].[FurnishingTypeId]) IS NOT NULL OR NULLIF([Target].[FurnishingTypeId], [Source].[FurnishingTypeId]) IS NOT NULL OR 
	NULLIF([Source].[Bedroom], [Target].[Bedroom]) IS NOT NULL OR NULLIF([Target].[Bedroom], [Source].[Bedroom]) IS NOT NULL OR 
	NULLIF([Source].[Bathroom], [Target].[Bathroom]) IS NOT NULL OR NULLIF([Target].[Bathroom], [Source].[Bathroom]) IS NOT NULL OR 
	NULLIF([Source].[Floor], [Target].[Floor]) IS NOT NULL OR NULLIF([Target].[Floor], [Source].[Floor]) IS NOT NULL OR 
	NULLIF([Source].[ReceptionRoom], [Target].[ReceptionRoom]) IS NOT NULL OR NULLIF([Target].[ReceptionRoom], [Source].[ReceptionRoom]) IS NOT NULL OR 
	NULLIF([Source].[Kitchen], [Target].[Kitchen]) IS NOT NULL OR NULLIF([Target].[Kitchen], [Source].[Kitchen]) IS NOT NULL OR 
	NULLIF([Source].[LivingRoom], [Target].[LivingRoom]) IS NOT NULL OR NULLIF([Target].[LivingRoom], [Source].[LivingRoom]) IS NOT NULL OR 
	NULLIF([Source].[YearBuilt], [Target].[YearBuilt]) IS NOT NULL OR NULLIF([Target].[YearBuilt], [Source].[YearBuilt]) IS NOT NULL OR 
	NULLIF([Source].[DisplayAddress], [Target].[DisplayAddress]) IS NOT NULL OR NULLIF([Target].[DisplayAddress], [Source].[DisplayAddress]) IS NOT NULL OR 
	NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL OR 
	NULLIF([Source].[DefaultNightlyPrice], [Target].[DefaultNightlyPrice]) IS NOT NULL OR NULLIF([Target].[DefaultNightlyPrice], [Source].[DefaultNightlyPrice]) IS NOT NULL OR 
	NULLIF([Source].[DefaultMinimumStay], [Target].[DefaultMinimumStay]) IS NOT NULL OR NULLIF([Target].[DefaultMinimumStay], [Source].[DefaultMinimumStay]) IS NOT NULL OR 
	NULLIF([Source].[DefaultMaximumStay], [Target].[DefaultMaximumStay]) IS NOT NULL OR NULLIF([Target].[DefaultMaximumStay], [Source].[DefaultMaximumStay]) IS NOT NULL OR 
	NULLIF([Source].[CheckInTimeAfter], [Target].[CheckInTimeAfter]) IS NOT NULL OR NULLIF([Target].[CheckInTimeAfter], [Source].[CheckInTimeAfter]) IS NOT NULL OR 
	NULLIF([Source].[CheckOutTimeBefore], [Target].[CheckOutTimeBefore]) IS NOT NULL OR NULLIF([Target].[CheckOutTimeBefore], [Source].[CheckOutTimeBefore]) IS NOT NULL OR
    NULLIF([Source].[MaximumNumberOfGuests], [Target].[MaximumNumberOfGuests]) IS NOT NULL OR NULLIF([Target].[MaximumNumberOfGuests], [Source].[MaximumNumberOfGuests]) IS NOT NULL OR 
	NULLIF([Source].[MinimumNumberOfAdult], [Target].[MinimumNumberOfAdult]) IS NOT NULL OR NULLIF([Target].[MinimumNumberOfAdult], [Source].[MinimumNumberOfAdult]) IS NOT NULL OR 
	NULLIF([Source].[MaximumNumberOfAdult], [Target].[MaximumNumberOfAdult]) IS NOT NULL OR NULLIF([Target].[MaximumNumberOfAdult], [Source].[MaximumNumberOfAdult]) IS NOT NULL OR 
	NULLIF([Source].[MaximumNumberOfChildren], [Target].[MaximumNumberOfChildren]) IS NOT NULL OR NULLIF([Target].[MaximumNumberOfChildren], [Source].[MaximumNumberOfChildren]) IS NOT NULL OR 
	NULLIF([Source].[ShowGoogleMap], [Target].[ShowGoogleMap]) IS NOT NULL OR NULLIF([Target].[ShowGoogleMap], [Source].[ShowGoogleMap]) IS NOT NULL OR 
	NULLIF([Source].[ShowStreetView], [Target].[ShowStreetView]) IS NOT NULL OR NULLIF([Target].[ShowStreetView], [Source].[ShowStreetView]) IS NOT NULL OR 
	NULLIF([Source].[GeoLocationId], [Target].[GeoLocationId]) IS NOT NULL OR NULLIF([Target].[GeoLocationId], [Source].[GeoLocationId]) IS NOT NULL OR 
	NULLIF([Source].[CarSpace], [Target].[CarSpace]) IS NOT NULL OR NULLIF([Target].[CarSpace], [Source].[CarSpace]) IS NOT NULL OR 
	NULLIF([Source].[Size], [Target].[Size]) IS NOT NULL OR NULLIF([Target].[Size], [Source].[Size]) IS NOT NULL OR 
	NULLIF([Source].[SizeUnitTypeId], [Target].[SizeUnitTypeId]) IS NOT NULL OR NULLIF([Target].[SizeUnitTypeId], [Source].[SizeUnitTypeId]) IS NOT NULL OR 
	NULLIF([Source].[VideoHtml], [Target].[VideoHtml]) IS NOT NULL OR NULLIF([Target].[VideoHtml], [Source].[VideoHtml]) IS NOT NULL OR 
	NULLIF([Source].[Disclaimer], [Target].[Disclaimer]) IS NOT NULL OR NULLIF([Target].[Disclaimer], [Source].[Disclaimer]) IS NOT NULL OR 
	NULLIF([Source].[ShowOnHomepage], [Target].[ShowOnHomepage]) IS NOT NULL OR NULLIF([Target].[ShowOnHomepage], [Source].[ShowOnHomepage]) IS NOT NULL OR 
	NULLIF([Source].[ShowOnSite], [Target].[ShowOnSite]) IS NOT NULL OR NULLIF([Target].[ShowOnSite], [Source].[ShowOnSite]) IS NOT NULL OR 
	NULLIF([Source].[Created], [Target].[Created]) IS NOT NULL OR NULLIF([Target].[Created], [Source].[Created]) IS NOT NULL OR 
	NULLIF([Source].[Updated], [Target].[Updated]) IS NOT NULL OR NULLIF([Target].[Updated], [Source].[Updated]) IS NOT NULL)
THEN
  UPDATE 
   SET [FriendlyName] = [Source].[FriendlyName]
      ,[StaffId] = [Source].[StaffId]
	  ,[AddressId] = [Source].[AddressId]
	  ,[SubHouseTypeId] = [Source].[SubHouseTypeId]
      ,[FurnishingTypeId] = [Source].[FurnishingTypeId] 
      ,[Bedroom] = [Source].[Bedroom] 
      ,[Bathroom] = [Source].[Bathroom] 
      ,[Floor] = [Source].[Floor] 
      ,[ReceptionRoom] = [Source].[ReceptionRoom]
      ,[Kitchen] = [Source].[Kitchen]
      ,[LivingRoom] = [Source].[LivingRoom]
      ,[YearBuilt] = [Source].[YearBuilt]
      ,[DisplayAddress] = [Source].[DisplayAddress] 
      ,[Description] = [Source].[Description]
      ,[DefaultNightlyPrice] = [Source].[DefaultNightlyPrice]
      ,[DefaultMinimumStay] = [Source].[DefaultMinimumStay]
      ,[DefaultMaximumStay] = [Source].[DefaultMaximumStay]
      ,[CheckInTimeAfter] = [Source].[CheckInTimeAfter]
      ,[CheckOutTimeBefore] = [Source].[CheckOutTimeBefore]
      ,[MaximumNumberOfGuests] = [Source].[MaximumNumberOfGuests]
	  ,[MinimumNumberOfAdult] = [Source].[MinimumNumberOfAdult]
      ,[MaximumNumberOfAdult] = [Source].[MaximumNumberOfAdult]
      ,[MaximumNumberOfChildren] = [Source].[MaximumNumberOfChildren]
      ,[MaximumNumberOfInfants] =[Source].[MaximumNumberOfInfants]
	  ,[ShowGoogleMap] = [Source].[ShowGoogleMap]
      ,[ShowStreetView] = [Source].[ShowStreetView]
      ,[GeoLocationId] = [Source].[GeoLocationId]
      ,[CarSpace] = [Source].[CarSpace]
      ,[Size] = [Source].[Size]
      ,[SizeUnitTypeId] = [Source].[SizeUnitTypeId]
      ,[VideoHtml] = [Source].[VideoHtml] 
      ,[Disclaimer] = [Source].[Disclaimer]
      ,[ShowOnHomepage] = [Source].[ShowOnHomepage]
      ,[ShowOnSite] = [Source].[ShowOnSite]
      ,[Created] = [Source].[Created]
      ,[Updated] = [Source].[Updated]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[FriendlyName],[StaffId],[AddressId],[SubHouseTypeId]
      ,[FurnishingTypeId],[Bedroom],[Bathroom],[Floor],[ReceptionRoom]
      ,[Kitchen],[LivingRoom],[YearBuilt],[DisplayAddress]
      ,[Description],[DefaultNightlyPrice],[DefaultMinimumStay],[DefaultMaximumStay],[CheckInTimeAfter]
    ,[CheckOutTimeBefore],[MaximumNumberOfGuests],[MinimumNumberOfAdult],[MaximumNumberOfAdult],[MaximumNumberOfChildren]
    ,[MaximumNumberOfInfants],[ShowGoogleMap],[ShowStreetView],[GeoLocationId],[CarSpace],[Size],[SizeUnitTypeId]
      ,[VideoHtml],[Disclaimer],[ShowOnHomepage],[ShowOnSite],[Created],[Updated])
 VALUES([Source].[ID],[Source].[FriendlyName],[Source].[StaffId],[Source].[AddressId],[Source].[SubHouseTypeId],
        [Source].[FurnishingTypeId],[Source].[Bedroom],[Source].[Bathroom],[Source].[Floor],[Source].[ReceptionRoom]
       ,[Source].[Kitchen],[Source].[LivingRoom],[Source].[YearBuilt],[Source].[DisplayAddress]
       ,[Source].[Description],[Source].[DefaultNightlyPrice],[Source].[DefaultMinimumStay],[Source].[DefaultMaximumStay],[Source].[CheckInTimeAfter]
       ,[Source].[CheckOutTimeBefore],[Source].[MaximumNumberOfGuests],[Source].[MinimumNumberOfAdult],[Source].[MaximumNumberOfAdult],[Source].[MaximumNumberOfChildren]
       ,[Source].[MaximumNumberOfInfants],[Source].[ShowGoogleMap], [Source].[ShowStreetView],[Source].[GeoLocationId],[Source].[CarSpace],[Source].[Size],[Source].[SizeUnitTypeId]
      , [Source].[VideoHtml],[Source].[Disclaimer],[Source].[ShowOnHomepage],[Source].[ShowOnSite],[Source].[Created],[Source].[Updated]); 
SET IDENTITY_INSERT [dbo].[Property] OFF

GO

MERGE INTO [dbo].[PropertyGuestEmailSchedule] AS [Target]
USING (VALUES
  (1, 1, 5, '2026-04-18 00:00:00', NULL)
) AS [Source] ([PropertyID],[IsEnabled],[SendDaysBeforeCheckIn],[Created],[Updated])
ON ([Target].[PropertyID] = [Source].[PropertyID])
WHEN MATCHED AND (
    NULLIF([Source].[IsEnabled], [Target].[IsEnabled]) IS NOT NULL OR NULLIF([Target].[IsEnabled], [Source].[IsEnabled]) IS NOT NULL OR
    NULLIF([Source].[SendDaysBeforeCheckIn], [Target].[SendDaysBeforeCheckIn]) IS NOT NULL OR NULLIF([Target].[SendDaysBeforeCheckIn], [Source].[SendDaysBeforeCheckIn]) IS NOT NULL OR
    NULLIF([Source].[Created], [Target].[Created]) IS NOT NULL OR NULLIF([Target].[Created], [Source].[Created]) IS NOT NULL OR
    NULLIF([Source].[Updated], [Target].[Updated]) IS NOT NULL OR NULLIF([Target].[Updated], [Source].[Updated]) IS NOT NULL)
THEN
  UPDATE
    SET [IsEnabled] = [Source].[IsEnabled],
        [SendDaysBeforeCheckIn] = [Source].[SendDaysBeforeCheckIn],
        [Created] = [Source].[Created],
        [Updated] = [Source].[Updated]
WHEN NOT MATCHED BY TARGET THEN
  INSERT([PropertyID],[IsEnabled],[SendDaysBeforeCheckIn],[Created],[Updated])
  VALUES([Source].[PropertyID],[Source].[IsEnabled],[Source].[SendDaysBeforeCheckIn],[Source].[Created],[Source].[Updated]);

GO

MERGE INTO [dbo].[PropertyGuestEmailTemplate] AS [Target]
USING (VALUES
  (1,
   'Your stay at {propertyname} starts soon',
   'You can arrive from {arrivaltime} on {arrivaldate}.',
   '<p style="margin:0 0 14px;">We are looking forward to welcoming you to {propertyname}.</p><p style="margin:0 0 18px;">Below is a clearer summary of the key details you may need before you arrive.</p><table role="presentation" style="width:100%;border-collapse:collapse;background-color:#f8fafc;border:1px solid #dbe3ea;border-radius:10px;margin:0 0 24px;"><tr><td style="padding:18px 20px;"><p style="margin:0 0 12px;font-size:12px;line-height:16px;font-weight:700;letter-spacing:1.3px;text-transform:uppercase;color:#6b7280;">Arrival summary</p><ul style="margin:0;padding-left:18px;"><li style="margin:0 0 6px;">Check-in is from <strong>{arrivaltime}</strong> on <strong>{arrivaldate}</strong>.</li><li style="margin:0 0 6px;">Please send us your chosen 4-6 digit door code before arrival if you have not already done so.</li><li style="margin:0 0 6px;"><strong>WiFi:</strong> PLUSNET-8XHQ / <strong>Password:</strong> 9e8a9c6464</li><li style="margin:0;"><strong>Parking:</strong> details and map are available here: <a href="https://bit.ly/3Maq0GJ" style="color:#2563eb;">Parking options</a></li></ul></td></tr></table><h3 style="margin:0 0 10px;font-size:18px;line-height:24px;font-weight:800;color:#111827;">Arrival and access</h3><ul style="margin:0 0 18px;padding-left:18px;"><li style="margin:0 0 6px;">The property will be ready on <strong>{arrivaldate}</strong> at <strong>{arrivaltime}</strong>.</li><li style="margin:0 0 6px;">Press the Yale button on the keypad, enter your code, then press the tick to unlock the front door.</li><li style="margin:0 0 6px;">The door locks automatically after 30 seconds. You can also press the Yale key when leaving.</li><li style="margin:0;">If you still need to choose an entry code, please send us a 4-6 digit code before you travel.</li></ul><h3 style="margin:0 0 10px;font-size:18px;line-height:24px;font-weight:800;color:#111827;">Parking</h3><ul style="margin:0 0 18px;padding-left:18px;"><li style="margin:0 0 6px;">Parking options are available here: <a href="https://bit.ly/3Maq0GJ" style="color:#2563eb;">Parking map and directions</a>.</li><li style="margin:0;">You can usually park on Grove Rise or Rosebank, and there are also two pay and display car parks nearby.</li></ul><h3 style="margin:0 0 10px;font-size:18px;line-height:24px;font-weight:800;color:#111827;">House essentials</h3><ul style="margin:0 0 18px;padding-left:18px;"><li style="margin:0 0 6px;"><strong>Heating and hot water:</strong> use the boiler in the kitchen. You can choose hot water only or heating plus hot water, then adjust the two temperature dials.</li><li style="margin:0 0 6px;"><strong>Alarm:</strong> the panel is in the hallway. Enter code <strong>1307</strong> to activate or deactivate it.</li><li style="margin:0 0 6px;"><strong>Bins:</strong> the bin marked 2b is at the back of the building. Use the alleyway beside the coffee shop, walk to the bottom, then turn right. The mortice gate key is by the hallway alarm on the leaf keyring.</li><li style="margin:0 0 6px;"><strong>Ironing board:</strong> behind the kitchen door.</li><li style="margin:0 0 6px;"><strong>Iron and hair dryer:</strong> stored in the hallway storage unit.</li><li style="margin:0;">The shower has two heads. Use the pull and push control beside the smaller head to switch between them.</li></ul><h3 style="margin:0 0 10px;font-size:18px;line-height:24px;font-weight:800;color:#111827;">Local area</h3><ul style="margin:0 0 18px;padding-left:18px;"><li style="margin:0 0 6px;">There is a Sainsburys Local in the village, plus a larger Sainsburys around 5 minutes away on Rushgreen Road.</li><li style="margin:0;">Popular spots include Lymm Dam, the Bridgewater Canal, and village pubs such as The Golden Fleece, The Church Green, The Jolly Thresher, and The Green Dragon.</li></ul><p style="margin:0 0 12px;">Thank you again for your booking. We hope you have a comfortable and enjoyable stay.</p><p style="margin:0;">Regards<br/>Matt and Kath</p>',
   '2026-04-18 00:00:00',
   NULL)
) AS [Source] ([PropertyID],[SubjectTemplate],[PreviewTextTemplate],[HtmlBody],[Created],[Updated])
ON ([Target].[PropertyID] = [Source].[PropertyID])
WHEN MATCHED AND (
    NULLIF([Source].[SubjectTemplate], [Target].[SubjectTemplate]) IS NOT NULL OR NULLIF([Target].[SubjectTemplate], [Source].[SubjectTemplate]) IS NOT NULL OR
    NULLIF([Source].[PreviewTextTemplate], [Target].[PreviewTextTemplate]) IS NOT NULL OR NULLIF([Target].[PreviewTextTemplate], [Source].[PreviewTextTemplate]) IS NOT NULL OR
    NULLIF([Source].[HtmlBody], [Target].[HtmlBody]) IS NOT NULL OR NULLIF([Target].[HtmlBody], [Source].[HtmlBody]) IS NOT NULL OR
    NULLIF([Source].[Created], [Target].[Created]) IS NOT NULL OR NULLIF([Target].[Created], [Source].[Created]) IS NOT NULL OR
    NULLIF([Source].[Updated], [Target].[Updated]) IS NOT NULL OR NULLIF([Target].[Updated], [Source].[Updated]) IS NOT NULL)
THEN
  UPDATE
    SET [SubjectTemplate] = [Source].[SubjectTemplate],
        [PreviewTextTemplate] = [Source].[PreviewTextTemplate],
        [HtmlBody] = [Source].[HtmlBody],
        [Created] = [Source].[Created],
        [Updated] = [Source].[Updated]
WHEN NOT MATCHED BY TARGET THEN
  INSERT([PropertyID],[SubjectTemplate],[PreviewTextTemplate],[HtmlBody],[Created],[Updated])
  VALUES([Source].[PropertyID],[Source].[SubjectTemplate],[Source].[PreviewTextTemplate],[Source].[HtmlBody],[Source].[Created],[Source].[Updated]);

GO

SET IDENTITY_INSERT [dbo].[ICal] ON
GO
MERGE INTO [dbo].[ICal] AS [Target]
USING (VALUES
  (1,1,N'D93DA743-F66D-4D8E-97E1-6F94D81E0ABE')
 ,(2,2,N'0E715D78-BFE3-4376-9D81-DD2D74F24311')
 ,(3,3,N'67300AE1-099D-4F89-AB58-64BC76D249A8')
) AS [Source] ([ID],[PropertyID],[Identifier])
ON ([Target].[ID] = [Source].[ID]
AND [Target].[PropertyID] = [Source].[PropertyID]
AND [Target].[Identifier] = [Source].[Identifier])
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID],[PropertyID],[Identifier])
 VALUES([Source].[ID],[Source].[PropertyID],[Source].[Identifier])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;
SET IDENTITY_INSERT [dbo].[ICal] OFF

GO

MERGE INTO [dbo].[PropertyAdditionalProduct] AS [Target]
USING (VALUES
  (1, N'prod_Ozdrv0LkJploWd', N'Cleaning', N'Cleaning', N'price_1OBeZxGmKSE7A4MVS1MQ5Z3X', 90.00, 1)
 ,(2, N'prod_Ozdrv0LkJploWd', N'Cleaning', N'Cleaning', N'price_1OBeZxGmKSE7A4MVS1MQ5Z3X', 90.00, 1)
 ,(3, N'prod_Ozdrv0LkJploWd', N'Cleaning', N'Cleaning', N'price_1OBeZxGmKSE7A4MVS1MQ5Z3X', 90.00, 1)
 ,(1, N'prod_OzduyRoqr5KXVN', N'Security Deposit', N'Security Deposit', N'price_1OBectGmKSE7A4MVSb9dplzq', 250.00, 1)
 ,(2, N'prod_OzduyRoqr5KXVN', N'Security Deposit', N'Security Deposit', N'price_1OBectGmKSE7A4MVSb9dplzq', 250.00, 1)
 ,(3, N'prod_OzduyRoqr5KXVN', N'Security Deposit', N'Security Deposit', N'price_1OBectGmKSE7A4MVSb9dplzq', 250.00, 1)
) AS [Source] ([PropertyID],[StripeProductID],[StripeName],[StripeDescription],[StripeDefaultPriceID],[StripeDefaultUnitPrice],[Quantity])
ON ([Target].[PropertyID] = [Source].[PropertyID] 
AND [Target].[StripeProductID] = [Source].[StripeProductID]
AND [Target].[StripeName] = [Source].[StripeName]
AND [Target].[StripeDescription] = [Source].[StripeDescription]
AND [Target].[StripeDefaultPriceID] = [Source].[StripeDefaultPriceID]
AND [Target].[StripeDefaultUnitPrice] = [Source].[StripeDefaultUnitPrice]
AND [Target].[Quantity] = [Source].[Quantity]
)
WHEN NOT MATCHED BY TARGET THEN
 INSERT([PropertyID],[StripeProductID],[StripeName],[StripeDescription],[StripeDefaultPriceID],[StripeDefaultUnitPrice],[Quantity])
 VALUES([Source].[PropertyID],[Source].[StripeProductID],[Source].[StripeName],[Source].[StripeDescription],[Source].[StripeDefaultPriceID],[Source].[StripeDefaultUnitPrice],[Source].[Quantity])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;

GO

MERGE INTO [dbo].[PropertyNightCoupon] AS [Target]
USING (VALUES
  (1, 7, 10.00)
 ,(2, 7, 10.00)
 ,(3, 7, 10.00)
 ,(1, 28, 38.00)
 ,(2, 28, 38.00)
 ,(3, 28, 38.00)
) AS [Source] ([PropertyID],[NoOfNight],[Percentage])
ON ([Target].[PropertyID] = [Source].[PropertyID] 
AND [Target].[NoOfNight] = [Source].[NoOfNight]
AND [Target].[Percentage] = [Source].[Percentage]
)
WHEN NOT MATCHED BY TARGET THEN
 INSERT([PropertyID],[NoOfNight],[Percentage])
 VALUES([Source].[PropertyID],[Source].[NoOfNight],[Source].[Percentage])
WHEN NOT MATCHED BY SOURCE THEN 
 DELETE;

GO

INSERT INTO [dbo].[Review] ([PropertyID],[Company],[Description],[PrivateNote],[Name],[EmailAddress],[Position],[Rating],[Cleanliness],[Accuracy],[Communication],[Location],[Checkin],[Facilities],[Comfort],[Value],[ReviewTypeId],[LinkToView],[ShowOnHomepage],[DateTimeAdded],[RegistrationCode],[DateTimeApproved],[Approved],[Created])
SELECT 1, N'Booking.com', N'Lovely stay — the property was spotless and in a great location.', N'Internal: verified booking', N'Alice Example', N'alice@example.com', N'Guest', 5, 5, 5, 5, 5, 5, 5, 5, 5, 1, NULL, 1, GETUTCDATE(), '11111111-1111-1111-1111-111111111111', GETUTCDATE(), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Review] WHERE [RegistrationCode] = '11111111-1111-1111-1111-111111111111');
GO

INSERT INTO [dbo].[Review] ([PropertyID],[Company],[Description],[PrivateNote],[Name],[EmailAddress],[Position],[Rating],[Cleanliness],[Accuracy],[Communication],[Location],[Checkin],[Facilities],[Comfort],[Value],[ReviewTypeId],[LinkToView],[ShowOnHomepage],[DateTimeAdded],[RegistrationCode],[DateTimeApproved],[Approved],[Created])
SELECT 2, N'Airbnb', N'Great host and fast communication — would return.', N'Internal: promo applied', N'Bob Example', N'bob@example.com', N'Guest', 5, 5, 5, 5, 5, 5, 5, 5, 5, 2, NULL, 0, GETUTCDATE(), '22222222-2222-2222-2222-222222222222', NULL, 0, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Review] WHERE [RegistrationCode] = '22222222-2222-2222-2222-222222222222');
GO

INSERT INTO [dbo].[Review] ([PropertyID],[Company],[Description],[PrivateNote],[Name],[EmailAddress],[Position],[Rating],[Cleanliness],[Accuracy],[Communication],[Location],[Checkin],[Facilities],[Comfort],[Value],[ReviewTypeId],[LinkToView],[ShowOnHomepage],[DateTimeAdded],[RegistrationCode],[DateTimeApproved],[Approved],[Created])
SELECT 3, N'Google', N'Superb cottage, very cosy and comfortable for a family weekend.', NULL, N'Carol Example', N'carol@example.com', N'Guest', 4, 4, 4, 4, 4, 4, 4, 4, 4, 5, N'https://example.com/review/3', 0, GETUTCDATE(), '33333333-3333-3333-3333-333333333333', GETUTCDATE(), 1, GETUTCDATE()
WHERE NOT EXISTS (SELECT 1 FROM [dbo].[Review] WHERE [RegistrationCode] = '33333333-3333-3333-3333-333333333333');
GO

/* ==========================================================================
   1. Create the WebhookEvent Table
   ========================================================================== */
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[WebhookEvent]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[WebhookEvent] (
                                              [Id]               INT            IDENTITY(1,1) NOT NULL,
                                              [ExternalId]       NVARCHAR(255)  NOT NULL, -- Stripe Event ID (e.g., evt_...)
                                              [Data]             NVARCHAR(MAX)  NOT NULL, -- Full JSON payload
                                              [State]            TINYINT        NOT NULL, -- 0: Pending, 1: Processing, 2: Processed, 3: Failed
                                              [ProcessingErrors] NVARCHAR(MAX)  NULL,
                                              [CreatedAt]        DATETIME       NOT NULL DEFAULT (GETUTCDATE()),
                                              [UpdatedAt]        DATETIME       NOT NULL DEFAULT (GETUTCDATE()),
                                              CONSTRAINT [PK_WebhookEvent] PRIMARY KEY CLUSTERED ([Id] ASC)
        );
        -- Index for fast lookup via Stripe ID
        CREATE UNIQUE NONCLUSTERED INDEX [IX_WebhookEvent_ExternalId]
            ON [dbo].[WebhookEvent] ([ExternalId] ASC);
    END
GO
/* ==========================================================================
   2. Stored Procedure: WebhookEvent_GetByExternalId
   ========================================================================== */
CREATE OR ALTER PROCEDURE [dbo].[WebhookEvent_GetByExternalId]
@externalId NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT [Id], [ExternalId], [Data], [State], [ProcessingErrors], [CreatedAt], [UpdatedAt]
    FROM [dbo].[WebhookEvent]
    WHERE [ExternalId] = @externalId;
END
GO
/* ==========================================================================
   3. Stored Procedure: WebhookEvent_Create
   ========================================================================== */
CREATE OR ALTER PROCEDURE [dbo].[WebhookEvent_Create]
    @ExternalId       NVARCHAR(255),
    @Data             NVARCHAR(MAX),
    @State            TINYINT,
    @CreatedAt        DATETIME,
    @UpdatedAt        DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO [dbo].[WebhookEvent] ([ExternalId], [Data], [State], [CreatedAt], [UpdatedAt])
    VALUES (@ExternalId, @Data, @State, @CreatedAt, @UpdatedAt);
END
GO
/* ==========================================================================
   4. Stored Procedure: WebhookEvent_Update
   ========================================================================== */
CREATE OR ALTER PROCEDURE [dbo].[WebhookEvent_Update]
    @ExternalId       NVARCHAR(255),
    @State            TINYINT,
    @ProcessingErrors NVARCHAR(MAX) = NULL,
    @UpdatedAt        DATETIME
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE [dbo].[WebhookEvent]
    SET [State] = @State,
        [ProcessingErrors] = @ProcessingErrors,
        [UpdatedAt] = @UpdatedAt
    WHERE [ExternalId] = @ExternalId;
END
GO
-- =============================================
-- Seed PropertyImage data
-- =============================================
SET IDENTITY_INSERT [dbo].[PropertyImage] ON;
GO

MERGE INTO [dbo].[PropertyImage] AS [Target]
USING (VALUES
  -- Property 1 images
  (1, 1, N'/uploads/images/slides/lymm-village-apartment.jpg', N'Lymm Village Apartment', 1, 0, 1, GETDATE(), 1),
  (2, 1, N'/images/properties/property-1-living.jpg', N'Spacious living room with modern furniture', 2, 1, 0, GETDATE(), 1),
  (3, 1, N'/images/properties/property-1-kitchen.jpg', N'Fully equipped modern kitchen', 3, 1, 0, GETDATE(), 1),
  (4, 1, N'/images/properties/property-1-bedroom.jpg', N'Comfortable master bedroom', 4, 1, 0, GETDATE(), 1),
  (5, 1, N'/images/properties/property-1-bathroom.jpg', N'Clean modern bathroom', 5, 1, 0, GETDATE(), 1),
  -- Property 2 images
  (6, 2, N'/uploads/images/slides/lymm-house.jpg', N'Lymm House', 1, 1, 1, GETDATE(), 1),
  (7, 2, N'/images/properties/property-2-interior.jpg', N'Property 2 interior', 2, 1, 0, GETDATE(), 1),
  -- Property 3 images
  (8, 3, N'/uploads/images/slides/lymm-cottage.jpg', N'Lymm Cottage', 1, 1, 1, GETDATE(), 1)
) AS [Source] ([ID], [PropertyId], [ImagePath], [AltText], [SequenceOrder], [ShowOnSite], [ForEmail], [Created], [Optimised])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
    NULLIF([Source].[PropertyId], [Target].[PropertyId]) IS NOT NULL OR 
    NULLIF([Target].[PropertyId], [Source].[PropertyId]) IS NOT NULL OR
    NULLIF([Source].[ImagePath], [Target].[ImagePath]) IS NOT NULL OR 
    NULLIF([Target].[ImagePath], [Source].[ImagePath]) IS NOT NULL OR
    NULLIF([Source].[AltText], [Target].[AltText]) IS NOT NULL OR 
    NULLIF([Target].[AltText], [Source].[AltText]) IS NOT NULL OR
    NULLIF([Source].[SequenceOrder], [Target].[SequenceOrder]) IS NOT NULL OR 
    NULLIF([Target].[SequenceOrder], [Source].[SequenceOrder]) IS NOT NULL OR
    NULLIF([Source].[ShowOnSite], [Target].[ShowOnSite]) IS NOT NULL OR 
    NULLIF([Target].[ShowOnSite], [Source].[ShowOnSite]) IS NOT NULL OR
    NULLIF([Source].[ForEmail], [Target].[ForEmail]) IS NOT NULL OR
    NULLIF([Target].[ForEmail], [Source].[ForEmail]) IS NOT NULL OR
    NULLIF([Source].[Optimised], [Target].[Optimised]) IS NOT NULL OR 
    NULLIF([Target].[Optimised], [Source].[Optimised]) IS NOT NULL
) THEN UPDATE SET
  [PropertyId] = [Source].[PropertyId],
  [ImagePath] = [Source].[ImagePath],
  [AltText] = [Source].[AltText],
  [SequenceOrder] = [Source].[SequenceOrder],
  [ShowOnSite] = [Source].[ShowOnSite],
  [ForEmail] = [Source].[ForEmail],
  [Optimised] = [Source].[Optimised]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID], [PropertyId], [ImagePath], [AltText], [SequenceOrder], [ShowOnSite], [ForEmail], [Created], [Optimised])
 VALUES([Source].[ID], [Source].[PropertyId], [Source].[ImagePath], [Source].[AltText], [Source].[SequenceOrder], [Source].[ShowOnSite], [Source].[ForEmail], [Source].[Created], [Source].[Optimised]);

SET IDENTITY_INSERT [dbo].[PropertyImage] OFF;
GO

-- =============================================
-- Seed BedType lookup data
-- =============================================
SET IDENTITY_INSERT [dbo].[BedType] ON;
GO

MERGE INTO [dbo].[BedType] AS [Target]
USING (VALUES
  (1, N'Single Bed', N'/images/bed-icons/single.svg'),
  (2, N'Double Bed', N'/images/bed-icons/double.svg'),
  (3, N'King Bed', N'/images/bed-icons/king.svg'),
  (4, N'Queen Bed', N'/images/bed-icons/queen.svg'),
  (5, N'Sofa Bed', N'/images/bed-icons/sofa.svg'),
  (6, N'Bunk Bed', N'/images/bed-icons/bunk.svg')
) AS [Source] ([ID], [Description], [IconPath])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
    NULLIF([Source].[Description], [Target].[Description]) IS NOT NULL OR 
    NULLIF([Target].[Description], [Source].[Description]) IS NOT NULL OR
    NULLIF([Source].[IconPath], [Target].[IconPath]) IS NOT NULL OR 
    NULLIF([Target].[IconPath], [Source].[IconPath]) IS NOT NULL
) THEN UPDATE SET
  [Description] = [Source].[Description],
  [IconPath] = [Source].[IconPath]
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID], [Description], [IconPath])
 VALUES([Source].[ID], [Source].[Description], [Source].[IconPath])
WHEN NOT MATCHED BY SOURCE THEN DELETE;

SET IDENTITY_INSERT [dbo].[BedType] OFF;
GO

-- =============================================
-- Seed PropertyBedroom data
-- =============================================
SET IDENTITY_INSERT [dbo].[PropertyBedroom] ON;
GO

MERGE INTO [dbo].[PropertyBedroom] AS [Target]
USING (VALUES
  -- Property 1 bedrooms (3 bedrooms)
  (1, 1, 1, N'Master Bedroom', 3, 1, 1, 1, GETDATE(), NULL),  -- King bed
  (2, 1, 2, N'Bedroom 2', 2, 1, 2, 1, GETDATE(), NULL),        -- Double bed
  (3, 1, 3, N'Bedroom 3', 1, 2, 3, 1, GETDATE(), NULL),        -- 2 Single beds
  -- Property 2 bedrooms (2 bedrooms)
  (4, 2, 1, N'Main Bedroom', 2, 1, 1, 1, GETDATE(), NULL),     -- Double bed
  (5, 2, 2, N'Second Bedroom', 6, 1, 2, 1, GETDATE(), NULL)    -- Bunk bed
) AS [Source] ([ID], [PropertyId], [BedroomNumber], [BedroomName], [BedTypeId], [NumberOfBeds], [SequenceOrder], [ShowOnSite], [Created], [Updated])
ON ([Target].[ID] = [Source].[ID])
WHEN MATCHED AND (
    NULLIF([Source].[PropertyId], [Target].[PropertyId]) IS NOT NULL OR 
    NULLIF([Target].[PropertyId], [Source].[PropertyId]) IS NOT NULL OR
    NULLIF([Source].[BedroomNumber], [Target].[BedroomNumber]) IS NOT NULL OR 
    NULLIF([Target].[BedroomNumber], [Source].[BedroomNumber]) IS NOT NULL OR
    NULLIF([Source].[BedroomName], [Target].[BedroomName]) IS NOT NULL OR 
    NULLIF([Target].[BedroomName], [Source].[BedroomName]) IS NOT NULL OR
    NULLIF([Source].[BedTypeId], [Target].[BedTypeId]) IS NOT NULL OR 
    NULLIF([Target].[BedTypeId], [Source].[BedTypeId]) IS NOT NULL OR
    NULLIF([Source].[NumberOfBeds], [Target].[NumberOfBeds]) IS NOT NULL OR 
    NULLIF([Target].[NumberOfBeds], [Source].[NumberOfBeds]) IS NOT NULL OR
    NULLIF([Source].[SequenceOrder], [Target].[SequenceOrder]) IS NOT NULL OR 
    NULLIF([Target].[SequenceOrder], [Source].[SequenceOrder]) IS NOT NULL OR
    NULLIF([Source].[ShowOnSite], [Target].[ShowOnSite]) IS NOT NULL OR 
    NULLIF([Target].[ShowOnSite], [Source].[ShowOnSite]) IS NOT NULL
) THEN UPDATE SET
  [PropertyId] = [Source].[PropertyId],
  [BedroomNumber] = [Source].[BedroomNumber],
  [BedroomName] = [Source].[BedroomName],
  [BedTypeId] = [Source].[BedTypeId],
  [NumberOfBeds] = [Source].[NumberOfBeds],
  [SequenceOrder] = [Source].[SequenceOrder],
  [ShowOnSite] = [Source].[ShowOnSite],
  [Updated] = GETDATE()
WHEN NOT MATCHED BY TARGET THEN
 INSERT([ID], [PropertyId], [BedroomNumber], [BedroomName], [BedTypeId], [NumberOfBeds], [SequenceOrder], [ShowOnSite], [Created], [Updated])
 VALUES([Source].[ID], [Source].[PropertyId], [Source].[BedroomNumber], [Source].[BedroomName], [Source].[BedTypeId], [Source].[NumberOfBeds], [Source].[SequenceOrder], [Source].[ShowOnSite], [Source].[Created], [Source].[Updated]);

SET IDENTITY_INSERT [dbo].[PropertyBedroom] OFF;
GO

-- =============================================
-- Seed CancellationPolicy data
-- =============================================
MERGE INTO [dbo].[CancellationPolicy] AS [Target]
USING (VALUES
  -- Property 1
  (1, 30, N'Cancellations made 30 or more days before check-in receive a full refund.', 1),
  (1, 14, N'Cancellations made 14-29 days before check-in receive a 50% refund.', 2),
  (1,  0, N'Cancellations made within 14 days of check-in are non-refundable.', 3),
  -- Property 2
  (2, 30, N'Cancellations made 30 or more days before check-in receive a full refund.', 1),
  (2, 14, N'Cancellations made 14-29 days before check-in receive a 50% refund.', 2),
  (2,  0, N'Cancellations made within 14 days of check-in are non-refundable.', 3),
  -- Property 3
  (3, 30, N'Cancellations made 30 or more days before check-in receive a full refund.', 1),
  (3, 14, N'Cancellations made 14-29 days before check-in receive a 50% refund.', 2),
  (3,  0, N'Cancellations made within 14 days of check-in are non-refundable.', 3)
) AS [Source] ([PropertyID], [DaysBeforeCheckIn], [PolicyText], [SequenceOrder])
ON ([Target].[PropertyID] = [Source].[PropertyID]
    AND [Target].[DaysBeforeCheckIn] = [Source].[DaysBeforeCheckIn]
    AND [Target].[SequenceOrder] = [Source].[SequenceOrder])
WHEN MATCHED AND (
    NULLIF([Source].[PolicyText], [Target].[PolicyText]) IS NOT NULL OR
    NULLIF([Target].[PolicyText], [Source].[PolicyText]) IS NOT NULL) THEN
  UPDATE SET [PolicyText] = [Source].[PolicyText]
WHEN NOT MATCHED BY TARGET THEN
  INSERT ([PropertyID], [DaysBeforeCheckIn], [PolicyText], [SequenceOrder])
  VALUES ([Source].[PropertyID], [Source].[DaysBeforeCheckIn], [Source].[PolicyText], [Source].[SequenceOrder])
WHEN NOT MATCHED BY SOURCE THEN
  DELETE;

GO

-- =============================================
-- Seed PropertyHouseRule data
-- =============================================
MERGE INTO [dbo].[PropertyHouseRule] AS [Target]
USING (VALUES
  -- Property 1
  (1, N'No smoking anywhere on the premises.', 1),
  (1, N'No pets allowed.', 2),
  (1, N'No parties or events.', 3),
  (1, N'Please respect the neighbours and keep noise to a minimum after 10pm.', 4),
  (1, N'Check-in is from 4:00pm and check-out is by 10:00am.', 5),
  -- Property 2
  (2, N'No smoking anywhere on the premises.', 1),
  (2, N'No pets allowed.', 2),
  (2, N'No parties or events.', 3),
  (2, N'Please respect the neighbours and keep noise to a minimum after 10pm.', 4),
  (2, N'Check-in is from 4:00pm and check-out is by 10:00am.', 5),
  -- Property 3
  (3, N'No smoking anywhere on the premises.', 1),
  (3, N'No pets allowed.', 2),
  (3, N'No parties or events.', 3),
  (3, N'Please respect the neighbours and keep noise to a minimum after 10pm.', 4),
  (3, N'Check-in is from 4:00pm and check-out is by 10:00am.', 5)
) AS [Source] ([PropertyID], [RuleText], [SequenceOrder])
ON ([Target].[PropertyID] = [Source].[PropertyID]
    AND [Target].[SequenceOrder] = [Source].[SequenceOrder])
WHEN MATCHED AND (
    NULLIF([Source].[RuleText], [Target].[RuleText]) IS NOT NULL OR
    NULLIF([Target].[RuleText], [Source].[RuleText]) IS NOT NULL) THEN
  UPDATE SET [RuleText] = [Source].[RuleText]
WHEN NOT MATCHED BY TARGET THEN
  INSERT ([PropertyID], [RuleText], [SequenceOrder])
  VALUES ([Source].[PropertyID], [Source].[RuleText], [Source].[SequenceOrder])
WHEN NOT MATCHED BY SOURCE THEN
  DELETE;

GO

-- =============================================
-- Seed PropertySafetyItem data
-- =============================================
MERGE INTO [dbo].[PropertySafetyItem] AS [Target]
USING (VALUES
  -- Property 1
  (1, N'Smoke detector fitted on each floor.', 1),
  (1, N'Carbon monoxide detector installed.', 2),
  (1, N'Fire extinguisher located in the kitchen.', 3),
  (1, N'First aid kit available.', 4),
  -- Property 2
  (2, N'Smoke detector fitted on each floor.', 1),
  (2, N'Carbon monoxide detector installed.', 2),
  (2, N'Fire extinguisher located in the kitchen.', 3),
  (2, N'First aid kit available.', 4),
  -- Property 3
  (3, N'Smoke detector fitted on each floor.', 1),
  (3, N'Carbon monoxide detector installed.', 2),
  (3, N'Fire extinguisher located in the kitchen.', 3),
  (3, N'First aid kit available.', 4)
) AS [Source] ([PropertyID], [ItemText], [SequenceOrder])
ON ([Target].[PropertyID] = [Source].[PropertyID]
    AND [Target].[SequenceOrder] = [Source].[SequenceOrder])
WHEN MATCHED AND (
    NULLIF([Source].[ItemText], [Target].[ItemText]) IS NOT NULL OR
    NULLIF([Target].[ItemText], [Source].[ItemText]) IS NOT NULL) THEN
  UPDATE SET [ItemText] = [Source].[ItemText]
WHEN NOT MATCHED BY TARGET THEN
  INSERT ([PropertyID], [ItemText], [SequenceOrder])
  VALUES ([Source].[PropertyID], [Source].[ItemText], [Source].[SequenceOrder])
WHEN NOT MATCHED BY SOURCE THEN
  DELETE;

GO

