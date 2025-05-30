using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.Core;
using System;

namespace HoleAutoJoin.Services
{
    public class ManualJoinEventHandler : IManualJoinService
    {
        #region Fields
        private readonly IHoleFilter _holeFilter;
        private ExternalEvent _externalEvent;
        #endregion

        #region Constructor
        public ManualJoinEventHandler(IHoleFilter holeFilter)
        {
            _holeFilter = holeFilter ?? throw new ArgumentNullException(nameof(holeFilter));
            _externalEvent = ExternalEvent.Create(this);
        }
        #endregion

        #region IManualJoinService Implementation
        public void Raise()
        {
            _externalEvent.Raise();
        }
        #endregion

        #region IExternalEventHandler Implementation
        public void Execute(UIApplication app)
        {
            Document doc = app.ActiveUIDocument?.Document;
            if (doc == null) return;

            var joiner = new HoleJoiner(doc, _holeFilter);
            var result = joiner.JoinHolesToStructure();

            TaskDialog.Show("Результат", result.Message);
        }

        public string GetName()
        {
            return "ManualJoinEventHandler";
        }
        #endregion 
    }
}
