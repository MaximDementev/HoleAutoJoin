using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.Services;
using System;

namespace HoleAutoJoin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class JoinCommand : IExternalCommand
    {
        #region Fields
        private readonly IManualJoinService _manualJoinService;
        #endregion

        #region Constructors
        public JoinCommand()
        {
            // Получаем сервис из CommandFactory
            // Предполагается, что CommandFactory.Instance уже инициализирован
            // к моменту вызова этой команды Revit-ом.
            _manualJoinService = CommandFactory.Instance.ManualJoinService;
            if (_manualJoinService == null)
            {
                // Эта ошибка не должна возникать, если CommandFactory.Initialize вызван в OnStartup
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
                _manualJoinService.Raise();
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
