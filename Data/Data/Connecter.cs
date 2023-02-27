using SD.LLBLGen.Pro.ORMSupportClasses;
using TournamentCalendarDAL.DatabaseSpecific;

namespace TournamentCalendar.Data;

public static class Connecter
{
    public const string DefaultConnection = "DefaultConnection";

    static Connecter()
    {}

    /// <summary>
    /// Creates a new instance of <see cref="DataAccessAdapter"/> with ConnectionString key "Tournament"
    /// </summary>
    /// <returns>Returns a new instance of <see cref="DataAccessAdapter"/> with ConnectionString key "Tournament"</returns>
    public static DataAccessAdapter GetNewAdapter()
    {
        var adapter = new DataAccessAdapter(RuntimeConfiguration.GetConnectionString(DefaultConnection)) { KeepConnectionOpen = true };
        return adapter;
    }

    /// <summary>
    /// Creates a new instance of <see cref="DataAccessAdapter"/>
    /// </summary>
    /// <param name="dbConnectionString">The ConnectionString to use for creating the <see cref="DataAccessAdapter"/></param>
    /// <returns>Returns a new instance of <see cref="DataAccessAdapter"/></returns>
    public static DataAccessAdapter GetNewAdapter(string dbConnectionString)
    {
        var adapter = new DataAccessAdapter(dbConnectionString) { KeepConnectionOpen = true };
        return adapter;
    }
}