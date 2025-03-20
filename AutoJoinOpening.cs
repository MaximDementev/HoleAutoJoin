using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using System.Linq;
using System.IO;
using System.Reflection;
using System;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;

public class AutoJoinOpening : IExternalApplication
{
    private JoinCommand_EventHandler _joinCommand_EventHandler;
    private JoinCommand _joinCommand;
    public static bool IsDelegateAdded;
    private UIControlledApplication _controlledApplication;

    public Result OnStartup(UIControlledApplication application)
    {
        _controlledApplication = application;
        _joinCommand_EventHandler = new JoinCommand_EventHandler();
        _joinCommand = new JoinCommand();
        JoinCommand.Init(_joinCommand_EventHandler);
        application.ControlledApplication.DocumentChanged += OnDocumentChanged;
        IsDelegateAdded = true;


        //-----------------------------------------
        string assemblyLocation = Assembly.GetExecutingAssembly().Location,iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

        string panelName = "Проемы и отверстия";
        string ribbonName = "Соединить отверстия с плитой";

        #region 1. SPECIFIC
        {
            IList<RibbonPanel> panels = application.GetRibbonPanels(Tab.AddIns);
            RibbonPanel panel = panels.FirstOrDefault(p => p.Name == panelName);
            if (panel == null)panel = application.CreateRibbonPanel(Tab.AddIns, panelName);

            panel.AddItem(new PushButtonData(nameof(JoinCommand), ribbonName, assemblyLocation, typeof(JoinCommand).FullName)
            {
                LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "HoleAutoJoinIco.png")),
                LongDescription = "Соединение отверстий выполняется автоматически. Запускать при необходимости. " +
                "Соединяет все отверстия в плитах с их основами. Перед запуском рекомендуется синхронизироваться"
            });
        }
        #endregion

        string ribbonName2 = "Автосоединение";

        #region 1. SPECIFIC
        {
            IList<RibbonPanel> panels = application.GetRibbonPanels(Tab.AddIns);
            RibbonPanel panel = panels.FirstOrDefault(p => p.Name == panelName);
            if (panel == null) panel = application.CreateRibbonPanel(Tab.AddIns, panelName);

            panel.AddItem(new PushButtonData(nameof(OpenForm), ribbonName2, assemblyLocation, typeof(OpenForm).FullName)
            {
                //LargeImage = new BitmapImage(new Uri(iconsDirectoryPath + "HoleAutoJoinIco.png")),
                //LongDescription = "Соединение отверстий выполняется автоматически. Запускать при необходимости. " +
                //"Соединяет все отверстия в плитах с их основами. Перед запуском рекомендуется синхронизироваться"
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

    private  void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
        try
        {
            Document doc = e.GetDocument();
            if (doc == null) return;

            var changedOrAddedElements = new FilteredElementCollector(doc)
                .OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>()
                .Where(fi => fi.Symbol.Family.Name.StartsWith("Отверстие_Плита_"))
                .ToList();

            if (changedOrAddedElements.Any())
                _joinCommand_EventHandler.Raise(changedOrAddedElements);
        }
        catch (Exception ex)
        {
            TaskDialog.Show("Ошибка", $"{ex.Message}");
        }
    }

    public void OnOrOff(bool addDelegate)
    {
        if (addDelegate)
        {
            _controlledApplication.ControlledApplication.DocumentChanged += OnDocumentChanged;
            IsDelegateAdded = true;
        }
        else
        {
            _controlledApplication.ControlledApplication.DocumentChanged -= OnDocumentChanged;
            IsDelegateAdded = false;
        }

    }


}
