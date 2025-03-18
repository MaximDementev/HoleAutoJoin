using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;

public class AutoJoinOpening : IExternalApplication
{
    private JoinCommand_EventHandler _joinCommand;

    public Result OnStartup(UIControlledApplication application)
    {
        _joinCommand = new JoinCommand_EventHandler();

        application.ControlledApplication.DocumentChanged += OnDocumentChanged;
        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        application.ControlledApplication.DocumentChanged -= OnDocumentChanged;
        return Result.Succeeded;
    }

    private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
        Document doc = e.GetDocument();
        if (doc == null) return;

        var changedOrAddedElements = e.GetAddedElementIds()
        .Concat(e.GetModifiedElementIds())
        .Select(id => doc.GetElement(id))
        .OfType<FamilyInstance>()
        .Where(fi => fi.Symbol.Family.Name.StartsWith("Отверстие_Плита_"))
        .ToList();

        if (changedOrAddedElements.Any()) _joinCommand.Raise(changedOrAddedElements);
    }
}
