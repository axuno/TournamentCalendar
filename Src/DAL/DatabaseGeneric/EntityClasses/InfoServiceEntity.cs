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
	/// <summary>Entity class which represents the entity 'InfoService'.<br/><br/></summary>
	[Serializable]
	public partial class InfoServiceEntity : CommonEntityBase
		// __LLBLGENPRO_USER_CODE_REGION_START AdditionalInterfaces
		// __LLBLGENPRO_USER_CODE_REGION_END	
	{
		private CountryEntity _country;

		// __LLBLGENPRO_USER_CODE_REGION_START PrivateMembers
		// __LLBLGENPRO_USER_CODE_REGION_END
		private static InfoServiceEntityStaticMetaData _staticMetaData = new InfoServiceEntityStaticMetaData();
		private static InfoServiceRelations _relationsFactory = new InfoServiceRelations();

		/// <summary>All names of fields mapped onto a relation. Usable for in-memory filtering</summary>
		public static partial class MemberNames
		{
			/// <summary>Member name Country</summary>
			public static readonly string Country = "Country";
		}

		/// <summary>Static meta-data storage for navigator related information</summary>
		protected class InfoServiceEntityStaticMetaData : EntityStaticMetaDataBase
		{
			public InfoServiceEntityStaticMetaData()
			{
				SetEntityCoreInfo("InfoServiceEntity", InheritanceHierarchyType.None, false, (int)TournamentCalendarDAL.EntityType.InfoServiceEntity, typeof(InfoServiceEntity), typeof(InfoServiceEntityFactory), false);
				AddNavigatorMetaData<InfoServiceEntity, CountryEntity>("Country", "InfoServices", (a, b) => a._country = b, a => a._country, (a, b) => a.Country = b, TournamentCalendarDAL.RelationClasses.StaticInfoServiceRelations.CountryEntityUsingCountryIdStatic, ()=>new InfoServiceRelations().CountryEntityUsingCountryId, null, new int[] { (int)InfoServiceFieldIndex.CountryId }, null, true, (int)TournamentCalendarDAL.EntityType.CountryEntity);
			}
		}

		/// <summary>Static ctor</summary>
		static InfoServiceEntity()
		{
		}

		/// <summary> CTor</summary>
		public InfoServiceEntity()
		{
			InitClassEmpty(null, null);
		}

		/// <summary> CTor</summary>
		/// <param name="fields">Fields object to set as the fields for this entity.</param>
		public InfoServiceEntity(IEntityFields2 fields)
		{
			InitClassEmpty(null, fields);
		}

		/// <summary> CTor</summary>
		/// <param name="validator">The custom validator object for this InfoServiceEntity</param>
		public InfoServiceEntity(IValidator validator)
		{
			InitClassEmpty(validator, null);
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for InfoService which data should be fetched into this InfoService object</param>
		public InfoServiceEntity(System.Int32 id) : this(id, null)
		{
		}

		/// <summary> CTor</summary>
		/// <param name="id">PK value for InfoService which data should be fetched into this InfoService object</param>
		/// <param name="validator">The custom validator object for this InfoServiceEntity</param>
		public InfoServiceEntity(System.Int32 id, IValidator validator)
		{
			InitClassEmpty(validator, null);
			this.Id = id;
		}

		/// <summary>Private CTor for deserialization</summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected InfoServiceEntity(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			// __LLBLGENPRO_USER_CODE_REGION_START DeserializationConstructor
			// __LLBLGENPRO_USER_CODE_REGION_END
		}

		/// <summary>Creates a new IRelationPredicateBucket object which contains the predicate expression and relation collection to fetch the related entity of type 'Country' to this entity.</summary>
		/// <returns></returns>
		public virtual IRelationPredicateBucket GetRelationInfoCountry() { return CreateRelationInfoForNavigator("Country"); }
		
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
		/// <param name="validator">The validator object for this InfoServiceEntity</param>
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
		public static InfoServiceRelations Relations { get { return _relationsFactory; } }

		/// <summary>Creates a new PrefetchPathElement2 object which contains all the information to prefetch the related entities of type 'Country' for this entity.</summary>
		/// <returns>Ready to use IPrefetchPathElement2 implementation.</returns>
		public static IPrefetchPathElement2 PrefetchPathCountry { get { return _staticMetaData.GetPrefetchPathElement("Country", CommonEntityBase.CreateEntityCollection<CountryEntity>()); } }

		/// <summary>The City property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."City".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String City
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.City, true); }
			set { SetValue((int)InfoServiceFieldIndex.City, value); }
		}

		/// <summary>The Comments property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Comments".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 255.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Comments
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Comments, true); }
			set { SetValue((int)InfoServiceFieldIndex.Comments, value); }
		}

		/// <summary>The ConfirmedOn property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."ConfirmedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual Nullable<System.DateTime> ConfirmedOn
		{
			get { return (Nullable<System.DateTime>)GetValue((int)InfoServiceFieldIndex.ConfirmedOn, false); }
			set { SetValue((int)InfoServiceFieldIndex.ConfirmedOn, value); }
		}

		/// <summary>The CountryId property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."CountryId".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 2.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual System.String CountryId
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.CountryId, true); }
			set { SetValue((int)InfoServiceFieldIndex.CountryId, value); }
		}

		/// <summary>The Email property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Email".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Email
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Email, true); }
			set { SetValue((int)InfoServiceFieldIndex.Email, value); }
		}

		/// <summary>The FirstName property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."FirstName".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String FirstName
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.FirstName, true); }
			set { SetValue((int)InfoServiceFieldIndex.FirstName, value); }
		}

		/// <summary>The Gender property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Gender".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Gender
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Gender, true); }
			set { SetValue((int)InfoServiceFieldIndex.Gender, value); }
		}

		/// <summary>The Guid property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Guid".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 64.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual System.String Guid
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Guid, true); }
			set { SetValue((int)InfoServiceFieldIndex.Guid, value); }
		}

		/// <summary>The Id property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Id".<br/>Table field type characteristics (type, precision, scale, length): Int, 10, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, true, true</remarks>
		public virtual System.Int32 Id
		{
			get { return (System.Int32)GetValue((int)InfoServiceFieldIndex.Id, true); }
			set { SetValue((int)InfoServiceFieldIndex.Id, value); }
		}

		/// <summary>The LastName property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."LastName".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String LastName
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.LastName, true); }
			set { SetValue((int)InfoServiceFieldIndex.LastName, value); }
		}

		/// <summary>The Latitude property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Latitude".<br/>Table field type characteristics (type, precision, scale, length): Float, 38, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual Nullable<System.Double> Latitude
		{
			get { return (Nullable<System.Double>)GetValue((int)InfoServiceFieldIndex.Latitude, false); }
			set { SetValue((int)InfoServiceFieldIndex.Latitude, value); }
		}

		/// <summary>The Longitude property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Longitude".<br/>Table field type characteristics (type, precision, scale, length): Float, 38, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual Nullable<System.Double> Longitude
		{
			get { return (Nullable<System.Double>)GetValue((int)InfoServiceFieldIndex.Longitude, false); }
			set { SetValue((int)InfoServiceFieldIndex.Longitude, value); }
		}

		/// <summary>The MaxDistance property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."MaxDistance".<br/>Table field type characteristics (type, precision, scale, length): Int, 10, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): true, false, false</remarks>
		public virtual Nullable<System.Int32> MaxDistance
		{
			get { return (Nullable<System.Int32>)GetValue((int)InfoServiceFieldIndex.MaxDistance, false); }
			set { SetValue((int)InfoServiceFieldIndex.MaxDistance, value); }
		}

		/// <summary>The ModifiedOn property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."ModifiedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime ModifiedOn
		{
			get { return (System.DateTime)GetValue((int)InfoServiceFieldIndex.ModifiedOn, true); }
			set { SetValue((int)InfoServiceFieldIndex.ModifiedOn, value); }
		}

		/// <summary>The Street property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Street".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 255.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Street
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Street, true); }
			set { SetValue((int)InfoServiceFieldIndex.Street, value); }
		}

		/// <summary>The SubscribedOn property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."SubscribedOn".<br/>Table field type characteristics (type, precision, scale, length): DateTime, 0, 0, 0.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.DateTime SubscribedOn
		{
			get { return (System.DateTime)GetValue((int)InfoServiceFieldIndex.SubscribedOn, true); }
			set { SetValue((int)InfoServiceFieldIndex.SubscribedOn, value); }
		}

		/// <summary>The Title property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."Title".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 30.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String Title
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.Title, true); }
			set { SetValue((int)InfoServiceFieldIndex.Title, value); }
		}

		/// <summary>The UserName property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."UserName".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String UserName
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.UserName, true); }
			set { SetValue((int)InfoServiceFieldIndex.UserName, value); }
		}

		/// <summary>The UserPassword property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."UserPassword".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 50.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String UserPassword
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.UserPassword, true); }
			set { SetValue((int)InfoServiceFieldIndex.UserPassword, value); }
		}

		/// <summary>The ZipCode property of the Entity InfoService<br/><br/></summary>
		/// <remarks>Mapped on  table field: "InfoService"."ZipCode".<br/>Table field type characteristics (type, precision, scale, length): NVarChar, 0, 0, 6.<br/>Table field behavior characteristics (is nullable, is PK, is identity): false, false, false</remarks>
		public virtual System.String ZipCode
		{
			get { return (System.String)GetValue((int)InfoServiceFieldIndex.ZipCode, true); }
			set { SetValue((int)InfoServiceFieldIndex.ZipCode, value); }
		}

		/// <summary>Gets / sets related entity of type 'CountryEntity' which has to be set using a fetch action earlier. If no related entity is set for this property, null is returned..<br/><br/></summary>
		[Browsable(false)]
		public virtual CountryEntity Country
		{
			get { return _country; }
			set { SetSingleRelatedEntityNavigator(value, "Country"); }
		}

		// __LLBLGENPRO_USER_CODE_REGION_START CustomEntityCode
		// __LLBLGENPRO_USER_CODE_REGION_END

	}
}

namespace TournamentCalendarDAL
{
	public enum InfoServiceFieldIndex
	{
		///<summary>City. </summary>
		City,
		///<summary>Comments. </summary>
		Comments,
		///<summary>ConfirmedOn. </summary>
		ConfirmedOn,
		///<summary>CountryId. </summary>
		CountryId,
		///<summary>Email. </summary>
		Email,
		///<summary>FirstName. </summary>
		FirstName,
		///<summary>Gender. </summary>
		Gender,
		///<summary>Guid. </summary>
		Guid,
		///<summary>Id. </summary>
		Id,
		///<summary>LastName. </summary>
		LastName,
		///<summary>Latitude. </summary>
		Latitude,
		///<summary>Longitude. </summary>
		Longitude,
		///<summary>MaxDistance. </summary>
		MaxDistance,
		///<summary>ModifiedOn. </summary>
		ModifiedOn,
		///<summary>Street. </summary>
		Street,
		///<summary>SubscribedOn. </summary>
		SubscribedOn,
		///<summary>Title. </summary>
		Title,
		///<summary>UserName. </summary>
		UserName,
		///<summary>UserPassword. </summary>
		UserPassword,
		///<summary>ZipCode. </summary>
		ZipCode,
		/// <summary></summary>
		AmountOfFields
	}
}

namespace TournamentCalendarDAL.RelationClasses
{
	/// <summary>Implements the relations factory for the entity: InfoService. </summary>
	public partial class InfoServiceRelations: RelationFactory
	{

		/// <summary>Returns a new IEntityRelation object, between InfoServiceEntity and CountryEntity over the m:1 relation they have, using the relation between the fields: InfoService.CountryId - Country.Id</summary>
		public virtual IEntityRelation CountryEntityUsingCountryId
		{
			get	{ return ModelInfoProviderSingleton.GetInstance().CreateRelation(RelationType.ManyToOne, "Country", false, new[] { CountryFields.Id, InfoServiceFields.CountryId }); }
		}

	}
	
	/// <summary>Static class which is used for providing relationship instances which are re-used internally for syncing</summary>
	internal static class StaticInfoServiceRelations
	{
		internal static readonly IEntityRelation CountryEntityUsingCountryIdStatic = new InfoServiceRelations().CountryEntityUsingCountryId;

		/// <summary>CTor</summary>
		static StaticInfoServiceRelations() { }
	}
}
