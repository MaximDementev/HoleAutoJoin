using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.UI;
using System;

namespace HoleAutoJoin.Commands
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class HelpCommand : IExternalCommand
    {
        #region IExternalCommand Implementation
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                using (var helpForm = new HelpForm())
                {
                    helpForm.ShowDialog();
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
