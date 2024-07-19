﻿//////////////////////////////////////////////////////////////
// <auto-generated>This code was generated by LLBLGen Pro 5.9.</auto-generated>
//////////////////////////////////////////////////////////////
// Code is generated on: 
// Code is generated using templates: SD.TemplateBindings.SharedTemplates
// Templates vendor: Solutions Design.
//////////////////////////////////////////////////////////////
using System;
using System.Linq;
using System.Collections.Generic;
using SD.LLBLGen.Pro.LinqSupportClasses;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.EntityClasses;
using TournamentCalendarDAL.FactoryClasses;

namespace TournamentCalendarDAL.Linq
{
	/// <summary>Meta-data class for the construction of Linq queries which are to be executed using LLBLGen Pro code.</summary>
	public partial class LinqMetaData: ILinqMetaData
	{
		/// <summary>CTor. Using this ctor will leave the IDataAccessAdapter object to use empty. To be able to execute the query, an IDataAccessAdapter instance
		/// is required, and has to be set on the LLBLGenProProvider2 object in the query to execute. </summary>
		public LinqMetaData() : this(null, null) { }
		
		/// <summary>CTor which accepts an IDataAccessAdapter implementing object, which will be used to execute queries created with this metadata class.</summary>
		/// <param name="adapterToUse">the IDataAccessAdapter to use in queries created with this meta data</param>
		/// <remarks> Be aware that the IDataAccessAdapter object set via this property is kept alive by the LLBLGenProQuery objects created with this meta data till they go out of scope.</remarks>
		public LinqMetaData(IDataAccessAdapter adapterToUse) : this (adapterToUse, null) { }

		/// <summary>CTor which accepts an IDataAccessAdapter implementing object, which will be used to execute queries created with this metadata class.</summary>
		/// <param name="adapterToUse">the IDataAccessAdapter to use in queries created with this meta data</param>
		/// <param name="customFunctionMappings">The custom function mappings to use. These take higher precedence than the ones in the DQE to use.</param>
		/// <remarks> Be aware that the IDataAccessAdapter object set via this property is kept alive by the LLBLGenProQuery objects created with this meta data till they go out of scope.</remarks>
		public LinqMetaData(IDataAccessAdapter adapterToUse, FunctionMappingStore customFunctionMappings)
		{
			this.AdapterToUse = adapterToUse;
			this.CustomFunctionMappings = customFunctionMappings;
		}
	
		/// <summary>returns the datasource to use in a Linq query for the entity type specified</summary>
		/// <param name="typeOfEntity">the type of the entity to get the datasource for</param>
		/// <returns>the requested datasource</returns>
		public IDataSource GetQueryableForEntity(int typeOfEntity)
		{
			switch((TournamentCalendarDAL.EntityType)typeOfEntity)
			{
				case TournamentCalendarDAL.EntityType.CalendarEntity:
					return this.Calendar;
				case TournamentCalendarDAL.EntityType.CountryEntity:
					return this.Country;
				case TournamentCalendarDAL.EntityType.InfoServiceEntity:
					return this.InfoService;
				case TournamentCalendarDAL.EntityType.PlayingAbilityEntity:
					return this.PlayingAbility;
				case TournamentCalendarDAL.EntityType.SentNewsletterEntity:
					return this.SentNewsletter;
				case TournamentCalendarDAL.EntityType.SurfaceEntity:
					return this.Surface;
				default:
					return null;
			}
		}

		/// <summary>returns the datasource to use in a Linq query which wraps the specified SQL query and projects it to instances of type T</summary>
		/// <param name="sqlQuery">The SQL query to execute. Has to follow one of the parameter specification patterns for Plain SQL queries in the LLBLGen Pro Runtime Framework</param>
		/// <param name="parameterValues">The object which will provide the parameter values for the SQL query specified. Has to follow one of the parameter specification patterns for Plain SQL queries in the LLBLGen Pro Runtime Framework</param>
		/// <typeparam name="T">The type of the instances to project the rows into, returned by the SQL query specified</typeparam>
		/// <returns>the requested datasource</returns>
		[InMemoryCandidate]
		public DataSource2<T> FromSql<T>(string sqlQuery, object parameterValues)
			where T : class
		{
			return new DataSource2<T>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse).SetWrappedPlainSQLQuerySpecification(sqlQuery, parameterValues);
		}

		/// <summary>returns the datasource to use in a Linq query for the entity type specified</summary>
		/// <typeparam name="TEntity">the type of the entity to get the datasource for</typeparam>
		/// <returns>the requested datasource</returns>
		public DataSource2<TEntity> GetQueryableForEntity<TEntity>()
				where TEntity : class
		{
			return new DataSource2<TEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse);
		}

		/// <summary>returns the datasource to use in a Linq query when targeting CalendarEntity instances in the database.</summary>
		public DataSource2<CalendarEntity> Calendar {	get { return new DataSource2<CalendarEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		
		/// <summary>returns the datasource to use in a Linq query when targeting CountryEntity instances in the database.</summary>
		public DataSource2<CountryEntity> Country {	get { return new DataSource2<CountryEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		
		/// <summary>returns the datasource to use in a Linq query when targeting InfoServiceEntity instances in the database.</summary>
		public DataSource2<InfoServiceEntity> InfoService {	get { return new DataSource2<InfoServiceEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		
		/// <summary>returns the datasource to use in a Linq query when targeting PlayingAbilityEntity instances in the database.</summary>
		public DataSource2<PlayingAbilityEntity> PlayingAbility {	get { return new DataSource2<PlayingAbilityEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		
		/// <summary>returns the datasource to use in a Linq query when targeting SentNewsletterEntity instances in the database.</summary>
		public DataSource2<SentNewsletterEntity> SentNewsletter {	get { return new DataSource2<SentNewsletterEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		
		/// <summary>returns the datasource to use in a Linq query when targeting SurfaceEntity instances in the database.</summary>
		public DataSource2<SurfaceEntity> Surface {	get { return new DataSource2<SurfaceEntity>(this.AdapterToUse, new ElementCreator(), this.CustomFunctionMappings, this.ContextToUse); } }
		


		/// <summary> Gets / sets the IDataAccessAdapter to use for the queries created with this meta data object.</summary>
		/// <remarks> Be aware that the IDataAccessAdapter object set via this property is kept alive by the LLBLGenProQuery objects created with this meta data
		/// till they go out of scope.</remarks>
		public IDataAccessAdapter AdapterToUse { get; set; }
		
		/// <summary>Gets or sets the custom function mappings to use. These take higher precedence than the ones in the DQE to use</summary>
		public FunctionMappingStore CustomFunctionMappings { get; set; }
		
		/// <summary>Gets or sets the Context instance to use for entity fetches.</summary>
		public Context ContextToUse { get; set; }
	}
}