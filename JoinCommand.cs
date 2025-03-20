using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Linq;


[TransactionAttribute(TransactionMode.Manual)]
[RegenerationAttribute(RegenerationOption.Manual)]

public class JoinCommand : IExternalCommand
{
    private static JoinCommand_EventHandler _joinCommand_EventHandler;
    public static void Init(JoinCommand_EventHandler joinCommand_EventHandler)
    {
        _joinCommand_EventHandler = joinCommand_EventHandler;
    }

    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        Document doc = commandData.Application.ActiveUIDocument.Document;
        var changedOrAddedElements = new FilteredElementCollector(doc)
            .OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>()
            .Where(fi => fi.Symbol.Family.Name.StartsWith("Отверстие_Плита_"))
            .ToList();

        if (_joinCommand_EventHandler ==  null) _joinCommand_EventHandler = new JoinCommand_EventHandler();
        _joinCommand_EventHandler.Raise(changedOrAddedElements);

        return Result.Succeeded;
    }
}