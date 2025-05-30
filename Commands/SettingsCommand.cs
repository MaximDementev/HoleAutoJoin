using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.Services;
using HoleAutoJoin.UI;
using System;

namespace HoleAutoJoin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class SettingsCommand : IExternalCommand
    {
        #region Fields
        private readonly IAutoJoinService _autoJoinService;
        private readonly IManualJoinService _manualJoinService;
        #endregion

        #region Constructors
        public SettingsCommand()
        {
            _autoJoinService = CommandFactory.Instance.AutoJoinService;
            _manualJoinService = CommandFactory.Instance.ManualJoinService;

            if (_autoJoinService == null)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Ошибка инициализации", $"{nameof(IAutoJoinService)} не доступен.");
                throw new InvalidOperationException($"{nameof(IAutoJoinService)} не был инициализирован в {nameof(CommandFactory)}.");
            }
            if (_manualJoinService == null)
            {
                Autodesk.Revit.UI.TaskDialog.Show("Ошибка инициализации", $"{nameof(IManualJoinService)} не доступен.");
                throw new InvalidOperationException($"{nameof(IManualJoinService)} не был инициализирован в {nameof(CommandFactory)}.");
            }
        }
        #endregion

        #region IExternalCommand Implementation
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                using (var form = new HoleJoinSettingsForm(_autoJoinService, _manualJoinService))
                {
                    form.ShowDialog();
                }
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
        #endregion
    }
}
