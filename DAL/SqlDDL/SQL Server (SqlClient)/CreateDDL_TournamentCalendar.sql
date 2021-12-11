-- SQL Manager for SQL Server 4.4.1.49818
-- ---------------------------------------
-- Host      : (local)
-- Database  : TournamentCalendar
-- Version   : Microsoft SQL Server 2016 (SP1) (KB3182545) 13.0.4001.0


--
-- Dropping table Calendar :
--

DROP TABLE IF EXISTS dbo.Calendar
GO

--
-- Dropping table InfoService :
--

DROP TABLE IF EXISTS dbo.InfoService
GO

--
-- Dropping table Surface :
--

DROP TABLE IF EXISTS dbo.Surface
GO

--
-- Dropping table PlayingAbility :
--

DROP TABLE IF EXISTS dbo.PlayingAbility
GO

--
-- Dropping table Country :
--

DROP TABLE IF EXISTS dbo.Country
GO


--
-- Definition for table Surface :
--

CREATE TABLE dbo.Surface (
  Id bigint IDENTITY(1, 1) NOT NULL,
  Description nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  CreatedOn datetime NOT NULL,
  ModifiedOn datetime NOT NULL,
  CONSTRAINT PK_b779ee9469da9b52e7282721329 PRIMARY KEY CLUSTERED (Id)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO

--
-- Definition for table PlayingAbility :
--

CREATE TABLE dbo.PlayingAbility (
  Id bigint IDENTITY(1, 1) NOT NULL,
  Description nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  Strength bigint NOT NULL,
  CreatedOn datetime NOT NULL,
  ModifiedOn datetime NOT NULL,
  CONSTRAINT PK_ef96466400f929b0b99f896682b PRIMARY KEY CLUSTERED (Strength)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO

--
-- Definition for table Country :
--

CREATE TABLE dbo.Country (
  Id nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  Name nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  Iso3 nvarchar(3) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  CreatedOn datetime NOT NULL,
  ModifiedOn datetime NOT NULL,
  CONSTRAINT PK_99262ae40258db0d9b2a36f29ad PRIMARY KEY CLUSTERED (Id)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO

--
-- Definition for table Calendar :
--

CREATE TABLE dbo.Calendar (
  Id bigint IDENTITY(1, 1) NOT NULL,
  Guid nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  TournamentName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_TournamentName DEFAULT '' NOT NULL,
  DateFrom datetime NOT NULL,
  DateTo datetime NOT NULL,
  ClosingDate datetime NOT NULL,
  Venue nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Venue DEFAULT '' NULL,
  CountryId nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
  PostalCode nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_PostalCode DEFAULT '' NULL,
  City nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_City DEFAULT '' NULL,
  Street nvarchar(100) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Street DEFAULT '' NULL,
  Longitude float NULL,
  Latitude float NULL,
  NumOfTeams int NOT NULL,
  NumPlayersMale int NOT NULL,
  IsMinPlayersMale bit NOT NULL,
  NumPlayersFemale int NOT NULL,
  IsMinPlayersFemale bit NOT NULL,
  Surface bigint NOT NULL,
  PlayingAbilityFrom bigint NOT NULL,
  PlayingAbilityTo bigint NOT NULL,
  EntryFee decimal(5, 2) NOT NULL,
  Bond decimal(5, 2) NOT NULL,
  Info nvarchar(1024) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Info DEFAULT '' NULL,
  Organizer nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Organizer DEFAULT '' NULL,
  ContactAddress nvarchar(1024) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_ContactAddress DEFAULT '' NULL,
  Email nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Email DEFAULT '' NULL,
  Website nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Website DEFAULT '' NULL,
  PostedByName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_PostedByName DEFAULT '' NULL,
  PostedByEmail nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_PostedByEmail DEFAULT '' NULL,
  PostedByPassword nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_PostedByPassword DEFAULT '' NULL,
  AttachmentFile nvarchar(512) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_AttachmentFile DEFAULT '' NULL,
  Special nvarchar(512) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_Calendar_Special DEFAULT '' NULL,
  CreatedOn datetime NOT NULL,
  ModifiedOn datetime NOT NULL,
  DeletedOn datetime NULL,
  ApprovedOn datetime NULL,
  CONSTRAINT PK_1fc3031419c9a8125ae1a8b4f02 PRIMARY KEY CLUSTERED (Id)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO

--
-- Definition for table InfoService :
--

CREATE TABLE dbo.InfoService (
  Id int IDENTITY(1, 1) NOT NULL,
  TeamName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_TeamName DEFAULT '' NOT NULL,
  ClubName nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_ClubName DEFAULT '' NOT NULL,
  Gender nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Gender DEFAULT '' NOT NULL,
  Title nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Title DEFAULT '' NOT NULL,
  FirstName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_FirstName DEFAULT '' NOT NULL,
  LastName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_LastName DEFAULT '' NOT NULL,
  Nickname nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Nickname DEFAULT '' NOT NULL,
  CountryId nvarchar(2) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
  ZipCode nvarchar(6) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_ZipCode DEFAULT '' NOT NULL,
  City nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_City DEFAULT '' NOT NULL,
  Street nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Street DEFAULT '' NOT NULL,
  Longitude float NULL,
  Latitude float NULL,
  MaxDistance int NULL,
  Email nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Email DEFAULT '' NOT NULL,
  UserName nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_UserName DEFAULT '' NOT NULL,
  UserPassword nvarchar(10) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_UserPassword DEFAULT '' NOT NULL,
  Guid nvarchar(64) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Guid DEFAULT '' NULL,
  Comments nvarchar(255) COLLATE SQL_Latin1_General_CP1_CI_AS CONSTRAINT DF_InfoService_Comments DEFAULT '' NOT NULL,
  SubscribedOn datetime NOT NULL,
  ModifiedOn datetime NOT NULL,
  ConfirmedOn datetime NULL,
  UnSubscribedOn datetime NULL,
  CONSTRAINT PK_c4d440042eb8dfdd13c5e7200d0 PRIMARY KEY CLUSTERED (Id)
    WITH (
      PAD_INDEX = OFF, IGNORE_DUP_KEY = OFF, STATISTICS_NORECOMPUTE = OFF,
      ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
ON [PRIMARY]
GO

--
-- Definition for indices :
--

ALTER TABLE dbo.PlayingAbility
ADD CONSTRAINT UC_cf2387e466aa1259db699dd071d
UNIQUE NONCLUSTERED (Id)
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO

ALTER TABLE dbo.Country
ADD CONSTRAINT UC_6a283784ae9a06be73f61031c43
UNIQUE NONCLUSTERED (Iso3)
WITH (
  PAD_INDEX = OFF,
  IGNORE_DUP_KEY = OFF,
  STATISTICS_NORECOMPUTE = OFF,
  ALLOW_ROW_LOCKS = ON,
  ALLOW_PAGE_LOCKS = ON)
ON [PRIMARY]
GO

--
-- Definition for foreign keys :
--

ALTER TABLE dbo.Calendar
ADD CONSTRAINT FK_21b4638438eb5ed29dc461cb581 FOREIGN KEY (Surface)
  REFERENCES dbo.Surface (Id)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
GO

ALTER TABLE dbo.Calendar
ADD CONSTRAINT FK_c472c3f45069be32b1b75ef81e6 FOREIGN KEY (CountryId)
  REFERENCES dbo.Country (Id)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
GO

ALTER TABLE dbo.Calendar
ADD CONSTRAINT FK_d0f94394de0b6ef2a0b5646b689 FOREIGN KEY (PlayingAbilityFrom)
  REFERENCES dbo.PlayingAbility (Strength)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
GO

ALTER TABLE dbo.Calendar
ADD CONSTRAINT FK_e43c35b45018bb920ab19e67bf8 FOREIGN KEY (PlayingAbilityTo)
  REFERENCES dbo.PlayingAbility (Strength)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
GO

ALTER TABLE dbo.InfoService
ADD CONSTRAINT FK_f41ddf54294b7d713f434223d67 FOREIGN KEY (CountryId)
  REFERENCES dbo.Country (Id)
  ON UPDATE NO ACTION
  ON DELETE NO ACTION
GO

