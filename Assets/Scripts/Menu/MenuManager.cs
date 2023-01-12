using FreeMode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Menu
{
    public class MenuManager : MonoBehaviour
    {
        public bool isSimpleMode;
        public MenuUIManager menuUIManager;
        public FreeModeController freeModeController;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            menuUIManager.simpleModeBtn.onClick.AddListener(SimpleMode);
            menuUIManager.hardModeBtn.onClick.AddListener(HardMode);
            menuUIManager.exitBtn.onClick.AddListener(Exit);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
        {
            if (scene.buildIndex == 0)
            {
                var dontDestroyOnLoadObjects = FindObjectsOfType<MenuManager>();
                foreach (var obj in dontDestroyOnLoadObjects)
                {
                    if (obj.transform.gameObject != transform.gameObject)
                        Destroy(obj);
                }

                menuUIManager = FindObjectOfType<MenuUIManager>();
                menuUIManager.simpleModeBtn.onClick.AddListener(SimpleMode);
                menuUIManager.hardModeBtn.onClick.AddListener(HardMode);
                menuUIManager.exitBtn.onClick.AddListener(Exit);
            }
            else if (scene.buildIndex == 1)
            {
                freeModeController = FindObjectOfType<FreeModeController>();
                freeModeController.freeModeUIManager.backBtn.onClick.AddListener(Back);
                freeModeController.freeModeUIManager.exitBtn.onClick.AddListener(Exit);
                freeModeController.isSimpleMode = isSimpleMode;
            }
            else if (scene.buildIndex == 2)
            {
                freeModeController = FindObjectOfType<FreeModeController>();
                freeModeController.freeModeUIManager.backBtn.onClick.AddListener(Back);
                freeModeController.freeModeUIManager.exitBtn.onClick.AddListener(Exit);
                freeModeController.isSimpleMode = isSimpleMode;
            }
        }

        public void Back()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            Application.Quit();
        }

        private void SimpleMode()
        {
            SceneManager.LoadScene(1);
            isSimpleMode = true;
        }

        private void HardMode()
        {
            SceneManager.LoadScene(2);
            isSimpleMode = false;
        }
    }
}
