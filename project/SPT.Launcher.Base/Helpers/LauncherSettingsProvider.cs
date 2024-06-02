/* LauncherSettingsProvider.cs
 * License: NCSA Open Source License
 * 
 * Copyright: SPT
 * AUTHORS:
 * waffle.lord
 */


using SPT.Launcher.MiniCommon;
using SPT.Launcher.Models.Launcher;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.IO;
using SPT.Launcher.Controllers;

namespace SPT.Launcher.Helpers
{
    public static class LauncherSettingsProvider
    {
        public static string DefaultSettingsFileLocation = Path.Join(Environment.CurrentDirectory, "user", "launcher", "config.json");
        public static Settings Instance { get; } = Json.Load<Settings>(DefaultSettingsFileLocation) ?? new Settings();
    }

    public class Settings : INotifyPropertyChanged
    {
        public bool FirstRun { get; set; } = true;

        public void SaveSettings()
        {
            Server.Url = Path.TrimEndingDirectorySeparator(Server.Url);
            Json.SaveWithFormatting(LauncherSettingsProvider.DefaultSettingsFileLocation, this, Formatting.Indented);
        }

        public void ResetDefaults()
        {
            string defaultUrl = "http://127.0.0.1:6969";
            string defaultPath = Environment.CurrentDirectory;
            
            // don't reset if running in dev mode
            if (IsDevMode)
            {
                LogManager.Instance.Info("Running in dev mode, not resetting");
                return;
            }

            if (Server != null && Server.Url != defaultUrl)
            {
                LogManager.Instance.Info($"Server URL was '{Server.Url}'");
                LogManager.Instance.Info($"Server URL was reset to default '{defaultUrl}'");
                Server.Url = defaultUrl;
            }
            else
            {
                LogManager.Instance.Info($"Server URL is already set to default '{defaultUrl}'");
            }

            if (GamePath != defaultPath)
            {
                LogManager.Instance.Info($"Game path was '{GamePath}'");
                LogManager.Instance.Info($"Game path was reset to default '{defaultPath}'");
                GamePath = defaultPath;
            }
            else
            {
                LogManager.Instance.Info($"Game path is already set to default '{defaultPath}'");
            }
            
            SaveSettings();
        }

        public string DefaultLocale { get; set; } = "English";

        private bool _IsAddingServer;
        [JsonIgnore]
        public bool IsAddingServer
        {
            get => _IsAddingServer;
            set
            {
                if (_IsAddingServer != value)
                {
                    _IsAddingServer = value;
                    RaisePropertyChanged(nameof(IsAddingServer));
                }
            }
        }

        private bool _AllowSettings;
        [JsonIgnore]
        public bool AllowSettings
        {
            get => _AllowSettings;
            set
            {
                if (_AllowSettings != value)
                {
                    _AllowSettings = value;
                    RaisePropertyChanged(nameof(AllowSettings));
                }
            }
        }

        private bool _GameRunning;
        [JsonIgnore]
        public bool GameRunning
        {
            get => _GameRunning;
            set
            {
                if (_GameRunning != value)
                {
                    _GameRunning = value;
                    RaisePropertyChanged(nameof(GameRunning));
                }
            }
        }

        private LauncherAction _LauncherStartGameAction;
        public LauncherAction LauncherStartGameAction
        {
            get => _LauncherStartGameAction;
            set
            {
                if (_LauncherStartGameAction != value)
                {
                    _LauncherStartGameAction = value;
                    RaisePropertyChanged(nameof(LauncherStartGameAction));
                }
            }
        }

        private bool _UseAutoLogin;
        public bool UseAutoLogin
        {
            get => _UseAutoLogin;
            set
            {
                if (_UseAutoLogin != value)
                {
                    _UseAutoLogin = value;
                    RaisePropertyChanged(nameof(UseAutoLogin));
                }
            }
        }

        private bool _IsDevMode;

        public bool IsDevMode
        {
            get => _IsDevMode;
            set
            {
                if (_IsDevMode != value)
                {
                    _IsDevMode = value;
                    RaisePropertyChanged(nameof(IsDevMode));
                }
            }
        }

        private string _GamePath;
        public string GamePath
        {
            get => _GamePath;
            set
            {
                if (_GamePath != value)
                {
                    _GamePath = value;
                    RaisePropertyChanged(nameof(GamePath));
                }
            }
        }

        private string[] _ExcludeFromCleanup;

        public string[] ExcludeFromCleanup
        {
            get => _ExcludeFromCleanup ??= Array.Empty<string>();
            set
            {
                if (_ExcludeFromCleanup != value)
                {
                    _ExcludeFromCleanup = value;
                    RaisePropertyChanged(nameof(ExcludeFromCleanup));
                }
            }
        }

        public ServerSetting Server { get; set; } = new ServerSetting();

        public Settings()
        {
            if (!File.Exists(LauncherSettingsProvider.DefaultSettingsFileLocation))
            {
                LogManager.Instance.Warning("Launcher config not found");
                LogManager.Instance.Info($"Creating launcher config: {LauncherSettingsProvider.DefaultSettingsFileLocation}");
                LauncherStartGameAction = LauncherAction.MinimizeAction;
                UseAutoLogin = true;
                GamePath = Environment.CurrentDirectory;
                IsDevMode = false;

                Server = new ServerSetting { Name = "SPT", Url = "http://127.0.0.1:6969" };
                SaveSettings();
            }
            
            LogManager.Instance.Info($"Using launcher config at: {LauncherSettingsProvider.DefaultSettingsFileLocation}");
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}
