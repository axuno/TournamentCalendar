﻿//////////////////////////////////////////////////////////////
// <auto-generated>This code was generated by LLBLGen Pro v5.9.</auto-generated>
//////////////////////////////////////////////////////////////
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
//////////////////////////////////////////////////////////////
using System;
using SD.LLBLGen.Pro.ORMSupportClasses;

namespace TournamentCalendarDAL.DatabaseSpecific
{
	/// <summary>Singleton implementation of the PersistenceInfoProvider. This class is the singleton wrapper through which the actual instance is retrieved.</summary>
	internal static class PersistenceInfoProviderSingleton
	{
		private static readonly IPersistenceInfoProvider _providerInstance = new PersistenceInfoProviderCore();

		/// <summary>Dummy static constructor to make sure threadsafe initialization is performed.</summary>
		static PersistenceInfoProviderSingleton() {	}

		/// <summary>Gets the singleton instance of the PersistenceInfoProviderCore</summary>
		/// <returns>Instance of the PersistenceInfoProvider.</returns>
		public static IPersistenceInfoProvider GetInstance() { return _providerInstance; }
	}

	/// <summary>Actual implementation of the PersistenceInfoProvider. Used by singleton wrapper.</summary>
	internal class PersistenceInfoProviderCore : PersistenceInfoProviderBase
	{
		/// <summary>Initializes a new instance of the <see cref="PersistenceInfoProviderCore"/> class.</summary>
		internal PersistenceInfoProviderCore()
		{
			Init();
		}

		/// <summary>Method which initializes the internal datastores with the structure of hierarchical types.</summary>
		private void Init()
		{
			this.InitClass();
			InitCalendarEntityMappings();
			InitCountryEntityMappings();
			InitInfoServiceEntityMappings();
			InitPlayingAbilityEntityMappings();
			InitSentNewsletterEntityMappings();
			InitSurfaceEntityMappings();
		}

		/// <summary>Inits CalendarEntity's mappings</summary>
		private void InitCalendarEntityMappings()
		{
			this.AddElementMapping("CalendarEntity", @"TournamentCalendar", @"dbo", "Calendar", 38, 0);
			this.AddElementFieldMapping("CalendarEntity", "ApprovedOn", "ApprovedOn", true, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 0);
			this.AddElementFieldMapping("CalendarEntity", "AttachmentFile", "AttachmentFile", true, "NVarChar", 512, 0, 0, false, "", null, typeof(System.String), 1);
			this.AddElementFieldMapping("CalendarEntity", "Bond", "Bond", false, "Decimal", 0, 5, 2, false, "", null, typeof(System.Decimal), 2);
			this.AddElementFieldMapping("CalendarEntity", "City", "City", true, "NVarChar", 100, 0, 0, false, "", null, typeof(System.String), 3);
			this.AddElementFieldMapping("CalendarEntity", "ClosingDate", "ClosingDate", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 4);
			this.AddElementFieldMapping("CalendarEntity", "ContactAddress", "ContactAddress", true, "NVarChar", 1024, 0, 0, false, "", null, typeof(System.String), 5);
			this.AddElementFieldMapping("CalendarEntity", "CountryId", "CountryId", false, "NVarChar", 2, 0, 0, false, "", null, typeof(System.String), 6);
			this.AddElementFieldMapping("CalendarEntity", "CreatedByUser", "CreatedByUser", true, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 7);
			this.AddElementFieldMapping("CalendarEntity", "CreatedOn", "CreatedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 8);
			this.AddElementFieldMapping("CalendarEntity", "DateFrom", "DateFrom", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 9);
			this.AddElementFieldMapping("CalendarEntity", "DateTo", "DateTo", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 10);
			this.AddElementFieldMapping("CalendarEntity", "DeletedOn", "DeletedOn", true, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 11);
			this.AddElementFieldMapping("CalendarEntity", "Email", "Email", true, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 12);
			this.AddElementFieldMapping("CalendarEntity", "EntryFee", "EntryFee", false, "Decimal", 0, 5, 2, false, "", null, typeof(System.Decimal), 13);
			this.AddElementFieldMapping("CalendarEntity", "Guid", "Guid", true, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 14);
			this.AddElementFieldMapping("CalendarEntity", "Id", "Id", false, "BigInt", 0, 19, 0, true, "SCOPE_IDENTITY()", null, typeof(System.Int64), 15);
			this.AddElementFieldMapping("CalendarEntity", "Info", "Info", true, "NVarChar", 1024, 0, 0, false, "", null, typeof(System.String), 16);
			this.AddElementFieldMapping("CalendarEntity", "IsMinPlayersFemale", "IsMinPlayersFemale", false, "Bit", 0, 0, 0, false, "", null, typeof(System.Boolean), 17);
			this.AddElementFieldMapping("CalendarEntity", "IsMinPlayersMale", "IsMinPlayersMale", false, "Bit", 0, 0, 0, false, "", null, typeof(System.Boolean), 18);
			this.AddElementFieldMapping("CalendarEntity", "Latitude", "Latitude", true, "Float", 0, 38, 0, false, "", null, typeof(System.Double), 19);
			this.AddElementFieldMapping("CalendarEntity", "Longitude", "Longitude", true, "Float", 0, 38, 0, false, "", null, typeof(System.Double), 20);
			this.AddElementFieldMapping("CalendarEntity", "ModifiedOn", "ModifiedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 21);
			this.AddElementFieldMapping("CalendarEntity", "NumOfTeams", "NumOfTeams", false, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 22);
			this.AddElementFieldMapping("CalendarEntity", "NumPlayersFemale", "NumPlayersFemale", false, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 23);
			this.AddElementFieldMapping("CalendarEntity", "NumPlayersMale", "NumPlayersMale", false, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 24);
			this.AddElementFieldMapping("CalendarEntity", "Organizer", "Organizer", true, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 25);
			this.AddElementFieldMapping("CalendarEntity", "PlayingAbilityFrom", "PlayingAbilityFrom", false, "BigInt", 0, 19, 0, false, "", null, typeof(System.Int64), 26);
			this.AddElementFieldMapping("CalendarEntity", "PlayingAbilityTo", "PlayingAbilityTo", false, "BigInt", 0, 19, 0, false, "", null, typeof(System.Int64), 27);
			this.AddElementFieldMapping("CalendarEntity", "PostalCode", "PostalCode", true, "NVarChar", 10, 0, 0, false, "", null, typeof(System.String), 28);
			this.AddElementFieldMapping("CalendarEntity", "PostedByEmail", "PostedByEmail", true, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 29);
			this.AddElementFieldMapping("CalendarEntity", "PostedByName", "PostedByName", true, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 30);
			this.AddElementFieldMapping("CalendarEntity", "PostedByPassword", "PostedByPassword", true, "NVarChar", 64, 0, 0, false, "", null, typeof(System.String), 31);
			this.AddElementFieldMapping("CalendarEntity", "Special", "Special", true, "NVarChar", 512, 0, 0, false, "", null, typeof(System.String), 32);
			this.AddElementFieldMapping("CalendarEntity", "Street", "Street", true, "NVarChar", 100, 0, 0, false, "", null, typeof(System.String), 33);
			this.AddElementFieldMapping("CalendarEntity", "Surface", "Surface", false, "BigInt", 0, 19, 0, false, "", null, typeof(System.Int64), 34);
			this.AddElementFieldMapping("CalendarEntity", "TournamentName", "TournamentName", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 35);
			this.AddElementFieldMapping("CalendarEntity", "Venue", "Venue", true, "NVarChar", 100, 0, 0, false, "", null, typeof(System.String), 36);
			this.AddElementFieldMapping("CalendarEntity", "Website", "Website", true, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 37);
		}

		/// <summary>Inits CountryEntity's mappings</summary>
		private void InitCountryEntityMappings()
		{
			this.AddElementMapping("CountryEntity", @"TournamentCalendar", @"dbo", "Country", 6, 0);
			this.AddElementFieldMapping("CountryEntity", "CreatedOn", "CreatedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 0);
			this.AddElementFieldMapping("CountryEntity", "Id", "Id", false, "NVarChar", 2, 0, 0, false, "", null, typeof(System.String), 1);
			this.AddElementFieldMapping("CountryEntity", "Iso3", "Iso3", false, "NVarChar", 3, 0, 0, false, "", null, typeof(System.String), 2);
			this.AddElementFieldMapping("CountryEntity", "ModifiedOn", "ModifiedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 3);
			this.AddElementFieldMapping("CountryEntity", "Name", "Name", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 4);
			this.AddElementFieldMapping("CountryEntity", "NameEn", "NameEN", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 5);
		}

		/// <summary>Inits InfoServiceEntity's mappings</summary>
		private void InitInfoServiceEntityMappings()
		{
			this.AddElementMapping("InfoServiceEntity", @"TournamentCalendar", @"dbo", "InfoService", 24, 0);
			this.AddElementFieldMapping("InfoServiceEntity", "City", "City", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 0);
			this.AddElementFieldMapping("InfoServiceEntity", "ClubName", "ClubName", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 1);
			this.AddElementFieldMapping("InfoServiceEntity", "Comments", "Comments", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 2);
			this.AddElementFieldMapping("InfoServiceEntity", "ConfirmedOn", "ConfirmedOn", true, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 3);
			this.AddElementFieldMapping("InfoServiceEntity", "CountryId", "CountryId", true, "NVarChar", 2, 0, 0, false, "", null, typeof(System.String), 4);
			this.AddElementFieldMapping("InfoServiceEntity", "Email", "Email", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 5);
			this.AddElementFieldMapping("InfoServiceEntity", "FirstName", "FirstName", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 6);
			this.AddElementFieldMapping("InfoServiceEntity", "Gender", "Gender", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 7);
			this.AddElementFieldMapping("InfoServiceEntity", "Guid", "Guid", true, "NVarChar", 64, 0, 0, false, "", null, typeof(System.String), 8);
			this.AddElementFieldMapping("InfoServiceEntity", "Id", "Id", false, "Int", 0, 10, 0, true, "SCOPE_IDENTITY()", null, typeof(System.Int32), 9);
			this.AddElementFieldMapping("InfoServiceEntity", "LastName", "LastName", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 10);
			this.AddElementFieldMapping("InfoServiceEntity", "Latitude", "Latitude", true, "Float", 0, 38, 0, false, "", null, typeof(System.Double), 11);
			this.AddElementFieldMapping("InfoServiceEntity", "Longitude", "Longitude", true, "Float", 0, 38, 0, false, "", null, typeof(System.Double), 12);
			this.AddElementFieldMapping("InfoServiceEntity", "MaxDistance", "MaxDistance", true, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 13);
			this.AddElementFieldMapping("InfoServiceEntity", "ModifiedOn", "ModifiedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 14);
			this.AddElementFieldMapping("InfoServiceEntity", "Nickname", "Nickname", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 15);
			this.AddElementFieldMapping("InfoServiceEntity", "Street", "Street", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 16);
			this.AddElementFieldMapping("InfoServiceEntity", "SubscribedOn", "SubscribedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 17);
			this.AddElementFieldMapping("InfoServiceEntity", "TeamName", "TeamName", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 18);
			this.AddElementFieldMapping("InfoServiceEntity", "Title", "Title", false, "NVarChar", 30, 0, 0, false, "", null, typeof(System.String), 19);
			this.AddElementFieldMapping("InfoServiceEntity", "UnSubscribedOn", "UnSubscribedOn", true, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 20);
			this.AddElementFieldMapping("InfoServiceEntity", "UserName", "UserName", false, "NVarChar", 50, 0, 0, false, "", null, typeof(System.String), 21);
			this.AddElementFieldMapping("InfoServiceEntity", "UserPassword", "UserPassword", false, "NVarChar", 10, 0, 0, false, "", null, typeof(System.String), 22);
			this.AddElementFieldMapping("InfoServiceEntity", "ZipCode", "ZipCode", false, "NVarChar", 6, 0, 0, false, "", null, typeof(System.String), 23);
		}

		/// <summary>Inits PlayingAbilityEntity's mappings</summary>
		private void InitPlayingAbilityEntityMappings()
		{
			this.AddElementMapping("PlayingAbilityEntity", @"TournamentCalendar", @"dbo", "PlayingAbility", 5, 0);
			this.AddElementFieldMapping("PlayingAbilityEntity", "CreatedOn", "CreatedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 0);
			this.AddElementFieldMapping("PlayingAbilityEntity", "Description", "Description", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 1);
			this.AddElementFieldMapping("PlayingAbilityEntity", "Id", "Id", false, "BigInt", 0, 19, 0, true, "SCOPE_IDENTITY()", null, typeof(System.Int64), 2);
			this.AddElementFieldMapping("PlayingAbilityEntity", "ModifiedOn", "ModifiedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 3);
			this.AddElementFieldMapping("PlayingAbilityEntity", "Strength", "Strength", false, "BigInt", 0, 19, 0, false, "", null, typeof(System.Int64), 4);
		}

		/// <summary>Inits SentNewsletterEntity's mappings</summary>
		private void InitSentNewsletterEntityMappings()
		{
			this.AddElementMapping("SentNewsletterEntity", @"TournamentCalendar", @"dbo", "SentNewsletters", 5, 0);
			this.AddElementFieldMapping("SentNewsletterEntity", "CompletedOn", "CompletedOn", true, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 0);
			this.AddElementFieldMapping("SentNewsletterEntity", "Id", "Id", false, "BigInt", 0, 19, 0, true, "SCOPE_IDENTITY()", null, typeof(System.Int64), 1);
			this.AddElementFieldMapping("SentNewsletterEntity", "NumOfRecipients", "NumOfRecipients", false, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 2);
			this.AddElementFieldMapping("SentNewsletterEntity", "NumOfTournaments", "NumOfTournaments", false, "Int", 0, 10, 0, false, "", null, typeof(System.Int32), 3);
			this.AddElementFieldMapping("SentNewsletterEntity", "StartedOn", "StartedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 4);
		}

		/// <summary>Inits SurfaceEntity's mappings</summary>
		private void InitSurfaceEntityMappings()
		{
			this.AddElementMapping("SurfaceEntity", @"TournamentCalendar", @"dbo", "Surface", 4, 0);
			this.AddElementFieldMapping("SurfaceEntity", "Id", "Id", false, "BigInt", 0, 19, 0, true, "SCOPE_IDENTITY()", null, typeof(System.Int64), 0);
			this.AddElementFieldMapping("SurfaceEntity", "Description", "Description", false, "NVarChar", 255, 0, 0, false, "", null, typeof(System.String), 1);
			this.AddElementFieldMapping("SurfaceEntity", "CreatedOn", "CreatedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 2);
			this.AddElementFieldMapping("SurfaceEntity", "ModifiedOn", "ModifiedOn", false, "DateTime", 0, 0, 0, false, "", null, typeof(System.DateTime), 3);
		}

	}
}
