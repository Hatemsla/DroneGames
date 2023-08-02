using System;
using System.Collections.Generic;
using System.Linq;
using Drone;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Builder
{
    public class BuilderUI : MonoBehaviour
    {
        [Header("Drone view")]
        public GameObject droneView;
        public TMP_Text speedText;
        public TMP_Text batteryText;
        public TMP_Text timeText;
        public TMP_Text terminalTimeText;
        public TMP_Text checkpointsCountText;
        public TMP_Text coinsCountText;
        public TMP_Text crystalsCountText;
        public Prompt prompt;
        public HelpMessage helpMessage;
        public DroneBars healthBar;
        public DroneBars armorBar;

        
        [Header("UI")]
        public Button floorBtn;
        public Button wallBtn;
        public Button slantBtn;
        public Button gateBtn;
        public Button editorGameExitBtn;
        public Button editorExitBtn;
        public Button gameExitBtn;
        public Button backEditorTabBtn;
        public Button backGameTabBtn;
        public Button saveBtn;
        public Button testLevelBtn;
        public Button returnToLevelBtn;
        
        public RectTransform pathArrow;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;

        public GameObject exitBuilderPanel;
        public GameObject createPanel;
        public GameObject editButtons;
        public GameObject editorTabPanel;
        public GameObject gameTabPanel;
        public GameObject levelResultPanel;
        public GameObject loadLevelPanel;
        public GameObject uiPanel;
        public GameObject portUI;
        public GameObject passwordUI;
        public GameObject terminalUI;
        public GameObject loadPanel;
        public GameObject objectEditPanel;
        public GameObject saveLevelPanel;
        public GameObject createButtonsParent;
        public GameObject securityCameraView;

        public List<Button> createButtons;
        public List<ObjectPreInfo> objectPreInfos;

        [Header("Animations")]
        public Animator savePanelAnimator;



        private void Awake()
        {
            // createButtons = createButtonsParent.GetComponentsInChildren<Button>().ToList();
            foreach (var createButton in createButtons)
            {
                objectPreInfos.Add(createButton.GetComponent<ObjectPreInfo>());
            }
        }
    }
}