using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System.IO;
using System.Reflection;
using System;

public class AutoJoinOpening : IExternalApplication
{
    private JoinCommand_EventHandler _joinCommand_EventHandler;
    private JoinCommand _joinCommand;

    public Result OnStartup(UIControlledApplication application)
    {
        _joinCommand_EventHandler = new JoinCommand_EventHandler();
        _joinCommand = new JoinCommand();
        JoinCommand.Init(_joinCommand_EventHandler);
        application.ControlledApplication.DocumentChanged += OnDocumentChanged;


        //-----------------------------------------
        string assemblyLocation = Assembly.GetExecutingAssembly().Location,iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

        string tabName = "KRGP";
        string panelName = "PANELNAME";
        string ribbonName = "BUTTONNAME";


        try
        {
            application.CreateRibbonTab(tabName);
        }
        catch { }

        #region 1. SPECIFIC
        {
            RibbonPanel panel = application.GetRibbonPanels(tabName).Where(p => p.Name == panelName).FirstOrDefault();
            if (panel == null)
            {
                panel = application.CreateRibbonPanel(tabName, panelName);
            }

            panel.AddItem(new PushButtonData(nameof(JoinCommand), ribbonName, assemblyLocation, typeof(JoinCommand).FullName)
            {
                //LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "IMAGENAME.png"))
            });
        }
        #endregion

        return Result.Succeeded;
    }

    public Result OnShutdown(UIControlledApplication application)
    {
        application.ControlledApplication.DocumentChanged -= OnDocumentChanged;
        return Result.Succeeded;
    }

    private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
        try
        {
            Document doc = e.GetDocument();
            if (doc == null) return;

            var changedOrAddedElements = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>()
                .Where(fi => fi.Symbol.Family.Name.StartsWith("Отверстие_"))
                .ToList();

            if (changedOrAddedElements.Any())
            {
                _joinCommand_EventHandler.AddElements(changedOrAddedElements);

                if (_joinCommand_EventHandler.Count < 10)
                {
                    _joinCommand_EventHandler.Count++;
                    return;
                }
                _joinCommand_EventHandler.Raise();
            }
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Error", $"Error processing document change: {ex.Message}");
        }
    }

}
