using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;

namespace TournamentCalendar.Models.InfoService;

public class UnsubscribeModel
{
    private readonly IAppDb _appDb;

    public UnsubscribeModel(IAppDb appDb, string guid)
    {
        _appDb = appDb;
        Guid = guid;
        SaveSuccessFul = IsEntityFound = false;
    }

    public string Guid { get; private set; }
    public bool SaveSuccessFul { get; set; }
    public bool IsEntityFound { get; set; }
    public Exception? Exception { get; set; }
    public InfoServiceEntity? Entity { get; set; }

    public static bool IsGuid(string guid)
    {
        return System.Guid.TryParse(guid, out _);
    }

    public async Task<UnsubscribeModel> Delete(CancellationToken cancellationToken)
    {
        SaveSuccessFul = false;
        Entity = new InfoServiceEntity();

        try
        {
            if (!(IsEntityFound = _appDb.InfoServiceRepository.GetRegistrationByGuid(Entity, Guid)))
                return this;

            Entity.ModifiedOn = DateTime.Now;
            Entity.ConfirmedOn = null;
            SaveSuccessFul = await _appDb.GenericRepository.Delete(Entity, cancellationToken);
        }
        catch (Exception ex)
        {
            Exception = ex;
        }

        return this;
    }
}
