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
        private readonly ControlledApplication _application;
        private readonly IHoleFilter _holeFilter;
        private readonly ExternalEvent _externalEvent;
        private readonly HashSet<ElementId> _elementsToProcess = new HashSet<ElementId>();
        private Document _currentDocument;
        private bool _isEnabled;
        #endregion

        #region Properties
        public bool IsEnabled
        {
            get => _isEnabled;
            set => _isEnabled = value;
        }
        #endregion

        #region Constructor
        public AutoJoinEventHandler(ControlledApplication application, IHoleFilter holeFilter)
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
            if (!_isEnabled) return;

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
            catch (Exception ex)
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
