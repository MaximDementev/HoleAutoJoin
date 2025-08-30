using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using HoleAutoJoin; // Для доступа к CommandFactory
using HoleAutoJoin.Services; // Для доступа к IAutoJoinService, IManualJoinService
using HoleAutoJoin.UI; // Для доступа к HoleJoinSettingsForm

namespace HoleAutoJoin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SettingsCommand : IExternalCommand
    {
        #region Fields
        private IAutoJoinService _autoJoinService;
        private IManualJoinService _manualJoinService;
        #endregion

        #region IPlugin Implementation
        public bool IsEnabled { get; set; }

        public bool Initialize()
        {
            return true;
        }

        public void Shutdown()
        {
            CommandFactory.Instance.ShutdownServices();
        }
        #endregion

        #region IExternalCommand Implementation
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                if (commandData?.Application?.Application == null)
                {
                    message = "Не удалось получить доступ к приложению Revit.";
                    TaskDialog.Show("Ошибка", message);
                    return Result.Failed;
                }

                CommandFactory.Instance.EnsureServicesInitialized(commandData);

                _autoJoinService = CommandFactory.Instance.AutoJoinService;
                _manualJoinService = CommandFactory.Instance.ManualJoinService;

                if (_manualJoinService == null)
                {
                    message = "ManualJoinService не инициализирован.";
                    TaskDialog.Show("Settings Command", message);
                    return Result.Failed;
                }

                using (var form = new HoleJoinSettingsForm(_autoJoinService, _manualJoinService))
                {
                    form.ShowDialog();
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = $"Ошибка при открытии настроек: {ex.Message}";
                TaskDialog.Show( "Settings Command Error",
                    $"Произошла ошибка:\n{ex.Message}\n\nДетали:\n{ex.ToString()}");
                return Result.Failed;
            }
        }
        #endregion
    }
}
