using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;

[TransactionAttribute(TransactionMode.Manual)]
[RegenerationAttribute(RegenerationOption.Manual)]

public class JoinCommand_EventHandler : IExternalEventHandler
{
    #region Private Fields
    private List<FamilyInstance> _elementsToJoin = new List<FamilyInstance>();
    private ExternalEvent _externalEvent;
    #endregion

    #region Constructor
    public JoinCommand_EventHandler()
    {
        _externalEvent = ExternalEvent.Create(this);
    }


    public void Raise(List<FamilyInstance> elementsToJoin)
    {
        if (elementsToJoin.Count == 0) return;
        _elementsToJoin = elementsToJoin;

        _externalEvent.Raise();
    }

    #endregion
    //------------------------- Methods -----------------------------------------------------

    public void Execute(UIApplication app)
    {
        if (_elementsToJoin == null || !_elementsToJoin.Any()) return;

        Document doc = app.ActiveUIDocument?.Document;
        if (doc == null) return;

        try
        {
            using (Transaction trans = new Transaction(doc, $"Автоматическое соединение отверстий с плитой"))
            {
                trans.Start();

                foreach (FamilyInstance familyInstance in _elementsToJoin.Distinct())
                {
                    if (familyInstance?.Host != null && !JoinGeometryUtils.AreElementsJoined(doc, familyInstance, familyInstance.Host))
                    {
                        JoinGeometryUtils.JoinGeometry(doc, familyInstance, familyInstance.Host);
                    }
                }

                trans.Commit();
            }
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Ошибка", $"{ex.Message}");
        }
    }



    public string GetName()
    {
        return "JoinCommand_EventHandler";
    }
}