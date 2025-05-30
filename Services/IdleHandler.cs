using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System;
using System.Collections.Generic;

namespace HoleAutoJoin.Services
{
    public class IdleHandler
    {
        #region Singleton
        private static IdleHandler _instance;
        public static IdleHandler Instance => _instance ?? (_instance = new IdleHandler());
        #endregion

        #region Fields
        private readonly Queue<Action> _actions = new Queue<Action>();
        private UIApplication _uiApplication;
        private bool _isRegistered;
        #endregion

        #region Constructor
        private IdleHandler()
        {
        }
        #endregion

        #region Public Methods
        public void Initialize(UIApplication uiApplication)
        {
            _uiApplication = uiApplication ?? throw new ArgumentNullException(nameof(uiApplication));
        }

        public void Register(Action action)
        {
            if (action == null || _uiApplication == null) return;

            _actions.Enqueue(action);

            if (!_isRegistered)
            {
                _isRegistered = true;
                _uiApplication.Idling += OnIdling;
            }
        }
        #endregion

        #region Private Methods
        private void OnIdling(object sender, IdlingEventArgs e)
        {
            if (_actions.Count == 0)
            {
                _uiApplication.Idling -= OnIdling;
                _isRegistered = false;
                return;
            }

            Action action = _actions.Dequeue();
            action?.Invoke();
        }
        #endregion
    }
}
