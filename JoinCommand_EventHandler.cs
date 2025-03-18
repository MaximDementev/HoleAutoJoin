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
    private bool _isProcessing = false; // Флаг для предотвращения повторной обработки
    #endregion

    public int Count = 0;

    #region Constructor
    public JoinCommand_EventHandler()
    {
        _externalEvent = ExternalEvent.Create(this);
    }

    public void AddElements(List<FamilyInstance> elementsToJoin)
    {
        _elementsToJoin.AddRange(elementsToJoin);
    }


    public void Raise()
    {
        if (_isProcessing || _elementsToJoin.Count == 0) return;

        _isProcessing = true;
        _elementsToJoin = new HashSet<FamilyInstance>(_elementsToJoin).ToList();
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
            using (Transaction trans = new Transaction(doc, $"Соединение {_elementsToJoin.Count} отверстий"))
            {
                trans.Start();

                foreach (FamilyInstance familyInstance in _elementsToJoin.Distinct()) // Убираем дубликаты
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
            TaskDialog.Show("Error", $"Error during geometry join: {ex.Message}");
        }
        finally
        {
            _elementsToJoin.Clear();
            Count = 0;
            _isProcessing = false;
        }
    }



    public string GetName()
    {
        return "JoinCommand_EventHandler";
    }
}