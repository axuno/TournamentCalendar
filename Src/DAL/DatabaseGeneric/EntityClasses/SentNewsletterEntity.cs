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
	/// <summary>Entity class which represents the entity 'SentNewsletter'.<br/><br/></summary>
	[Serializable]
	public partial class SentNewsletterEntity : CommonEntityBase
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfaces
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{

		// __LLBLGENPRO_USER_CODE_REGION_START PrivateMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static SentNewsletterEntityStaticMetaData _staticMetaData = new SentNewsletterEntityStaticMetaData();
		private static SentNewsletterRelations _relationsFactory = new SentNewsletterRelations();

		/// <summary>All names of fields mapped onto a relation. Usable for in-memory filtering</summary>
		public static partial class MemberNames
		{
		}

		/// <summary>Static meta-data storage for navigator related information</summary>
		protected class SentNewsletterEntityStaticMetaData : EntityStaticMetaDataBase
		{
			public SentNewsletterEntityStaticMetaData()
			{
				SetEntityCoreInfo("SentNewsletterEntity", InheritanceHierarchyType.None, false, (int)TournamentCalendarDAL.EntityType.SentNewsletterEntity, typeof(SentNewsletterEntity), typeof(SentNewsletterEntityFactory), false);
			}
		}

		/// <summary>Static ctor</summary>
		static SentNewsletterEntity()
		{
		}

		/// <summary> CTor</summary>
		public SentNewsletterEntity()
		{
			InitClassEmpty(null, null);
		}

		/// <summary> CTor</summary>
		/// <param name="fields">Fields object to set as the fields for this entity.</param>
		public SentNewsletterEntity(IEntityFields2 fields)
		{
			InitClassEmpty(null, fields);
		}

		/// <summary> CTor</summary>
		/// <param name="validator">The custom validator object for this SentNewsletterEntity</param>
		public SentNewsletterEntity(IValidator validator)
		{
			InitClassEmpty(validator, null);
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for SentNewsletter which data should be fetched into this SentNewsletter object</param>
		public SentNewsletterEntity(System.Int64 id) : this(id, null)
		{
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for SentNewsletter which data should be fetched into this SentNewsletter object</param>
		/// <param name="validator">The custom validator object for this SentNewsletterEntity</param>
		public SentNewsletterEntity(System.Int64 id, IValidator validator)
		{
			InitClassEmpty(validator, null);
			this.Id = id;
		}

		/// <summary>Private CTor for deserialization</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected SentNewsletterEntity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// __LLBLGENPRO_USER_CODE_REGION_START DeserializationConstructor
			// __LLBLGENPRO_USER_CODE_REGION_END
		}
		
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
		/// <param name="validator">The validator object for this SentNewsletterEntity</param>
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
		public static SentNewsletterRelations Relations { get { return _relationsFactory; } }

		/// <summary>The CompletedOn property of the Entity SentNewsletter<br/><br/></summary>
		/// <remarks>Mapped on  table field: "SentNewsletters"."CompletedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual Nullable<System.DateTime> CompletedOn
		{
			get { return (Nullable<System.DateTime>)GetValue((int)SentNewsletterFieldIndex.CompletedOn, false); }
			set { SetValue((int)SentNewsletterFieldIndex.CompletedOn, value); }
		}

		/// <summary>The Id property of the Entity SentNewsletter<br/><br/></summary>
		/// <remarks>Mapped on  table field: "SentNewsletters"."Id".<br/>Table field type characteristics (type, precision, scale, length): BigInt, 19, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, true, true</remarks>
		public virtual System.Int64 Id
		{
			get { return (System.Int64)GetValue((int)SentNewsletterFieldIndex.Id, true); }
			set { SetValue((int)SentNewsletterFieldIndex.Id, value); }
		}

		/// <summary>The NumOfRecipients property of the Entity SentNewsletter<br/><br/></summary>
		/// <remarks>Mapped on  table field: "SentNewsletters"."NumOfRecipients".<br/>Table field type characteristics (type, precision, scale, length): Int, 10, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.Int32 NumOfRecipients
		{
			get { return (System.Int32)GetValue((int)SentNewsletterFieldIndex.NumOfRecipients, true); }
			set { SetValue((int)SentNewsletterFieldIndex.NumOfRecipients, value); }
		}

		/// <summary>The NumOfTournaments property of the Entity SentNewsletter<br/><br/></summary>
		/// <remarks>Mapped on  table field: "SentNewsletters"."NumOfTournaments".<br/>Table field type characteristics (type, precision, scale, length): Int, 10, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.Int32 NumOfTournaments
		{
			get { return (System.Int32)GetValue((int)SentNewsletterFieldIndex.NumOfTournaments, true); }
			set { SetValue((int)SentNewsletterFieldIndex.NumOfTournaments, value); }
		}

		/// <summary>The StartedOn property of the Entity SentNewsletter<br/><br/></summary>
		/// <remarks>Mapped on  table field: "SentNewsletters"."StartedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime StartedOn
		{
			get { return (System.DateTime)GetValue((int)SentNewsletterFieldIndex.StartedOn, true); }
			set { SetValue((int)SentNewsletterFieldIndex.StartedOn, value); }
		}

		// __LLBLGENPRO_USER_CODE_REGION_START CustomEntityCode
		// __LLBLGENPRO_USER_CODE_REGION_END

	}
}

namespace TournamentCalendarDAL
{
	public enum SentNewsletterFieldIndex
	{
		///<summary>CompletedOn. </summary>
		CompletedOn,
		///<summary>Id. </summary>
		Id,
		///<summary>NumOfRecipients. </summary>
		NumOfRecipients,
		///<summary>NumOfTournaments. </summary>
		NumOfTournaments,
		///<summary>StartedOn. </summary>
		StartedOn,
		/// <summary></summary>
		AmountOfFields
	}
}

namespace TournamentCalendarDAL.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: SentNewsletter. </summary>
	public partial class SentNewsletterRelations: RelationFactory
	{

	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticSentNewsletterRelations
	{

		/// <summary>CTor</summary>
		static StaticSentNewsletterRelations() { }
	}
}
