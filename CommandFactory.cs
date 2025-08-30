using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using HoleAutoJoin.Core;
using HoleAutoJoin.Services;
using System;
using System.Diagnostics;

namespace HoleAutoJoin
{
    public class CommandFactory
    {
        #region Singleton
        private static readonly Lazy<CommandFactory> _lazyInstance = new Lazy<CommandFactory>(() => new CommandFactory());
        public static CommandFactory Instance => _lazyInstance.Value;
        #endregion

        #region Fields
        private IHoleFilter _holeFilter;
        private IAutoJoinService _autoJoinService;
        private IManualJoinService _manualJoinService;
        private Application _application; // Store Application reference
        private ControlledApplication _controlledApplication; // Store ControlledApplication reference

        private bool _isCoreInitialized = false; // Флаг для базовых сервисов
        private bool _isAutoJoinServiceInitialized = false; // Флаг для AutoJoinService
        private bool _servicesShutdown = false;
        private static readonly object _initLock = new object();
        #endregion

        #region Public Properties
        public IAutoJoinService AutoJoinService
        {
            get
            {
                // Пытаемся инициализировать AutoJoinService при первом обращении, если еще не сделано
                // и если ControlledApplication может быть получен (например, если команда активна)
                // Однако, если EnsureServicesInitialized не был вызван с валидным commandData,
                // _autoJoinService может остаться null или не быть полностью инициализированным.
                EnsureCoreServicesInitialized(); // Убедимся, что базовые сервисы есть
                return _autoJoinService;
            }
        }
        public IManualJoinService ManualJoinService
        {
            get
            {
                EnsureCoreServicesInitialized();
                return _manualJoinService;
            }
        }
        #endregion

        #region Constructor
        private CommandFactory()
        {
            try
            {
                SettingsManager.Instance.LoadSettings();
            }
            catch (Exception)
            {
            }

            // Инициализация только тех сервисов, которые не зависят от Revit Application контекста
            _holeFilter = new DefaultHoleFilter();
            _manualJoinService = new ManualJoinEventHandler(_holeFilter);
            _isCoreInitialized = true;
        }
        #endregion

        #region Private Initialization
        private void EnsureCoreServicesInitialized()
        {
            if (_isCoreInitialized) return;
            lock (_initLock)
            {
                if (_isCoreInitialized) return;
                _holeFilter = new DefaultHoleFilter();
                _manualJoinService = new ManualJoinEventHandler(_holeFilter);
                _isCoreInitialized = true;
            }
        }
        #endregion

        #region Public Initialization and Shutdown
        public void InitializeOnStartup(ControlledApplication controlledApplication)
        {
            _controlledApplication = controlledApplication ?? throw new ArgumentNullException(nameof(controlledApplication));
            EnsureCoreServicesInitialized();
        }

        public void Initialize(Application application)
        {
            _application = application ?? throw new ArgumentNullException(nameof(application));

            EnsureCoreServicesInitialized();

            if (!_isAutoJoinServiceInitialized)
            {
                lock (_initLock)
                {
                    if (!_isAutoJoinServiceInitialized)
                    {
                        try
                        {
                            _autoJoinService = new AutoJoinEventHandler(_application, _holeFilter);
                            _autoJoinService.Initialize();
                        }
                        catch (Exception)
                        {
                        }
                        _isAutoJoinServiceInitialized = true;
                    }
                }
            }
        }

        public void EnsureServicesInitialized(ExternalCommandData commandData)
        {
            EnsureCoreServicesInitialized();

            if (_isAutoJoinServiceInitialized) return;

            lock (_initLock)
            {
                if (_isAutoJoinServiceInitialized) return;

                Application app = null;
                try
                {
                    if (commandData?.Application != null)
                    {
                        app = commandData.Application.Application;
                    }
                }
                catch (System.Exception)
                {
                    app = null;
                }

                if (app != null)
                {
                    if (_autoJoinService == null)
                    {
                        _autoJoinService = new AutoJoinEventHandler(app, _holeFilter);
                        _autoJoinService.Initialize();
                        Debug.WriteLine("[CommandFactory] AutoJoinService initialized successfully.");
                    }
                }
                else
                {
                    Debug.WriteLine("[CommandFactory] Application is not available. AutoJoinService cannot be initialized and will not function.");
                }
                _isAutoJoinServiceInitialized = true;
            }
        }

        public void ShutdownServices()
        {
            if (_servicesShutdown) return;
            lock (_initLock)
            {
                if (_servicesShutdown) return;

                try
                {
                    SettingsManager.Instance.SaveSettings();
                    Debug.WriteLine("[CommandFactory] Settings saved on shutdown.");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[CommandFactory] Error saving settings on shutdown: {ex.Message}");
                }

                _autoJoinService?.Shutdown();
                Debug.WriteLine("[CommandFactory] Services shut down.");

                _servicesShutdown = true;
                _isCoreInitialized = false; // Сброс флагов для возможной повторной инициализации (если применимо)
                _isAutoJoinServiceInitialized = false;
            }
        }
        #endregion
    }

    // Пример "пустышки", если нужен не-null AutoJoinService, который ничего не делает
    // public class NullAutoJoinService : IAutoJoinService
    // {
    //     public bool IsEnabled { get; set; }
    //     public void Initialize() { /* Do nothing */ }
    //     public void Shutdown() { /* Do nothing */ }
    // }
}
