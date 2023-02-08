using System;
using System.Collections.Generic;
using System.Linq;
using DroneFootball;
using DroneRace;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public bool isSimpleMode;
        public MenuUIManager menuUIManager;
        public RaceController raceController;
        public FootballController footballController;
        public Color botsColor;
        public Color playerColor;

        private Resolution[] _resolutions;

        private readonly List<string> _difficulties = new List<string>
            {"Супер легко", "Легко", "Нормально", "Сложно", "Невозможно"};

        private readonly List<float> _gatesSize = new List<float> {3f, 2f, 1.5f, 1.25f, 1f};
        private readonly List<float> _droneSpeed = new List<float> {0.5f, 0.75f, 1f, 1.5f, 2f};
        private float _currentGateScale;
        private float _currentAIDroneSpeed;
        private float _currentVolume;
        private float _currentYawSensitivity = 1;
        private int _currentDifficultIndex;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            _resolutions = Screen.resolutions.Distinct().ToArray();
            SetDropdownResolutions();
            SetDropdownDifficulties();

            menuUIManager.raceBtn.onClick.AddListener(delegate { StartGame(1); });
            menuUIManager.footballBtn.onClick.AddListener(delegate { StartGame(2); });
            menuUIManager.difficultToggle.isOn = false;
            menuUIManager.volumeSlider.value = 1;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void SetDropdownResolutions()
        {
            var options = new List<string>();
            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                options.Add($"{_resolutions[i].width} x {_resolutions[i].height} {_resolutions[i].refreshRate}Hz");

                if (_resolutions[i].width == Screen.currentResolution.width ||
                    _resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            menuUIManager.resolutionDropdown.AddOptions(options);
            menuUIManager.resolutionDropdown.value = currentResolutionIndex;
            menuUIManager.resolutionDropdown.RefreshShownValue();
        }

        private void SetDropdownDifficulties()
        {
            menuUIManager.difficultDropdown.ClearOptions();
            menuUIManager.difficultDropdown.AddOptions(_difficulties);
            menuUIManager.difficultDropdown.value = 2;
        }

        public void SetResolution(int resolutionIndex)
        {
            if (_resolutions == null)
                _resolutions = Screen.resolutions.Distinct().ToArray();
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetDifficult(int difficultIndex)
        {
            _currentGateScale = _gatesSize[difficultIndex];
            _currentAIDroneSpeed = _droneSpeed[difficultIndex];
            _currentDifficultIndex = difficultIndex;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            AudioListener.volume = _currentVolume;
            if (scene.buildIndex == 0)
            {
                var dontDestroyOnLoadObjects = FindObjectsOfType<MenuManager>();
                foreach (var obj in dontDestroyOnLoadObjects)
                {
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);
                }

                menuUIManager = FindObjectOfType<MenuUIManager>();
                menuUIManager.volumeSlider.value = _currentVolume;
                menuUIManager.volumeSlider.onValueChanged.AddListener(delegate { ChangeVolume(); });
                menuUIManager.yawSensitivitySlider.value = _currentYawSensitivity - 1;
                menuUIManager.yawSensitivitySlider.onValueChanged.AddListener(delegate { ChangeYawSensitivity(); });
                menuUIManager.gameBtn.onClick.AddListener(GameMenu);
                menuUIManager.optionsBtn.onClick.AddListener(OptionsMenu);
                menuUIManager.startExitBtn.onClick.AddListener(Exit);
                menuUIManager.optionsExitBtn.onClick.AddListener(Exit);
                menuUIManager.gameExitBtn.onClick.AddListener(Exit);
                menuUIManager.gameBackBtn.onClick.AddListener(Back);
                menuUIManager.optionsBackBtn.onClick.AddListener(Back);
                menuUIManager.isFullscreenToggle.onValueChanged.AddListener(Fullscreen);
                menuUIManager.difficultDropdown.onValueChanged.AddListener(SetDifficult);
                menuUIManager.difficultDropdown.value = _currentDifficultIndex;
                menuUIManager.difficultToggle.onValueChanged.AddListener(SetGameMode);
                menuUIManager.difficultToggle.isOn = false;
                menuUIManager.raceBtn.onClick.AddListener(delegate { StartGame(1); });
                menuUIManager.footballBtn.onClick.AddListener(delegate { StartGame(2); });
                menuUIManager.generalSettingsBtn.onClick.AddListener(GeneralSettings);
                menuUIManager.soundSettingsBtn.onClick.AddListener(SoundSettings);
                menuUIManager.controlSettingsBtn.onClick.AddListener(ControlSettings);
                menuUIManager.customizationSettingsBtn.onClick.AddListener(CustomizationSettings);
                menuUIManager.playerColorPicker.GetComponentInChildren<ColorPreview>().color = playerColor;
                menuUIManager.playerColorPicker.GetComponentInChildren<ColorPreview>().image.color = playerColor;
                menuUIManager.botColorPicker.GetComponentInChildren<ColorPreview>().color = botsColor;
                menuUIManager.botColorPicker.GetComponentInChildren<ColorPreview>().image.color = botsColor;

                SetDropdownResolutions();
                menuUIManager.resolutionDropdown.onValueChanged.AddListener(SetResolution);
            }
            else if (scene.buildIndex == 1)
            {
                raceController = FindObjectOfType<RaceController>();
                raceController.currentAIDroneSpeed = _currentAIDroneSpeed;
                raceController.raceUIManager.backBtn.onClick.AddListener(Back);
                raceController.raceUIManager.exitBtn.onClick.AddListener(Exit);
                raceController.isSimpleMode = isSimpleMode;
                raceController.droneRaceController.yawPower = _currentYawSensitivity;
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_Color", playerColor);
                raceController.droneRaceController.droneMeshRenderer.material.SetColor("_EmissionColor", playerColor);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_Color", botsColor);
                raceController.droneRaceAI.droneMeshRenderer.material.SetColor("_EmissionColor", botsColor);
            }
            else if (scene.buildIndex == 2)
            {
                footballController = FindObjectOfType<FootballController>();
                footballController.currentGateScale = _currentGateScale;
                footballController.currentAIDroneSpeed = _currentAIDroneSpeed;
                footballController.footballUIManager.backBtn.onClick.AddListener(Back);
                footballController.footballUIManager.exitBtn.onClick.AddListener(Exit);
                footballController.isSimpleMode = isSimpleMode;
                footballController.droneFootballController.yawPower = _currentYawSensitivity;
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_Color", playerColor);
                footballController.droneFootballController.droneMeshRenderer.material.SetColor("_EmissionColor", playerColor);
                footballController.droneFootballAIList[0].droneMeshRenderer.material.SetColor("_Color", botsColor);
                footballController.droneFootballAIList[0].droneMeshRenderer.material.SetColor("_EmissionColor", botsColor);
                footballController.droneFootballAIList[1].droneMeshRenderer.material.SetColor("_Color", botsColor);
                footballController.droneFootballAIList[1].droneMeshRenderer.material.SetColor("_EmissionColor", botsColor);
            }
        }

        public void Back()
        {
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                menuUIManager.gameMenu.SetActive(false);
                menuUIManager.optionMenu.SetActive(false);
                menuUIManager.startMenu.SetActive(true);
                menuUIManager.botColorImage.SetActive(false);
                menuUIManager.playerColorImage.SetActive(false);
                menuUIManager.generalSettings.SetActive(true);
                menuUIManager.soundSettings.SetActive(false);
                menuUIManager.controlSettings.SetActive(false);
                menuUIManager.customizationSettings.SetActive(false);
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }

        public void Exit()
        {
            Application.Quit();
        }

        public void GameMenu()
        {
            menuUIManager.gameMenu.SetActive(true);
            menuUIManager.optionMenu.SetActive(false);
            menuUIManager.startMenu.SetActive(false);
        }

        public void OptionsMenu()
        {
            menuUIManager.gameMenu.SetActive(false);
            menuUIManager.optionMenu.SetActive(true);
            menuUIManager.startMenu.SetActive(false);
        }

        public void Fullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }

        public void SetGameMode(bool mode)
        {
            isSimpleMode = !mode;
        }

        public void ChangeVolume()
        {
            AudioListener.volume = menuUIManager.volumeSlider.value;
            _currentVolume = menuUIManager.volumeSlider.value;
        }

        public void ChangeYawSensitivity()
        {
            _currentYawSensitivity = menuUIManager.yawSensitivitySlider.value + 1;
        }

        private void StartGame(int sceneIndex)
        {
            botsColor = menuUIManager.botColorPicker.GetComponentInChildren<ColorPreview>().color;
            playerColor = menuUIManager.playerColorPicker.GetComponentInChildren<ColorPreview>().color;
            SceneManager.LoadScene(sceneIndex);
        }

        public void GeneralSettings()
        {
            menuUIManager.generalSettings.SetActive(true);
            menuUIManager.soundSettings.SetActive(false);
            menuUIManager.controlSettings.SetActive(false);
            menuUIManager.customizationSettings.SetActive(false);
            menuUIManager.botColorImage.SetActive(false);
            menuUIManager.playerColorImage.SetActive(false);
        }

        public void SoundSettings()
        {
            menuUIManager.generalSettings.SetActive(false);
            menuUIManager.soundSettings.SetActive(true);
            menuUIManager.controlSettings.SetActive(false);
            menuUIManager.customizationSettings.SetActive(false);
            menuUIManager.botColorImage.SetActive(false);
            menuUIManager.playerColorImage.SetActive(false);
        }

        public void ControlSettings()
        {
            menuUIManager.generalSettings.SetActive(false);
            menuUIManager.soundSettings.SetActive(false);
            menuUIManager.controlSettings.SetActive(true);
            menuUIManager.customizationSettings.SetActive(false);
            menuUIManager.botColorImage.SetActive(false);
            menuUIManager.playerColorImage.SetActive(false);
        }

        public void CustomizationSettings()
        {
            menuUIManager.generalSettings.SetActive(false);
            menuUIManager.soundSettings.SetActive(false);
            menuUIManager.controlSettings.SetActive(false);
            menuUIManager.customizationSettings.SetActive(true);
            menuUIManager.botColorImage.SetActive(false);
            menuUIManager.playerColorImage.SetActive(false);
        }
    }
}
