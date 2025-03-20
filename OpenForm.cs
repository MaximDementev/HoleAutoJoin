using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;


[TransactionAttribute(TransactionMode.Manual)]
[RegenerationAttribute(RegenerationOption.Manual)]

public class OpenForm : IExternalCommand
{
    private OnOff onOff;

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        if (onOff  == null)
            onOff = new OnOff();

        onOff.ShowDialog();

        return Result.Succeeded;
    }
}