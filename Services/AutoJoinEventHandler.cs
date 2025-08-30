using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Events;
using Autodesk.Revit.UI;
using HoleAutoJoin.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoleAutoJoin.Services
{
    public class AutoJoinEventHandler : IAutoJoinService, IExternalEventHandler
    {
        #region Fields
        private readonly Application _application;
        private readonly IHoleFilter _holeFilter;
        private readonly ExternalEvent _externalEvent;
        private readonly HashSet<ElementId> _elementsToProcess = new HashSet<ElementId>();
        private Document _currentDocument;
        #endregion

        #region Properties
        public bool IsEnabled
        {
            get => SettingsManager.Instance.Settings.IsAutoJoinEnabled;
            set
            {
                SettingsManager.Instance.Settings.IsAutoJoinEnabled = value;
                SettingsManager.Instance.SaveSettings();
            }
        }
        #endregion

        #region Constructor
        public AutoJoinEventHandler(Application application, IHoleFilter holeFilter)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));
            _holeFilter = holeFilter ?? throw new ArgumentNullException(nameof(holeFilter));
            _externalEvent = ExternalEvent.Create(this);
        }
        #endregion

        #region IAutoJoinService Implementation
        public void Initialize()
        {
            _application.DocumentChanged += OnDocumentChanged;
        }

        public void Shutdown()
        {
            _application.DocumentChanged -= OnDocumentChanged;
        }
        #endregion

        #region Event Handlers
        private void OnDocumentChanged(object sender, DocumentChangedEventArgs e)
        {
            if (!SettingsManager.Instance.Settings.IsAutoJoinEnabled) return;

            try
            {
                _currentDocument = e.GetDocument();
                var addedElements = e.GetAddedElementIds();
                var modifiedElements = e.GetModifiedElementIds();

                bool hasHolesToProcess = false;

                // Проверяем добавленные элементы
                foreach (var elementId in addedElements)
                {
                    var element = _currentDocument.GetElement(elementId);
                    if (element is FamilyInstance familyInstance && _holeFilter.IsValidHole(familyInstance))
                    {
                        _elementsToProcess.Add(elementId);
                        hasHolesToProcess = true;
                    }
                }

                // Проверяем измененные элементы
                foreach (var elementId in modifiedElements)
                {
                    var element = _currentDocument.GetElement(elementId);
                    if (element is FamilyInstance familyInstance && _holeFilter.IsValidHole(familyInstance))
                    {
                        _elementsToProcess.Add(elementId);
                        hasHolesToProcess = true;
                    }
                }

                // Если есть элементы для обработки, запускаем внешнее событие
                if (hasHolesToProcess)
                {
                    _externalEvent.Raise();
                }
            }
            catch (Exception)
            {
                // Игнорируем ошибки в обработчике события
            }
        }
        #endregion

        #region IExternalEventHandler Implementation
        public void Execute(UIApplication app)
        {
            if (_currentDocument == null || !_elementsToProcess.Any())
                return;

            try
            {
                using (Transaction trans = new Transaction(_currentDocument, "Автоматическое соединение отверстий"))
                {
                    trans.Start();

                    var elementsToJoin = new List<ElementId>(_elementsToProcess);
                    _elementsToProcess.Clear();

                    foreach (var elementId in elementsToJoin)
                    {
                        var element = _currentDocument.GetElement(elementId);
                        if (element is FamilyInstance familyInstance && _holeFilter.IsValidHole(familyInstance))
                        {
                            JoinHoleWithHost(familyInstance);
                        }
                    }

                    trans.Commit();
                }
            }
            catch
            {
                // Игнорируем ошибки транзакции
            }
        }

        public string GetName()
        {
            return "AutoJoinEventHandler";
        }
        #endregion

        #region Private Methods
        private void JoinHoleWithHost(FamilyInstance familyInstance)
        {
            if (familyInstance?.Host == null || !familyInstance.IsValidObject)
                return;

            try
            {
                if (!JoinGeometryUtils.AreElementsJoined(_currentDocument, familyInstance, familyInstance.Host))
                {
                    JoinGeometryUtils.JoinGeometry(_currentDocument, familyInstance, familyInstance.Host);

                    if (SettingsManager.Instance.Settings.ShowNotifications)
                    {
                        System.Diagnostics.Debug.WriteLine($"[AutoJoin] Соединено отверстие {familyInstance.Id} с элементом {familyInstance.Host.Id}");
                    }
                }
            }
            catch
            {
                // Игнорируем ошибки соединения отдельных элементов
            }
        }
        #endregion
    }
}
