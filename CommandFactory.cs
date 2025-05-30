using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.Commands;
using HoleAutoJoin.Core;
using HoleAutoJoin.Services;
using System;

namespace HoleAutoJoin
{
    public class CommandFactory : IExternalCommandAvailability
    {
        #region Singleton
        private static CommandFactory _instance;
        public static CommandFactory Instance => _instance ?? (_instance = new CommandFactory());
        #endregion

        #region Fields
        private IHoleFilter _holeFilter;
        private IAutoJoinService _autoJoinService;
        private IManualJoinService _manualJoinService;
        #endregion

        #region Public Properties
        public IAutoJoinService AutoJoinService => _autoJoinService;
        public IManualJoinService ManualJoinService => _manualJoinService;
        #endregion

        #region Constructor
        private CommandFactory()
        {
        }
        #endregion

        #region Public Methods
        public void Initialize(UIControlledApplication application)
        {
            // Очистка предыдущих подписок, если они были
            _autoJoinService?.Shutdown();

            _holeFilter = new DefaultHoleFilter();
            _manualJoinService = new ManualJoinEventHandler(_holeFilter);
            _autoJoinService = new AutoJoinEventHandler(application.ControlledApplication, _holeFilter);
            _autoJoinService.Initialize();
        }

        #endregion

        #region IExternalCommandAvailability Implementation
        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return applicationData?.ActiveUIDocument?.Document != null;
        }
        #endregion
    }
}
