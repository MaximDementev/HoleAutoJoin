using System;
using System.IO;
using System.Xml.Serialization;

namespace HoleAutoJoin.Core
{
    public class SettingsManager
    {
        #region Constants
        private const string SETTINGS_FOLDER = @"AppData\Roaming\MagicEntry\UserData\SettingsForApps";
        private const string SETTINGS_FILE = "HoleAutoJoinSettings.xml";
        #endregion

        #region Fields
        private static readonly Lazy<SettingsManager> _instance = new Lazy<SettingsManager>(() => new SettingsManager());
        private HoleJoinSettings _currentSettings;
        private readonly string _settingsPath;
        #endregion

        #region Properties
        public static SettingsManager Instance => _instance.Value;

        public HoleJoinSettings Settings
        {
            get
            {
                if (_currentSettings == null)
                {
                    LoadSettings();
                }
                return _currentSettings;
            }
        }
        #endregion

        #region Constructor
        private SettingsManager()
        {
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string settingsFolder = Path.Combine(userProfile, SETTINGS_FOLDER);
            _settingsPath = Path.Combine(settingsFolder, SETTINGS_FILE);

            // Создаем папку если она не существует
            Directory.CreateDirectory(settingsFolder);
        }
        #endregion

        #region Public Methods
        public void SaveSettings()
        {
            try
            {
                if (_currentSettings == null)
                    return;

                _currentSettings.LastModified = DateTime.Now;

                using (var writer = new StreamWriter(_settingsPath))
                {
                    var serializer = new XmlSerializer(typeof(HoleJoinSettings));
                    serializer.Serialize(writer, _currentSettings);
                }
            }
            catch (Exception ex)
            {
                // Логируем ошибку, но не прерываем работу
                System.Diagnostics.Debug.WriteLine($"[SettingsManager] Ошибка сохранения настроек: {ex.Message}");
            }
        }

        public void LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    using (var reader = new StreamReader(_settingsPath))
                    {
                        var serializer = new XmlSerializer(typeof(HoleJoinSettings));
                        _currentSettings = (HoleJoinSettings)serializer.Deserialize(reader);
                    }
                }
                else
                {
                    // Создаем настройки по умолчанию
                    _currentSettings = new HoleJoinSettings();
                    SaveSettings();
                }
            }
            catch (Exception ex)
            {
                // При ошибке загрузки создаем настройки по умолчанию
                System.Diagnostics.Debug.WriteLine($"[SettingsManager] Ошибка загрузки настроек: {ex.Message}");
                _currentSettings = new HoleJoinSettings();
                SaveSettings();
            }
        }

        public void ResetToDefaults()
        {
            _currentSettings = new HoleJoinSettings();
            SaveSettings();
        }
        #endregion
    }
}
