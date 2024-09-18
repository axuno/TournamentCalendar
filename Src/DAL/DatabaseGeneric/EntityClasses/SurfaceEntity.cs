﻿//////////////////////////////////////////////////////////////
// <auto-generated>This code was generated by LLBLGen Pro 5.11.</auto-generated>
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
	/// <summary>Entity class which represents the entity 'Surface'.<br/><br/></summary>
	[Serializable]
	public partial class SurfaceEntity : CommonEntityBase
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfaces
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		private EntityCollection<CalendarEntity> _tournamentCalendars;

		// __LLBLGENPRO_USER_CODE_REGION_START PrivateMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static SurfaceEntityStaticMetaData _staticMetaData = new SurfaceEntityStaticMetaData();
		private static SurfaceRelations _relationsFactory = new SurfaceRelations();

		/// <summary>All names of fields mapped onto a relation. Usable for in-memory filtering</summary>
		public static partial class MemberNames
		{
			/// <summary>Member name TournamentCalendars</summary>
			public static readonly string TournamentCalendars = "TournamentCalendars";
		}

		/// <summary>Static meta-data storage for navigator related information</summary>
		protected class SurfaceEntityStaticMetaData : EntityStaticMetaDataBase
		{
			public SurfaceEntityStaticMetaData()
			{
				SetEntityCoreInfo("SurfaceEntity", InheritanceHierarchyType.None, false, (int)TournamentCalendarDAL.EntityType.SurfaceEntity, typeof(SurfaceEntity), typeof(SurfaceEntityFactory), false);
				AddNavigatorMetaData<SurfaceEntity, EntityCollection<CalendarEntity>>("TournamentCalendars", a => a._tournamentCalendars, (a, b) => a._tournamentCalendars = b, a => a.TournamentCalendars, () => new SurfaceRelations().CalendarEntityUsingSurface, typeof(CalendarEntity), (int)TournamentCalendarDAL.EntityType.CalendarEntity);
			}
		}

		/// <summary>Static ctor</summary>
		static SurfaceEntity()
		{
		}

		/// <summary> CTor</summary>
		public SurfaceEntity()
		{
			InitClassEmpty(null, null);
		}

		/// <summary> CTor</summary>
		/// <param name="fields">Fields object to set as the fields for this entity.</param>
		public SurfaceEntity(IEntityFields2 fields)
		{
			InitClassEmpty(null, fields);
		}

		/// <summary> CTor</summary>
		/// <param name="validator">The custom validator object for this SurfaceEntity</param>
		public SurfaceEntity(IValidator validator)
		{
			InitClassEmpty(validator, null);
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for Surface which data should be fetched into this Surface object</param>
		public SurfaceEntity(System.Int64 id) : this(id, null)
		{
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for Surface which data should be fetched into this Surface object</param>
		/// <param name="validator">The custom validator object for this SurfaceEntity</param>
		public SurfaceEntity(System.Int64 id, IValidator validator)
		{
			InitClassEmpty(validator, null);
			this.Id = id;
		}

		/// <summary>Private CTor for deserialization</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SurfaceEntity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// __LLBLGENPRO_USER_CODE_REGION_START DeserializationConstructor
			// __LLBLGENPRO_USER_CODE_REGION_END
		}

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entities of type 'Calendar' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoTournamentCalendars() { return CreateRelationInfoForNavigator("TournamentCalendars"); }
		
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
		/// <param name="validator">The validator object for this SurfaceEntity</param>
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
		public static SurfaceRelations Relations { get { return _relationsFactory; } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'Calendar' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathTournamentCalendars { get { return _staticMetaData.GetPrefetchPathElement("TournamentCalendars", CommonEntityBase.CreateEntityCollection<CalendarEntity>()); } }

		/// <summary>The Id property of the Entity Surface<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Surface"."Id".<br/>Table field type characteristics (type, precision, scale, length): BigInt, 19, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, true, true</remarks>
		public virtual System.Int64 Id
		{
			get { return (System.Int64)GetValue((int)SurfaceFieldIndex.Id, true); }
			set { SetValue((int)SurfaceFieldIndex.Id, value); }
		}

		/// <summary>The Description property of the Entity Surface<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Surface"."Description".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 255.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Description
		{
			get { return (System.String)GetValue((int)SurfaceFieldIndex.Description, true); }
			set { SetValue((int)SurfaceFieldIndex.Description, value); }
		}

		/// <summary>The CreatedOn property of the Entity Surface<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Surface"."CreatedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime CreatedOn
		{
			get { return (System.DateTime)GetValue((int)SurfaceFieldIndex.CreatedOn, true); }
			set { SetValue((int)SurfaceFieldIndex.CreatedOn, value); }
		}

		/// <summary>The ModifiedOn property of the Entity Surface<br/><br/></summary>
		/// <remarks>Mapped on  table field: "Surface"."ModifiedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime ModifiedOn
		{
			get { return (System.DateTime)GetValue((int)SurfaceFieldIndex.ModifiedOn, true); }
			set { SetValue((int)SurfaceFieldIndex.ModifiedOn, value); }
		}

		/// <summary>Gets the EntityCollection with the related entities of type 'CalendarEntity' which are related to this entity via a relation of type '1:n'. If the EntityCollection hasn't been fetched yet, the collection returned will be empty.<br/><br/></summary>
		[TypeContainedAttribute(typeof(CalendarEntity))]
		public virtual EntityCollection<CalendarEntity> TournamentCalendars { get { return GetOrCreateEntityCollection<CalendarEntity, CalendarEntityFactory>("TournamentSurface", true, false, ref _tournamentCalendars); } }

		// __LLBLGENPRO_USER_CODE_REGION_START CustomEntityCode
		// __LLBLGENPRO_USER_CODE_REGION_END

	}
}

namespace TournamentCalendarDAL
{
	public enum SurfaceFieldIndex
	{
		///<summary>Id. </summary>
		Id,
		///<summary>Description. </summary>
		Description,
		///<summary>CreatedOn. </summary>
		CreatedOn,
		///<summary>ModifiedOn. </summary>
		ModifiedOn,
		/// <summary></summary>
		AmountOfFields
	}
}

namespace TournamentCalendarDAL.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: Surface. </summary>
	public partial class SurfaceRelations: RelationFactory
	{
		/// <summary>Returns a new IEntityRelation object, between SurfaceEntity and CalendarEntity over the 1:n relation they have, using the relation between the fields: Surface.Id - Calendar.Surface</summary>
		public virtual IEntityRelation CalendarEntityUsingSurface
		{
			get { return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.OneToMany, "TournamentCalendars", true, new[] { SurfaceFields.Id, CalendarFields.Surface }); }
		}

	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticSurfaceRelations
	{
		internal static readonly IEntityRelation CalendarEntityUsingSurfaceStatic = new SurfaceRelations().CalendarEntityUsingSurface;

		/// <summary>CTor</summary>
		static StaticSurfaceRelations() { }
	}
}
