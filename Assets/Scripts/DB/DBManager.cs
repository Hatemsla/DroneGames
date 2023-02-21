using System;
using Menu;
using Npgsql;
using Unity.VisualScripting;
using UnityEngine;

namespace DB
{
    public class DBManager : MonoBehaviour
    {
        public MenuManager menuManager;

        private readonly string _connectionString =
            "Host=192.168.1.130;Port=5432;Username=postgres;Password=Bobik123654;Database=drones";

        public UserColors BotsColor = new UserColors();
        public UserColors PlayerColor = new UserColors();
        public UserData UserData = new UserData();
        public UserDifficultlyLevels UserDifficultlyLevels = new UserDifficultlyLevels();
        public UserResolutions UserResolutions = new UserResolutions();
        public UserSettings UserSettings = new UserSettings();

        private void Start()
        {
            menuManager = GetComponent<MenuManager>();
            DontDestroyOnLoad(this);
        }

        public void LoadSettings()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
            }
        }

        public void SaveUserResolution()
        {
            UserResolutions.Width = menuManager.Resolutions[menuManager.currentResolutionIndex].width;
            UserResolutions.Height = menuManager.Resolutions[menuManager.currentResolutionIndex].height;
            UserResolutions.FrameRate = menuManager.Resolutions[menuManager.currentResolutionIndex].refreshRate;
            if (IsResolutionExist(UserResolutions.Width, UserResolutions.Height, UserResolutions.FrameRate))
            {
                var resId = 1;
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var selectId =
                        $"select resolution_id from resolutions where width = {UserResolutions.Width} and height = {UserResolutions.Height} and refresh_rate = {UserResolutions.FrameRate}";
                    var cmd = new NpgsqlCommand(selectId, conn);
                    var dr = cmd.ExecuteReader();
                    while (dr.Read()) resId = dr.GetInt32(0);
                }

                UserSettings.ResolutionId = resId;
            }
            else
            {
                var newId = SelectNewId("resolution_id", "resolutions");
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var selectId =
                        $"insert into resolutions values({newId}, {UserResolutions.Width}, {UserResolutions.Height}, {UserResolutions.FrameRate})";
                    var cmd = new NpgsqlCommand(selectId, conn);
                    cmd.ExecuteNonQuery();
                }

                UserSettings.ResolutionId = newId;
            }
        }

        public void SaveUserSettings()
        {
            PlayerColor =
                new UserColors(
                    SelectIdWhere("color_id", "colors", "color",
                        $"{menuManager.playerColorPreview.color.ToHexString() + "FF"}"),
                    menuManager.playerColorPreview.color.ToHexString() + "FF");
            BotsColor = new UserColors(
                SelectIdWhere("color_id", "colors", "color",
                    $"{menuManager.botsColorPreview.color.ToHexString() + "FF"}"),
                menuManager.botsColorPreview.color.ToHexString() + "FF");

            UserSettings.IsFullscreen = menuManager.menuUIManager.isFullscreenToggle.isOn;
            UserSettings.SoundLevel = menuManager.currentVolume;
            UserSettings.YawRotationSensitivity = menuManager.currentYawSensitivity;
            UserSettings.BotsColorId = BotsColor.ColorId;
            UserSettings.PlayerColorId = PlayerColor.ColorId;
            UserSettings.DifficultId = menuManager.currentDifficultIndex + 1;
            if (IsSettingsExist())
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var updateString =
                        $"update settings set is_fullscreen = {UserSettings.IsFullscreen}, sound_level = {UserSettings.SoundLevel.ToString().Replace(',', '.')}, " +
                        $"rotation_sensitivity = {UserSettings.YawRotationSensitivity.ToString().Replace(',', '.')}, resolution_id = {UserSettings.ResolutionId}, " +
                        $"bots_color_id = {UserSettings.BotsColorId}, player_color_id = {UserSettings.PlayerColorId}, difficult_id = {UserSettings.DifficultId} " +
                        $"where settings_id = {UserSettings.SettingsId}";
                    var cmd = new NpgsqlCommand(updateString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
            else
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var insertString =
                        $"insert into settings values ({UserSettings.SettingsId}, {UserSettings.IsFullscreen}, {UserSettings.SoundLevel.ToString().Replace(',', '.')}, {UserSettings.YawRotationSensitivity.ToString().Replace(',', '.')}, {UserSettings.ResolutionId}, {UserSettings.BotsColorId}, {UserSettings.PlayerColorId}, {UserSettings.DifficultId})";
                    var cmd = new NpgsqlCommand(insertString, conn);
                    cmd.ExecuteNonQuery();
                }

                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var insertString =
                        $"update users set settings_id = {UserSettings.SettingsId} where login = '{UserData.UserLogin}'";
                    var cmd = new NpgsqlCommand(insertString, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void SaveUserData()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var saveString =
                    $"update users set seconds_in_game = {(int) UserData.SecondsInGame} where login = '{UserData.UserLogin}'";
                var cmd = new NpgsqlCommand(saveString, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void Registration()
        {
            UserDifficultlyLevels = new UserDifficultlyLevels(menuManager.currentDifficultIndex + 1,
                menuManager.menuUIManager.difficultDropdown.options[menuManager.currentDifficultIndex].text);
            if (IsResolutionExist(menuManager.Resolutions[menuManager.currentResolutionIndex].width,
                menuManager.Resolutions[menuManager.currentResolutionIndex].height,
                menuManager.Resolutions[menuManager.currentResolutionIndex].refreshRate))
            {
                var resId = SelectIdWhere("resolution_id", "resolutions", "width", "height", "refresh_rate",
                    menuManager.Resolutions[menuManager.currentResolutionIndex].width.ToString(),
                    menuManager.Resolutions[menuManager.currentResolutionIndex].height.ToString(),
                    menuManager.Resolutions[menuManager.currentResolutionIndex].refreshRate.ToString());
                UserResolutions = new UserResolutions(resId,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].width,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].height,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].refreshRate);
            }
            else
            {
                var resId = SelectNewId("resolution_id", "resolutions");
                UserResolutions = new UserResolutions(resId,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].width,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].height,
                    menuManager.Resolutions[menuManager.currentResolutionIndex].refreshRate);
            }

            PlayerColor =
                new UserColors(
                    SelectIdWhere("color_id", "colors", "color",
                        $"{menuManager.playerColorPreview.color.ToHexString() + "FF"}"),
                    menuManager.playerColorPreview.color.ToHexString() + "FF");
            BotsColor = new UserColors(
                SelectIdWhere("color_id", "colors", "color",
                    $"{menuManager.botsColorPreview.color.ToHexString() + "FF"}"),
                menuManager.botsColorPreview.color.ToHexString() + "FF");
            UserSettings = new UserSettings(SelectNewId("settings_id", "settings"),
                menuManager.menuUIManager.isFullscreenToggle.isOn,
                menuManager.currentVolume,
                menuManager.currentYawSensitivity,
                UserResolutions.ResolutionId,
                BotsColor.ColorId,
                PlayerColor.ColorId,
                UserDifficultlyLevels.DifficultId);
            UserData = new UserData(menuManager.menuUIManager.regLoginInput.text,
                menuManager.menuUIManager.regPasswordInput.text, 0, UserSettings.SettingsId);

            if (!IsUserExist(true))
            {
                using (var conn = new NpgsqlConnection(_connectionString))
                {
                    conn.Open();
                    var reg = $"insert into users values('{UserData.UserLogin}', '{UserData.UserPassword}')";
                    var cmd = new NpgsqlCommand(reg, conn);
                    cmd.ExecuteNonQuery();
                    menuManager.OpenMenu("Start");
                }

                menuManager.menuUIManager.regLoginInput.text = string.Empty;
                menuManager.menuUIManager.regPasswordInput.text = string.Empty;
            }
        }

        public void Login()
        {
            UserData = new UserData(menuManager.menuUIManager.logLoginInput.text,
                menuManager.menuUIManager.logPasswordInput.text);

            if (IsUserExist(false))
            {
                var userData = SelectUserData(UserData.UserLogin, UserData.UserPassword);
                UserData.SecondsInGame = Convert.ToSingle(userData[2]);
                UserData.SettingsId = Convert.ToInt32(userData[3]);

                var userSettings = SelectUserSettings(UserData.SettingsId.ToString());
                UserSettings = new UserSettings(Convert.ToInt32(userSettings[0]), Convert.ToBoolean(userSettings[1]),
                    Convert.ToSingle(userSettings[2]), Convert.ToSingle(userSettings[3]),
                    Convert.ToInt32(userSettings[4]), Convert.ToInt32(userSettings[5]),
                    Convert.ToInt32(userSettings[6]), Convert.ToInt32(userSettings[7]));

                var userResolution = SelectUserResolution(UserSettings.ResolutionId.ToString());
                UserResolutions = new UserResolutions(Convert.ToInt32(userResolution[0]),
                    Convert.ToInt32(userResolution[1]), Convert.ToInt32(userResolution[2]),
                    Convert.ToInt32(userResolution[3]));

                var playerColor = SelectUserColor(UserSettings.PlayerColorId.ToString());
                PlayerColor = new UserColors(Convert.ToInt32(playerColor[0]), playerColor[1]);
                var botsColor = SelectUserColor(UserSettings.BotsColorId.ToString());
                BotsColor = new UserColors(Convert.ToInt32(botsColor[0]), botsColor[1]);

                var userDifficult = SelectUserDifficult(UserSettings.DifficultId.ToString());
                UserDifficultlyLevels = new UserDifficultlyLevels(Convert.ToInt32(userDifficult[0]), userDifficult[1]);

                menuManager.OpenMenu("Start");
                menuManager.menuUIManager.logLoginInput.text = string.Empty;
                menuManager.menuUIManager.logPasswordInput.text = string.Empty;
            }
        }
        
        private string[] SelectUserDifficult(string difficultId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from difficulty_levels where difficult_id = {difficultId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetString(1)}";
            }

            return result.Split(" ");
        }
        
        private string[] SelectUserColor(string colorId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from colors where color_id = {colorId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetString(1)}";
            }

            return result.Split(" ");
        }

        private string[] SelectUserResolution(string resolutionId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userResolution = $"select * from resolutions where resolution_id = {resolutionId}";
                var cmd = new NpgsqlCommand(userResolution, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetInt32(1)} {dr.GetInt32(2)} {dr.GetInt32(3)}";
            }

            return result.Split(" ");
        }

        private string[] SelectUserSettings(string settingsId)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userSettings = $"select * from settings where settings_id = {settingsId}";
                var cmd = new NpgsqlCommand(userSettings, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetInt32(0)} {dr.GetBoolean(1)} {dr.GetFloat(2)} {dr.GetFloat(3)} {dr.GetInt32(4)} {dr.GetInt32(5)} {dr.GetInt32(6)} {dr.GetInt32(7)}";
            }

            return result.Split(" ");
        }

        private string[] SelectUserData(string login, string password)
        {
            var result = "";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var userData = $"select * from users where login = '{login}' and password = '{password}'";
                var cmd = new NpgsqlCommand(userData, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    result =
                        $"{dr.GetString(0)} {dr.GetString(1)} {dr.GetInt32(2)} {dr.GetInt32(3)} {dr.GetValue(4)} {dr.GetValue(5)}";
            }

            return result.Split(" ");
        }

        private bool IsResolutionExist(int width, int height, int frameRate)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkResExist =
                    $"select exists (select * from resolutions where width = {width} and height = {height} and refresh_rate = {frameRate})";
                var cmd = new NpgsqlCommand(checkResExist, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    return dr.GetBoolean(0);
                return false;
            }
        }

        private bool IsUserExist(bool isRegistration)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkUserId = "";
                if (isRegistration)
                    checkUserId =
                        $"select exists (select * from users where login = '{UserData.UserLogin}')";
                else
                    checkUserId =
                        $"select exists (select * from users where login = '{UserData.UserLogin}' and password = '{UserData.UserPassword}')";

                var cmd =
                    new NpgsqlCommand(checkUserId, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) return dr.GetBoolean(0);
                return false;
            }
        }

        private bool IsSettingsExist()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var checkSettingsId =
                    $"select exists(select * from users where login = '{UserData.UserLogin}' and settings_id is not null)";
                var cmd =
                    new NpgsqlCommand(checkSettingsId, conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) return dr.GetBoolean(0);
                return false;
            }
        }

        private int SelectSecondsInGame()
        {
            var secondsInGame = "0";
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select seconds_in_game from users where login = '{UserData.UserLogin}'",
                    conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    secondsInGame = dr.GetInt32(0).ToString();
            }

            return Convert.ToInt32(secondsInGame);
        }

        private int SelectNewId(string idName, string table)
        {
            var newId = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select max({idName}) from {table}", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read())
                    try
                    {
                        newId = dr.GetInt32(0) + 1;
                    }
                    catch
                    {
                        newId = 1;
                    }
            }

            return newId;
        }

        private int SelectIdWhere(string idName, string table, string whereName, string where)
        {
            var id = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand($"select {idName} from {table} where {whereName} = '{where}'", conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) id = dr.GetInt32(0);
            }

            return id;
        }

        private int SelectIdWhere(string idName, string table, string whereName1, string whereName2, string whereName3,
            string where1, string where2, string where3)
        {
            var id = 1;
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                var cmd = new NpgsqlCommand(
                    $"select {idName} from {table} where {whereName1} = '{where1}' and {whereName2} = '{where2}' and {whereName3} = '{where3}'",
                    conn);
                var dr = cmd.ExecuteReader();
                while (dr.Read()) id = dr.GetInt32(0);
            }

            return id;
        }
    }
}