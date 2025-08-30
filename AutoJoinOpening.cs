using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using HoleAutoJoin.Commands;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace HoleAutoJoin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class AutoJoinOpening : IExternalApplication
    {
        private const string TAB_NAME = "KRGPMagic";
        private const string PANEL_NAME = "Отверстия";

        public Result OnStartup(UIControlledApplication application)
        {
            try
            {
                CreateUserInterface(application);

                CommandFactory.Instance.InitializeOnStartup(application.ControlledApplication);

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AutoJoinOpening] Error during startup: {ex.Message}");
                return Result.Failed;
            }
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            try
            {
                CommandFactory.Instance.ShutdownServices();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[AutoJoinOpening] Error during shutdown: {ex.Message}");
                return Result.Failed;
            }
        }

        #region Private Methods
        private void CreateUserInterface(UIControlledApplication application)
        {
            string assemblyLocation = Assembly.GetExecutingAssembly().Location;
            string iconsDirectoryPath = Path.GetDirectoryName(assemblyLocation) + @"\icons\";

            // Создаем или находим вкладку KRGPMagic
            CreateOrFindTab(application, TAB_NAME);

            // Создаем или находим панель
            RibbonPanel panel = GetOrCreatePanel(application, TAB_NAME, PANEL_NAME);

            // Загрузка иконок
            var joinIcons = LoadIconPair(iconsDirectoryPath, "HoleAutoJoinIco");
            var settingsIcons = LoadIconPair(iconsDirectoryPath, "Settings");
            var helpIcons = LoadIconPair(iconsDirectoryPath, "Help");

            // Создаем данные для кнопки "Соединить отверстия"
            PushButtonData joinButtonData = new PushButtonData(
                "JoinCommand",
                "Соединить\nотверстия",
                assemblyLocation,
                typeof(JoinCommand).FullName)
            {
                ToolTip = "Соединяет все отверстия в плитах с их основами (ручной режим)",
                LargeImage = joinIcons.Large,
                Image = joinIcons.Small
            };

            // Создаем данные для кнопки "Настройки соединения"
            PushButtonData settingsButtonData = new PushButtonData(
                "SettingsCommand",
                "Настройки\nсоединения",
                assemblyLocation,
                typeof(SettingsCommand).FullName)
            {
                ToolTip = "Открыть настройки автоматического и ручного соединения отверстий",
                LargeImage = settingsIcons.Large,
                Image = settingsIcons.Small
            };

            // Создаем данные для кнопки "Справка по отверстиям"
            PushButtonData helpButtonData = new PushButtonData(
                "HelpCommand",
                "Справка по\nотверстиям",
                assemblyLocation,
                typeof(HelpCommand).FullName)
            {
                ToolTip = "Открыть справку с инструкциями по работе с отверстиями",
                LargeImage = helpIcons.Large,
                Image = helpIcons.Small
            };

            // Создаем SplitButton
            SplitButtonData splitButtonData = new SplitButtonData("splitButtonHoleJoin", "Соединение");
            SplitButton splitButton = panel.AddItem(splitButtonData) as SplitButton;

            if (splitButton != null)
            {
                // Добавляем кнопки в SplitButton
                splitButton.AddPushButton(joinButtonData);
                splitButton.AddPushButton(settingsButtonData);
                splitButton.AddPushButton(helpButtonData);

                // Устанавливаем "Соединить отверстия" как текущую кнопку
                splitButton.IsSynchronizedWithCurrentItem = true;
            }
        }

        private void CreateOrFindTab(UIControlledApplication application, string tabName)
        {
            try
            {
                application.CreateRibbonTab(tabName);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка создания вкладки", $"Не удалось создать вкладку {tabName}: {ex.Message}");
            }
        }

        private RibbonPanel GetOrCreatePanel(UIControlledApplication application, string tabName, string panelName)
        {
            try
            {
                // Ищем существующую панель
                var panels = application.GetRibbonPanels(tabName);
                var existingPanel = panels.FirstOrDefault(p => p.Name == panelName);

                if (existingPanel != null)
                    return existingPanel;

                // Создаем новую панель
                return application.CreateRibbonPanel(tabName, panelName);
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка создания панели", $"Не удалось создать панель {panelName}: {ex.Message}");
                // Возвращаем панель из вкладки Add-Ins как fallback
                return application.CreateRibbonPanel(Tab.AddIns, panelName);
            }
        }

        private IconPair LoadIconPair(string iconsPath, string baseName)
        {
            return new IconPair
            {
                Large = LoadIcon(Path.Combine(iconsPath, $"{baseName}.png")),
                Small = LoadIcon(Path.Combine(iconsPath, $"{baseName}_small.png")) ??
                       LoadIcon(Path.Combine(iconsPath, $"{baseName}.png")) // Fallback к большой иконке
            };
        }

        private BitmapImage LoadIcon(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    return new BitmapImage(new Uri(path));
                }
            }
            catch (Exception)
            {
                // Можно добавить логирование, если нужно
            }
            return null;
        }
        #endregion

        #region Helper Classes
        private class IconPair
        {
            public BitmapImage Large { get; set; }
            public BitmapImage Small { get; set; }
        }
        #endregion
    }
}
