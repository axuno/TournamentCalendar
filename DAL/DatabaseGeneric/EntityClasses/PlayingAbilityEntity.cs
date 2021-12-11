﻿//////////////////////////////////////////////////////////////
// <auto-generated>This code was generated by LLBLGen Pro 5.7.</auto-generated>
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
	/// <summary>Entity class which represents the entity 'PlayingAbility'.<br/><br/></summary>
	[Serializable]
	public partial class PlayingAbilityEntity : CommonEntityBase
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfaces
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		private EntityCollection<CalendarEntity> _tournamentCalendars;
		private EntityCollection<CalendarEntity> _tournamentCalendars_;

		// __LLBLGENPRO_USER_CODE_REGION_START PrivateMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static PlayingAbilityEntityStaticMetaData _staticMetaData = new PlayingAbilityEntityStaticMetaData();
		private static PlayingAbilityRelations _relationsFactory = new PlayingAbilityRelations();

		/// <summary>All names of fields mapped onto a relation. Usable for in-memory filtering</summary>
		public static partial class MemberNames
		{
			/// <summary>Member name TournamentCalendars</summary>
			public static readonly string TournamentCalendars = "TournamentCalendars";
			/// <summary>Member name TournamentCalendars_</summary>
			public static readonly string TournamentCalendars_ = "TournamentCalendars_";
		}

		/// <summary>Static meta-data storage for navigator related information</summary>
		protected class PlayingAbilityEntityStaticMetaData : EntityStaticMetaDataBase
		{
			public PlayingAbilityEntityStaticMetaData()
			{
				SetEntityCoreInfo("PlayingAbilityEntity", InheritanceHierarchyType.None, false, (int)TournamentCalendarDAL.EntityType.PlayingAbilityEntity, typeof(PlayingAbilityEntity), typeof(PlayingAbilityEntityFactory), false);
				AddNavigatorMetaData<PlayingAbilityEntity, EntityCollection<CalendarEntity>>("TournamentCalendars", a => a._tournamentCalendars, (a, b) => a._tournamentCalendars = b, a => a.TournamentCalendars, () => new PlayingAbilityRelations().CalendarEntityUsingPlayingAbilityFrom, typeof(CalendarEntity), (int)TournamentCalendarDAL.EntityType.CalendarEntity);
				AddNavigatorMetaData<PlayingAbilityEntity, EntityCollection<CalendarEntity>>("TournamentCalendars_", a => a._tournamentCalendars_, (a, b) => a._tournamentCalendars_ = b, a => a.TournamentCalendars_, () => new PlayingAbilityRelations().CalendarEntityUsingPlayingAbilityTo, typeof(CalendarEntity), (int)TournamentCalendarDAL.EntityType.CalendarEntity);
			}
		}

		/// <summary>Static ctor</summary>
		static PlayingAbilityEntity()
		{
		}

		/// <summary> CTor</summary>
		public PlayingAbilityEntity()
		{
			InitClassEmpty(null, null);
		}

		/// <summary> CTor</summary>
		/// <param name="fields">Fields object to set as the fields for this entity.</param>
		public PlayingAbilityEntity(IEntityFields2 fields)
		{
			InitClassEmpty(null, fields);
		}

		/// <summary> CTor</summary>
		/// <param name="validator">The custom validator object for this PlayingAbilityEntity</param>
		public PlayingAbilityEntity(IValidator validator)
		{
			InitClassEmpty(validator, null);
		}

		/// <summary> CTor</summary>
		/// <param name="strength">PK value for PlayingAbility which data should be fetched into this PlayingAbility object</param>
		public PlayingAbilityEntity(System.Int64 strength) : this(strength, null)
		{
		}

		/// <summary> CTor</summary>
		/// <param name="strength">PK value for PlayingAbility which data should be fetched into this PlayingAbility object</param>
		/// <param name="validator">The custom validator object for this PlayingAbilityEntity</param>
		public PlayingAbilityEntity(System.Int64 strength, IValidator validator)
		{
			InitClassEmpty(validator, null);
			this.Strength = strength;
		}

		/// <summary>Private CTor for deserialization</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected PlayingAbilityEntity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// __LLBLGENPRO_USER_CODE_REGION_START DeserializationConstructor
			// __LLBLGENPRO_USER_CODE_REGION_END
		}

		/// <summary>Method which will construct a filter (predicate expression) for the unique constraint defined on the fields: Id .</summary>
		/// <returns>true if succeeded and the contents is read, false otherwise</returns>
		public IPredicateExpression ConstructFilterForUCId()
		{
			var filter = new PredicateExpression();
			filter.Add(TournamentCalendarDAL.HelperClasses.PlayingAbilityFields.Id == this.Fields.GetCurrentValue((int)PlayingAbilityFieldIndex.Id));
 			return filter;
		}

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entities of type 'Calendar' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoTournamentCalendars() { return CreateRelationInfoForNavigator("TournamentCalendars"); }

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entities of type 'Calendar' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoTournamentCalendars_() { return CreateRelationInfoForNavigator("TournamentCalendars_"); }
		
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
		/// <param name="validator">The validator object for this PlayingAbilityEntity</param>
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
		public static PlayingAbilityRelations Relations { get { return _relationsFactory; } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'Calendar' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathTournamentCalendars { get { return _staticMetaData.GetPrefetchPathElement("TournamentCalendars", CommonEntityBase.CreateEntityCollection<CalendarEntity>()); } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'Calendar' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathTournamentCalendars_ { get { return _staticMetaData.GetPrefetchPathElement("TournamentCalendars_", CommonEntityBase.CreateEntityCollection<CalendarEntity>()); } }

		/// <summary>The CreatedOn property of the Entity PlayingAbility<br/><br/></summary>
		/// <remarks>Mapped on  table field: "PlayingAbility"."CreatedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime CreatedOn
		{
			get { return (System.DateTime)GetValue((int)PlayingAbilityFieldIndex.CreatedOn, true); }
			set	{ SetValue((int)PlayingAbilityFieldIndex.CreatedOn, value); }
		}

		/// <summary>The Description property of the Entity PlayingAbility<br/><br/></summary>
		/// <remarks>Mapped on  table field: "PlayingAbility"."Description".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 255.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Description
		{
			get { return (System.String)GetValue((int)PlayingAbilityFieldIndex.Description, true); }
			set	{ SetValue((int)PlayingAbilityFieldIndex.Description, value); }
		}

		/// <summary>The Id property of the Entity PlayingAbility<br/><br/></summary>
		/// <remarks>Mapped on  table field: "PlayingAbility"."Id".<br/>Table field type characteristics (type, precision, scale, length): BigInt, 19, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, true</remarks>
		public virtual System.Int64 Id
		{
			get { return (System.Int64)GetValue((int)PlayingAbilityFieldIndex.Id, true); }
		}

		/// <summary>The ModifiedOn property of the Entity PlayingAbility<br/><br/></summary>
		/// <remarks>Mapped on  table field: "PlayingAbility"."ModifiedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime ModifiedOn
		{
			get { return (System.DateTime)GetValue((int)PlayingAbilityFieldIndex.ModifiedOn, true); }
			set	{ SetValue((int)PlayingAbilityFieldIndex.ModifiedOn, value); }
		}

		/// <summary>The Strength property of the Entity PlayingAbility<br/><br/></summary>
		/// <remarks>Mapped on  table field: "PlayingAbility"."Strength".<br/>Table field type characteristics (type, precision, scale, length): BigInt, 19, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, true, false</remarks>
		public virtual System.Int64 Strength
		{
			get { return (System.Int64)GetValue((int)PlayingAbilityFieldIndex.Strength, true); }
			set	{ SetValue((int)PlayingAbilityFieldIndex.Strength, value); }
		}

		/// <summary>Gets the EntityCollection with the related entities of type 'CalendarEntity' which are related to this entity via a relation of type '1:n'. If the EntityCollection hasn't been fetched yet, the collection returned will be empty.<br/><br/></summary>
		[TypeContainedAttribute(typeof(CalendarEntity))]
		public virtual EntityCollection<CalendarEntity> TournamentCalendars { get { return GetOrCreateEntityCollection<CalendarEntity, CalendarEntityFactory>("TournamentPlayingAbilityFrom", true, false, ref _tournamentCalendars); } }

		/// <summary>Gets the EntityCollection with the related entities of type 'CalendarEntity' which are related to this entity via a relation of type '1:n'. If the EntityCollection hasn't been fetched yet, the collection returned will be empty.<br/><br/></summary>
		[TypeContainedAttribute(typeof(CalendarEntity))]
		public virtual EntityCollection<CalendarEntity> TournamentCalendars_ { get { return GetOrCreateEntityCollection<CalendarEntity, CalendarEntityFactory>("TournamentPlayingAbilityTo", true, false, ref _tournamentCalendars_); } }

		// __LLBLGENPRO_USER_CODE_REGION_START CustomEntityCode
		// __LLBLGENPRO_USER_CODE_REGION_END

	}
}

namespace TournamentCalendarDAL
{
	public enum PlayingAbilityFieldIndex
	{
		///<summary>CreatedOn. </summary>
		CreatedOn,
		///<summary>Description. </summary>
		Description,
		///<summary>Id. </summary>
		Id,
		///<summary>ModifiedOn. </summary>
		ModifiedOn,
		///<summary>Strength. </summary>
		Strength,
		/// <summary></summary>
		AmountOfFields
	}
}

namespace TournamentCalendarDAL.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: PlayingAbility. </summary>
	public partial class PlayingAbilityRelations: RelationFactory
	{
		/// <summary>Returns a new IEntityRelation object, between PlayingAbilityEntity and CalendarEntity over the 1:n relation they have, using the relation between the fields: PlayingAbility.Strength - Calendar.PlayingAbilityFrom</summary>
		public virtual IEntityRelation CalendarEntityUsingPlayingAbilityFrom
		{
			get { return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.OneToMany, "TournamentCalendars", true, new[] { PlayingAbilityFields.Strength, CalendarFields.PlayingAbilityFrom }); }
		}

		/// <summary>Returns a new IEntityRelation object, between PlayingAbilityEntity and CalendarEntity over the 1:n relation they have, using the relation between the fields: PlayingAbility.Strength - Calendar.PlayingAbilityTo</summary>
		public virtual IEntityRelation CalendarEntityUsingPlayingAbilityTo
		{
			get { return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.OneToMany, "TournamentCalendars_", true, new[] { PlayingAbilityFields.Strength, CalendarFields.PlayingAbilityTo }); }
		}

	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticPlayingAbilityRelations
	{
		internal static readonly IEntityRelation CalendarEntityUsingPlayingAbilityFromStatic = new PlayingAbilityRelations().CalendarEntityUsingPlayingAbilityFrom;
		internal static readonly IEntityRelation CalendarEntityUsingPlayingAbilityToStatic = new PlayingAbilityRelations().CalendarEntityUsingPlayingAbilityTo;

		/// <summary>CTor</summary>
		static StaticPlayingAbilityRelations() { }
	}
}
