﻿using System;
using System.Collections.Generic;
using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.DatabaseSpecific;

namespace TournamentCalendar.Data;

/// <summary>
/// Provides database-specific data and methods.
/// </summary>
public class DbContext : IDbContext
{
    private readonly object _locker = new();

    public DbContext()
    {
        AppDb = new AppDb(this);
    }

    /// <summary>
    /// The connection key used to retrieve the <see cref="ConnectionString"/>.
    /// </summary>
    public virtual string ConnectionKey { get; set; } = string.Empty;

    /// <summary>
    /// The connection string for the database.
    /// </summary>
    public virtual string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The catalog aka database name.
    /// </summary>
    public virtual string Catalog { get; set; } = string.Empty;

    /// <summary>
    /// The schema inside the database.
    /// </summary>
    public virtual string Schema { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the timeout value to use with the command object(s) created by <see cref="IDataAccessAdapter"/>s.
    /// Default is 30 seconds
    /// </summary>
    /// <remarks>
    /// Set this prior to calling a method which executes database logic.
    /// </remarks>
    public virtual int CommandTimeOut { get; set; } = 30;

    /// <summary>
    /// Gets a new instance of an <see cref="IDataAccessAdapter"/> which will be used to access repositories.
    ///  </summary>
    /// <returns>Returns a new instance of an <see cref="IDataAccessAdapter"/> which will be used to access repositories.</returns>
    public virtual IDataAccessAdapter GetNewAdapter()
    {
        lock (_locker)
        {
            // see docs: https://www.llblgen.com/Documentation/5.9/LLBLGen%20Pro%20RTF/Using%20the%20generated%20code/gencode_resultsetcaching.htm
            // if connection string exists, the method simply returns without creating a new cache
            // ** Note ** The connection string must be EXACTLY as it's used for queries.
            CacheController.RegisterCache(ConnectionString, new ResultsetCache(TimeSpan.FromDays(1).Seconds));

            return new DataAccessAdapter(ConnectionString)
            {
                KeepConnectionOpen = true,
                CompatibilityLevel = SqlServerCompatibilityLevel.SqlServer2012,
                CommandTimeOut = CommandTimeOut,
                // As pointed out in https://www.llblgen.com/tinyforum/Thread/27471, Catalog and Schema
                // could also be overwritten with string.Empty. In this case, the default Schema
                // and the database from the connection string would be used.
                CatalogNameOverwrites =
                    new CatalogNameOverwriteHashtable(new Dictionary<string, string> { { "*", Catalog } })
                    {
                        CatalogNameUsageSetting = CatalogNameUsage.ForceName
                    },
                SchemaNameOverwrites =
                    new SchemaNameOverwriteHashtable(new Dictionary<string, string> { { "*", Schema } })
                    {
                        SchemaNameUsageSetting = SchemaNameUsage.ForceName
                    }
            };
        }
    }

    /// <summary>
    /// Gives access to the repositories.
    /// </summary>
    public virtual AppDb AppDb { get; }
}
