using Autodesk.Revit.UI;

namespace HoleAutoJoin.Services
{
    public interface IManualJoinService : IExternalEventHandler
    {
        void Raise();
    }
}
