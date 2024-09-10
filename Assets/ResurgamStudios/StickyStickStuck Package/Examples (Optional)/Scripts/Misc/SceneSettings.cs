/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for managing the demo scenes.
*******************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

namespace StickyStickStuck
{
    public class SceneSettings : MonoBehaviour
    {
        //Singleton Logic
        private static SceneSettings _instance;
        public static SceneSettings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = GameObject.FindObjectOfType<SceneSettings>();

                return _instance;
            }
        }

        #region Properties

        [SerializeField, Tooltip("Enable/disable's the main menu."), Header("Scene Options:")]
        private bool toggleMainMenu = false;
        public bool ToggleMainMenu
        {
            get { return toggleMainMenu; }
            set { toggleMainMenu = value; }
        }
        [SerializeField, Tooltip("Locks the mouse.")]
        private bool lockMouse = false;
        public bool LockMouse
        {
            get { return lockMouse; }
            set { lockMouse = value; }
        }

        [SerializeField, Tooltip("GUI canvas gameobject."), Header("Canvas Objects:")]
        private GameObject guiCanvas;
        public GameObject GUICanvas
        {
            get { return guiCanvas; }
            set { guiCanvas = value; }
        }
        [SerializeField, Tooltip("Help text.")]
        private Text helpUIText;
        public Text HelpUIText
        {
            get { return helpUIText; }
            set { helpUIText = value; }
        }
        [SerializeField, Tooltip("Gameobject that contatins the panel for the main menu.")]
        private GameObject panel_MainMenu;
        public GameObject Panel_MainMenu
        {
            get { return panel_MainMenu; }
            set { panel_MainMenu = value; }
        }

        [SerializeField, Tooltip("FPS weapon script."), Header("Scene Objects:")]
        private Inventory inventory;
        public Inventory _inventory
        {
            get { return inventory; }
            set { inventory = value; }
        }
        [SerializeField, Tooltip("firstPersonController script.")]
        private UnityStandardAssets.Characters.FirstPerson.FirstPersonController firstPersonController;
        public UnityStandardAssets.Characters.FirstPerson.FirstPersonController FirstPersonController
        {
            get { return firstPersonController; }
            set { firstPersonController = value; }
        }
		
		private bool toggleGUI = true;
        public bool ToggleGUI
        {
            get { return toggleGUI; }
            set { toggleGUI = value; }
        }

        #endregion

        #region Unity Functions

        void Update()
        {
            //Main Key
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                ToggleMainMenu = !ToggleMainMenu;
            }
            //Reset Scene Key
            if (Input.GetKeyUp(KeyCode.R))
            {
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

			//Toggles GUI
			if(Input.GetKeyUp(KeyCode.Home))
			{
                ToggleGUILayout();
			}
		
            //Help ESC text
            if (HelpUIText != null)
            {
                if (ToggleMainMenu)
                    HelpUIText.text = "Press <b>ESC</b> to go back.";
                else
                    HelpUIText.text = "Press <b>ESC</b> for menu.";
            }

            if (Panel_MainMenu != null)
                Panel_MainMenu.SetActive(ToggleMainMenu);

            if (_inventory != null)
            {
                if(_inventory.Bow.GetComponent<Weapon>() != null)
                    _inventory.Bow.GetComponent<Weapon>().enabled = (!ToggleMainMenu);
                if (_inventory.Axe.GetComponent<Weapon>() != null)
                    _inventory.Axe.GetComponent<Weapon>().enabled = (!ToggleMainMenu);
                if (_inventory.Spear.GetComponent<Weapon>() != null)
                    _inventory.Spear.GetComponent<Weapon>().enabled = (!ToggleMainMenu);

                if (_inventory.Bow.GetComponent<Weapon2D>() != null)
                    _inventory.Bow.GetComponent<Weapon2D>().enabled = (!ToggleMainMenu);
                if (_inventory.Axe.GetComponent<Weapon2D>() != null)
                    _inventory.Axe.GetComponent<Weapon2D>().enabled = (!ToggleMainMenu);
                if (_inventory.Spear.GetComponent<Weapon2D>() != null)
                    _inventory.Spear.GetComponent<Weapon2D>().enabled = (!ToggleMainMenu);
            }

            if (FirstPersonController != null)
                FirstPersonController.enabled = (!ToggleMainMenu);

			if((!ToggleMainMenu && LockMouse))
			{
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
			}
			else
			{
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
			}
        }

        #endregion

        #region Functions
		
		public void ToggleGUILayout()
		{
            ToggleGUI = !ToggleGUI;

            GUICanvas.SetActive(ToggleGUI);
		}
	
        //Load scene function
        public void LoadScene(string sceneName)
        {
			SceneManager.LoadScene(sceneName);
        }

        #endregion
    }
}