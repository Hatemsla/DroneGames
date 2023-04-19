using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Builder
{
    public class BuilderUI : MonoBehaviour
    {
        public Button floorBtn;
        public Button wallBtn;
        public Button slantBtn;
        public Button gateBtn;
        public Button exitBtn;
        public Button backBtn;
        public Button backTabBtn;
        public Button saveBtn;
        public Button testLevelBtn;
        public Button returnToLevelBtn;
        
        public RectTransform pathArrow;
        public Sprite pointerIcon;
        public Sprite outOfScreenIcon;

        public GameObject createPanel;
        public GameObject editButtons;
        public GameObject tabPanel;
        public GameObject levelResultPanel;

        public List<Button> createButtons;
    }
}