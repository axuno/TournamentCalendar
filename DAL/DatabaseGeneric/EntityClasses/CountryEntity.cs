﻿//////////////////////////////////////////////////////////////
// <auto-generated>This code was generated by LLBLGen Pro 5.9.</auto-generated>
//////////////////////////////////////////////////////////////
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
//////////////////////////////////////////////////////////////
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using TournamentCalendarDAL.HelperClasses;
using TournamentCalendarDAL.FactoryClasses;
using TournamentCalendarDAL.RelationClasses;

using SD.LLBLGen.Pro.ORMSupportClasses;

namespace TournamentCalendarDAL.EntityClasses
{
	// __LLBLGENPRO_USER_CODE_REGION_START AdditionalNamespaces
	// __LLBLGENPRO_USER_CODE_REGION_END
	/// <summary>Entity class which represents the entity 'Country'.<br/><br/></summary>
	[Serializable]
	public partial class CountryEntity : CommonEntityBase
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfaces
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		private EntityCollection<CalendarEntity> _tournamentCalendars;
		private EntityCollection<InfoServiceEntity> _infoServices;

		// __LLBLGENPRO_USER_CODE_REGION_START PrivateMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static CountryEntityStaticMetaData _staticMetaData = new CountryEntityStaticMetaData();
		private static CountryRelations _relationsFactory = new CountryRelations();

		/// <summary>All names of fields mapped onto a relation. Usable for in-memory filtering</summary>
		public static partial class MemberNames
		{
			/// <summary>Member name TournamentCalendars</summary>
			public static readonly string TournamentCalendars = "TournamentCalendars";
			/// <summary>Member name InfoServices</summary>
			public static readonly string InfoServices = "InfoServices";
		}

		/// <summary>Static meta-data storage for navigator related information</summary>
		protected class CountryEntityStaticMetaData : EntityStaticMetaDataBase
		{
			public CountryEntityStaticMetaData()
			{
				SetEntityCoreInfo("CountryEntity", InheritanceHierarchyType.None, false, (int)TournamentCalendarDAL.EntityType.CountryEntity, typeof(CountryEntity), typeof(CountryEntityFactory), false);
				AddNavigatorMetaData<CountryEntity, EntityCollection<CalendarEntity>>("TournamentCalendars", a => a._tournamentCalendars, (a, b) => a._tournamentCalendars = b, a => a.TournamentCalendars, () => new CountryRelations().CalendarEntityUsingCountryId, typeof(CalendarEntity), (int)TournamentCalendarDAL.EntityType.CalendarEntity);
				AddNavigatorMetaData<CountryEntity, EntityCollection<InfoServiceEntity>>("InfoServices", a => a._infoServices, (a, b) => a._infoServices = b, a => a.InfoServices, () => new CountryRelations().InfoServiceEntityUsingCountryId, typeof(InfoServiceEntity), (int)TournamentCalendarDAL.EntityType.InfoServiceEntity);
			}
		}

		/// <summary>Static ctor</summary>
		static CountryEntity()
		{
		}

		/// <summary> CTor</summary>
		public CountryEntity()
		{
			InitClassEmpty(null, null);
		}

		/// <summary> CTor</summary>
		/// <param name="fields">Fields object to set as the fields for this entity.</param>
		public CountryEntity(IEntityFields2 fields)
		{
			InitClassEmpty(null, fields);
		}

		/// <summary> CTor</summary>
		/// <param name="validator">The custom validator object for this CountryEntity</param>
		public CountryEntity(IValidator validator)
		{
			InitClassEmpty(validator, null);
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for Country which data should be fetched into this Country object</param>
		public CountryEntity(System.String id) : this(id, null)
		{
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for Country which data should be fetched into this Country object</param>
		/// <param name="validator">The custom validator object for this CountryEntity</param>
		public CountryEntity(System.String id, IValidator validator)
		{
			InitClassEmpty(validator, null);
			this.Id = id;
		}

		/// <summary>Private CTor for deserialization</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected CountryEntity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// __LLBLGENPRO_USER_CODE_REGION_START DeserializationConstructor
			// __LLBLGENPRO_USER_CODE_REGION_END
		}

		/// <summary>Method which will construct a filter (predicate expression) for the unique constraint defined on the fields: Iso3 .</summary>
		/// <returns>true if succeeded and the contents is read, false otherwise</returns>
		public IPredicateExpression ConstructFilterForUCIso3()
		{
			var filter = new PredicateExpression();
			filter.Add(TournamentCalendarDAL.HelperClasses.CountryFields.Iso3 == this.Fields.GetCurrentValue((int)CountryFieldIndex.Iso3));
 			return filter;
		}

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entities of type 'Calendar' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoTournamentCalendars() { return CreateRelationInfoForNavigator("TournamentCalendars"); }

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entities of type 'InfoService' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoInfoServices() { return CreateRelationInfoForNavigator("InfoServices"); }
		
		/// <inheritdoc/>
		protected override EntityStaticMetaDataBase GetEntityStaticMetaData() {	return _staticMetaData; }

		/// <summary>Initializes the class members</summary>
		private void InitClassMembers()
		{
			PerformDependencyInjection();
			// __LLBLGENPRO_USER_CODE_REGION_START InitClassMembers
			// __LLBLGENPRO_USER_CODE_REGION_END
			OnInitClassMembersComplete();
		}

		/// <summary>Initializes the class with empty data, as if it is a new Entity.</summary>
		/// <param name="validator">The validator object for this CountryEntity</param>
		/// <param name="fields">Fields of this entity</param>
		private void InitClassEmpty(IValidator validator, IEntityFields2 fields)
		{
			OnInitializing();
			this.Fields = fields ?? CreateFields();
			this.Validator = validator;
			InitClassMembers();
			// __LLBLGENPRO_USER_CODE_REGION_START InitClassEmpty
			// __LLBLGENPRO_USER_CODE_REGION_END

			OnInitialized();
		}

		/// <summary>The relations object holding all relations of this entity with other entity classes.</summary>
		public static CountryRelations Relations { get { return _relationsFactory; } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'Calendar' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathTournamentCalendars { get { return _staticMetaData.GetPrefetchPathElement("TournamentCalendars", CommonEntityBase.CreateEntityCollection<CalendarEntity>()); } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'InfoService' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathInfoServices { get { return _staticMetaData.GetPrefetchPathElement("InfoServices", CommonEntityBase.CreateEntityCollection<InfoServiceEntity>()); } }

		/// <summary>The CreatedOn property of the Entity Country<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Country"."CreatedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime CreatedOn
		{
			get { return (System.DateTime)GetValue((int)CountryFieldIndex.CreatedOn, true); }
			set	{ SetValue((int)CountryFieldIndex.CreatedOn, value); }
		}

		/// <summary>The Id property of the Entity Country<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Country"."Id".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 2.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, true, false</remarks>
		public virtual System.String Id
		{
			get { return (System.String)GetValue((int)CountryFieldIndex.Id, true); }
			set	{ SetValue((int)CountryFieldIndex.Id, value); }
		}

		/// <summary>The Iso3 property of the Entity Country<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Country"."Iso3".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 3.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Iso3
		{
			get { return (System.String)GetValue((int)CountryFieldIndex.Iso3, true); }
			set	{ SetValue((int)CountryFieldIndex.Iso3, value); }
		}

		/// <summary>The ModifiedOn property of the Entity Country<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Country"."ModifiedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime ModifiedOn
		{
			get { return (System.DateTime)GetValue((int)CountryFieldIndex.ModifiedOn, true); }
			set	{ SetValue((int)CountryFieldIndex.ModifiedOn, value); }
		}

		/// <summary>The Name property of the Entity Country<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Country"."Name".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 255.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Name
		{
			get { return (System.String)GetValue((int)CountryFieldIndex.Name, true); }
			set	{ SetValue((int)CountryFieldIndex.Name, value); }
		}

		/// <summary>Gets the EntityCollection with the related entities of type 'CalendarEntity' which are related to this entity via a relation of type '1:n'. If the EntityCollection hasn't been fetched yet, the collection returned will be empty.<br/><br/></summary>
		[TypeContainedAttribute(typeof(CalendarEntity))]
		public virtual EntityCollection<CalendarEntity> TournamentCalendars { get { return GetOrCreateEntityCollection<CalendarEntity, CalendarEntityFactory>("Country", true, false, ref _tournamentCalendars); } }

		/// <summary>Gets the EntityCollection with the related entities of type 'InfoServiceEntity' which are related to this entity via a relation of type '1:n'. If the EntityCollection hasn't been fetched yet, the collection returned will be empty.<br/><br/></summary>
		[TypeContainedAttribute(typeof(InfoServiceEntity))]
		public virtual EntityCollection<InfoServiceEntity> InfoServices { get { return GetOrCreateEntityCollection<InfoServiceEntity, InfoServiceEntityFactory>("Country", true, false, ref _infoServices); } }

		// __LLBLGENPRO_USER_CODE_REGION_START CustomEntityCode
		// __LLBLGENPRO_USER_CODE_REGION_END

	}
}

namespace TournamentCalendarDAL
{
	public enum CountryFieldIndex
	{
		///<summary>CreatedOn. </summary>
		CreatedOn,
		///<summary>Id. </summary>
		Id,
		///<summary>Iso3. </summary>
		Iso3,
		///<summary>ModifiedOn. </summary>
		ModifiedOn,
		///<summary>Name. </summary>
		Name,
		/// <summary></summary>
		AmountOfFields
	}
}

namespace TournamentCalendarDAL.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: Country. </summary>
	public partial class CountryRelations: RelationFactory
	{
		/// <summary>Returns a new IEntityRelation object, between CountryEntity and CalendarEntity over the 1:n relation they have, using the relation between the fields: Country.Id - Calendar.CountryId</summary>
		public virtual IEntityRelation CalendarEntityUsingCountryId
		{
			get { return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.OneToMany, "TournamentCalendars", true, new[] { CountryFields.Id, CalendarFields.CountryId }); }
		}

		/// <summary>Returns a new IEntityRelation object, between CountryEntity and InfoServiceEntity over the 1:n relation they have, using the relation between the fields: Country.Id - InfoService.CountryId</summary>
		public virtual IEntityRelation InfoServiceEntityUsingCountryId
		{
			get { return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.OneToMany, "InfoServices", true, new[] { CountryFields.Id, InfoServiceFields.CountryId }); }
		}

	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticCountryRelations
	{
		internal static readonly IEntityRelation CalendarEntityUsingCountryIdStatic = new CountryRelations().CalendarEntityUsingCountryId;
		internal static readonly IEntityRelation InfoServiceEntityUsingCountryIdStatic = new CountryRelations().InfoServiceEntityUsingCountryId;

		/// <summary>CTor</summary>
		static StaticCountryRelations() { }
	}
}
