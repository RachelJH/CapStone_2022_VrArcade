/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Used for managing weapon icons colors in the weapon demo scenes.
*******************************************************************************************/
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StickyStickStuck
{
	public class UI_WeaponIcon : MonoBehaviour
    {
        #region Properties

        public Color defaultColor;
        public Color selectColor;

        [SerializeField, Tooltip("Inventory objects.")]
		private Inventory inventory;
		public Inventory _inventory
        {
			get { return inventory; }
			set { inventory = value; }
		}
        [SerializeField]
        private Inventory.WeaponTypes weaponTypes;
        public Inventory.WeaponTypes _weaponTypes
        {
            get { return weaponTypes; }
            set { weaponTypes = value; }
        }

        [SerializeField, Tooltip("Icon image used for the gun type.")]
		private Image icon;
		public Image Icon
		{
			get { return icon; }
			set { icon = value; }
		}

		[SerializeField, Tooltip("Text of the gun icon.")]
		private Text text;
		public Text Text
		{
			get { return text; }
			set { text = value; }
		}

        #endregion

        #region Unity Functions

        // Update is called once per frame
		void Update () 
		{
            if(_inventory._weaponTypes == _weaponTypes)
            {
                Icon.color = selectColor;
                Text.color = selectColor;
            }
            else
            {
                Icon.color = defaultColor;
                Text.color = defaultColor;
            }
        }

        #endregion
    }
}