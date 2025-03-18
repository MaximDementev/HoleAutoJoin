using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;

[TransactionAttribute(TransactionMode.Manual)]
[RegenerationAttribute(RegenerationOption.Manual)]

public class JoinCommand_EventHandler : IExternalEventHandler
{
    #region Private Fields
    private List<FamilyInstance> _addedElements = new List<FamilyInstance>();
    private ExternalEvent _externalEvent;
    #endregion

    #region Constructor
    public JoinCommand_EventHandler()
    {
        Initialize();
    }

    public void Initialize()
    {
        _externalEvent = ExternalEvent.Create(this);
    }

    public void Raise(List<FamilyInstance> addedElements)
    {
        _addedElements = addedElements;
        _externalEvent.Raise();
    }
    #endregion
    //------------------------- Methods -----------------------------------------------------

    public void Execute(UIApplication app)
    {
        if (_addedElements == null || !_addedElements.Any()) return;

        Document doc = app.ActiveUIDocument?.Document;
        if (doc == null) return;

        using (Transaction trans = new Transaction(doc, $"Соединение с основой {_addedElements.Count} отверстий"))
        {
            trans.Start();

            foreach (FamilyInstance familyInstance in _addedElements)
            {
                Element host = familyInstance.Host;

                if (host != null && !JoinGeometryUtils.AreElementsJoined(doc, familyInstance, host))
                {
                    JoinGeometryUtils.JoinGeometry(doc, familyInstance, host);
                }
            }

            trans.Commit();
        }
    }


    public string GetName()
    {
        return "JoinCommand_EventHandler";
    }
}