using System;
using System.Threading;
using System.Threading.Tasks;
using TournamentCalendar.Data;
using TournamentCalendarDAL.EntityClasses;

namespace TournamentCalendar.Models.InfoService;

public class UnsubscribeModel
{
    public UnsubscribeModel(string guid)
    {
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

    public async Task<UnsubscribeModel> Save(CancellationToken cancellationToken)
    {
        SaveSuccessFul = false;
        Entity = new InfoServiceEntity();

        try
        {
            if (!(IsEntityFound = InfoServiceRepository.GetRegistrationByGuid(Entity, Guid)))
                return this;

            // only save dates if not already unsubscribed
            if (!Entity.UnSubscribedOn.HasValue)
            {
                Entity.UnSubscribedOn = Entity.ModifiedOn = DateTime.Now;
                Entity.ConfirmedOn = null;
                SaveSuccessFul = await GenericRepository.Save(Entity, true, cancellationToken);
            }
            else
            {
                SaveSuccessFul = true;
            }
        }
        catch (Exception ex)
        {
            Exception = ex;
        }
        return this;
    }
}