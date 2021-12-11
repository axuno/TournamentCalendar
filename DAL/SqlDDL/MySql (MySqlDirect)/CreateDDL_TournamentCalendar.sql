/* SQL Manager for MySQL                              5.6.4.50082 */
/* -------------------------------------------------------------- */
/* Host     : localhost                                           */
/* Port     : 3306                                                */
/* Database : dbvbc01                                             */


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES 'utf8' */;

SET FOREIGN_KEY_CHECKS=0;

/* Dropping database objects */

DROP TABLE IF EXISTS `vbt_calendar`;
DROP TABLE IF EXISTS `vbt_info_service`;
DROP TABLE IF EXISTS `vbt_country`;
DROP TABLE IF EXISTS `vbt_playing_ability`;
DROP TABLE IF EXISTS `vbt_surface`;

/* Structure for the `vbt_surface` table : */

CREATE TABLE `vbt_surface` (
  `Id` INTEGER(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Description` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CreatedOn` DATETIME NOT NULL,
  `ModifiedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY USING BTREE (`Id`),
  UNIQUE KEY `Id` USING BTREE (`Id`)
) ENGINE=InnoDB
AUTO_INCREMENT=6 CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
COMMENT='ISO 3166-1 Country Codes with German names'
;

/* Structure for the `vbt_playing_ability` table : */

CREATE TABLE `vbt_playing_ability` (
  `Id` INTEGER(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Strength` INTEGER(2) UNSIGNED NOT NULL DEFAULT 0,
  `Description` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CreatedOn` DATETIME NOT NULL,
  `ModifiedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY USING BTREE (`Strength`),
  UNIQUE KEY `Id` USING BTREE (`Id`)
) ENGINE=InnoDB
AUTO_INCREMENT=11 CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
COMMENT='ISO 3166-1 Country Codes with German names'
;

/* Structure for the `vbt_country` table : */

CREATE TABLE `vbt_country` (
  `Id` CHAR(2) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Iso3` CHAR(3) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Name` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CreatedOn` DATETIME NOT NULL,
  `ModifiedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY USING BTREE (`Id`),
  UNIQUE KEY `Id` USING BTREE (`Id`),
  UNIQUE KEY `Iso3` USING BTREE (`Iso3`)
) ENGINE=InnoDB
CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
COMMENT='ISO 3166-1 Country Codes with German names'
;

/* Structure for the `vbt_info_service` table : */

CREATE TABLE `vbt_info_service` (
  `Id` INTEGER(11) NOT NULL AUTO_INCREMENT,
  `TeamName` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `ClubName` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Gender` ENUM('m','f') COLLATE utf8_general_ci NOT NULL DEFAULT 'm',
  `Title` VARCHAR(30) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `FirstName` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `LastName` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Nickname` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `CountryId` CHAR(2) COLLATE utf8_general_ci DEFAULT '',
  `ZipCode` VARCHAR(6) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `City` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Street` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Longitude` DOUBLE(15,12) DEFAULT NULL,
  `Latitude` DOUBLE(15,12) DEFAULT NULL,
  `MaxDistance` INTEGER(4) DEFAULT NULL,
  `Email` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `UserName` VARCHAR(50) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `UserPassword` VARCHAR(10) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `Guid` VARCHAR(64) COLLATE utf8_general_ci DEFAULT NULL,
  `Comments` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '',
  `SubscribedOn` DATETIME NOT NULL,
  `ModifiedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ConfirmedOn` DATETIME DEFAULT NULL,
  `UnSubscribedOn` DATETIME DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`),
  UNIQUE KEY `Email` USING BTREE (`Email`),
  UNIQUE KEY `Email_2` USING BTREE (`Email`),
  UNIQUE KEY `Guid` USING BTREE (`Guid`),
  UNIQUE KEY `Guid_2` USING BTREE (`Guid`),
  UNIQUE KEY `Guid_3` USING BTREE (`Guid`),
  KEY `CountryId` USING BTREE (`CountryId`),
  CONSTRAINT `Fk_Vbc_InfoService_Country` FOREIGN KEY (`CountryId`) REFERENCES `vbt_country` (`Id`) ON DELETE SET NULL ON UPDATE CASCADE
) ENGINE=InnoDB
AUTO_INCREMENT=527 CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
COMMENT='Info Service Addresses'
;

/* Structure for the `vbt_calendar` table : */

CREATE TABLE `vbt_calendar` (
  `Id` INTEGER(11) UNSIGNED NOT NULL AUTO_INCREMENT COMMENT 'Eindeutiger interner Bezeichner für den Eintrag',
  `Guid` VARCHAR(50) COLLATE utf8_general_ci DEFAULT NULL COMMENT 'Eindeutiger Bezeichner, mit dem der Eintrag geändert/bestätigt/gelöscht werden kann',
  `TournamentName` VARCHAR(255) COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT 'Name des Turniers\r\nBsp: \"Beach for Life\"',
  `DateFrom` DATETIME NOT NULL COMMENT 'Datum und Uhrzeit des Turnierbeginns',
  `DateTo` DATETIME NOT NULL COMMENT 'Datum und Uhrzeit des Turnierendes',
  `ClosingDate` DATE NOT NULL COMMENT 'Datum für den Meldeschluss (Zeit 00:00:00)',
  `Venue` VARCHAR(100) COLLATE utf8_general_ci DEFAULT NULL COMMENT 'Name der Sportanlage\r\nBsp.: Beachanlage Freibad',
  `CountryId` CHAR(2) COLLATE utf8_general_ci NOT NULL DEFAULT '' COMMENT 'ISO-Länderkürzel\r\nBsp: \"DE\" für Deutschland',
  `PostalCode` VARCHAR(10) COLLATE utf8_general_ci DEFAULT NULL COMMENT 'PostalCode',
  `City` VARCHAR(100) COLLATE utf8_general_ci DEFAULT NULL COMMENT 'Ort, in dem das Turnier stattfindet',
  `Street` VARCHAR(100) COLLATE utf8_general_ci DEFAULT NULL COMMENT 'Straße, in der das Turnier stattfindet',
  `Longitude` DOUBLE(15,12) DEFAULT NULL COMMENT 'Längengrad des Turnierstandorts – unbekannt: 0.00',
  `Latitude` DOUBLE(15,12) DEFAULT NULL COMMENT 'Breitengrad des Turnierstandorts – unbekannt: 0.00',
  `NumOfTeams` INTEGER(3) NOT NULL DEFAULT 0 COMMENT 'Maximale Teilnehmeranzahl – unbekannt: 0',
  `NumPlayersMale` INTEGER(2) NOT NULL COMMENT 'Anzahl männliche Spieler',
  `IsMinPlayersMale` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '1 = \"true\", wenn die Anzahl der männlichen Spieler eine Mindestanzahl ist, sonst \"false\"',
  `NumPlayersFemale` INTEGER(2) NOT NULL COMMENT 'Anzahl weiblilche Spieler',
  `IsMinPlayersFemale` TINYINT(1) NOT NULL DEFAULT 0 COMMENT '1 = \"true\", wenn die Anzahl der weibllichen Spieler eine Mindestanzahl ist, sonst \"false\"\r\nBsp. \"true\": 6 Spieler, aber mindestens 2 Damen\r\nBsp. \"false\": genau 3 Herren und 3 Damen',
  `Surface` INTEGER(2) UNSIGNED NOT NULL DEFAULT 1 COMMENT '1 = Halle, 2 = Sand indoor, 3= Sand outdoor 4 = Rasen, 5 = Hartplatz',
  `PlayingAbilityFrom` INTEGER(2) UNSIGNED NOT NULL DEFAULT 0 COMMENT '0 = ohne Beschränkung, 1-9 = Abstufung der Spielstärke von Freizeit bis Bundesliga.',
  `PlayingAbilityTo` INTEGER(2) UNSIGNED NOT NULL DEFAULT 0 COMMENT '0 = ohne Beschränkung, 1-9 = Abstufung der Spielstärke von Freizeit bis Bundesliga.',
  `EntryFee` DECIMAL(5,2) NOT NULL COMMENT 'Startgebühr in Euro, 2 Nachkommastellen',
  `Bond` DECIMAL(5,2) NOT NULL COMMENT 'Kaution in Euro, 2 Nachkommastellen',
  `Info` VARCHAR(1024) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Freier Text für Zusatzinformationen zum Turnier',
  `Organizer` VARCHAR(255) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Bezeichnung des Turnierveranstalters\r\nBsp.: \"MTV Ludwigsburg\"',
  `ContactAddress` VARCHAR(1024) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Kontaktdaten des Veranstalters (kein festes Format; die Wahl der Kontaktdaten ist dem Eintragenden überlassen)',
  `Email` VARCHAR(255) COLLATE utf8_general_ci DEFAULT '' COMMENT 'E-Mail-Adresse für Anmeldungen/Rückfragen (wird publiziert)',
  `Website` VARCHAR(255) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Internetadresse für das Turnier. Zulässige Protokolle: http und https (wird publiziert)',
  `PostedByName` VARCHAR(50) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Name der Person, die den Kalendereintrag angelegt hat',
  `PostedByEmail` VARCHAR(255) COLLATE utf8_general_ci DEFAULT '' COMMENT 'E-Mail der Person, die den Kalendereintrag angelegt hat',
  `PostedByPassword` VARCHAR(64) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Passwort der Person, die den Kalendereintrag angelegt hat',
  `AttachmentFile` VARCHAR(512) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Url zu Turnierdetails, die heruntergeladen werden können (z.B. zu Dropbox)',
  `Special` VARCHAR(512) COLLATE utf8_general_ci DEFAULT '' COMMENT 'Spezialfeld zum Hervorheben bestimmter Turniereinträge',
  `CreatedOn` DATETIME NOT NULL COMMENT 'Datum/Uhr, wann der Eintrag angelegt wurde',
  `ModifiedOn` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT 'Datum/Uhr, wann der Eintrag zuletzt geändert wurde',
  `DeletedOn` DATETIME DEFAULT NULL COMMENT 'Datum/Uhr, wann der Eintrag als gelöscht markiert wurde',
  `ApprovedOn` DATETIME DEFAULT NULL COMMENT 'Datum/Uhr, wann der Eintrag von der anlegenden Person bestätigt wurde',
  PRIMARY KEY USING BTREE (`Id`),
  KEY `CountryId` USING BTREE (`CountryId`),
  KEY `Fk_Surface` USING BTREE (`Surface`),
  KEY `Fk_PlayingAbilityFrom` USING BTREE (`PlayingAbilityFrom`),
  KEY `Fk_PlayingAbilityTo` USING BTREE (`PlayingAbilityTo`),
  CONSTRAINT `Fk_PlayingAbilityFrom` FOREIGN KEY (`PlayingAbilityFrom`) REFERENCES `vbt_playing_ability` (`Strength`) ON UPDATE CASCADE,
  CONSTRAINT `Fk_PlayingAbilityTo` FOREIGN KEY (`PlayingAbilityTo`) REFERENCES `vbt_playing_ability` (`Strength`) ON UPDATE CASCADE,
  CONSTRAINT `Fk_Surface` FOREIGN KEY (`Surface`) REFERENCES `vbt_surface` (`Id`),
  CONSTRAINT `Fk_Tournament_Country` FOREIGN KEY (`CountryId`) REFERENCES `vbt_country` (`Id`) ON UPDATE CASCADE
) ENGINE=InnoDB
PACK_KEYS=1 AUTO_INCREMENT=510 AVG_ROW_LENGTH=949 CHARACTER SET 'utf8' COLLATE 'utf8_general_ci'
COMMENT='Tournament Calendar'
;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;