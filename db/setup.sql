IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'LymmHolidayLets')
BEGIN
    CREATE DATABASE LymmHolidayLets;
END
GO

USE LymmHolidayLets;
GO

CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY(1,1),
    Username NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) UNIQUE
);
GO

INSERT INTO Users (Username, Email) VALUES 
('MatthewC', 'matthew@example.com'), 
('TestUser', 'test@example.com');
GO