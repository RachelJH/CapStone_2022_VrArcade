/*******************************************************************************************
* Author: Lane Gresham, AKA LaneMax
* Websites: http://resurgamstudios.com
* Description: Manages the weapon inventory.
*******************************************************************************************/
using UnityEngine;
using System.Collections;

namespace StickyStickStuck
{
    public class Inventory : MonoBehaviour
    {
        //Weapon Types
        public enum WeaponTypes
        {
            Bow,
            Axe,
            Spear,
            Hands
        }

        //Weapon mode
        [SerializeField]
        private WeaponTypes weaponTypes;
        public WeaponTypes _weaponTypes
        {
            get { return weaponTypes; }
            set { weaponTypes = value; }
        }

        //Bow Weapon
        [SerializeField]
        private GameObject bow;
        public GameObject Bow
        {
            get { return bow; }
            set { bow = value; }
        }
        //Axe Weapon
        [SerializeField]
        private GameObject axe;
        public GameObject Axe
        {
            get { return axe; }
            set { axe = value; }
        }
        //Spear Weapon
        [SerializeField]
        private GameObject spear;
        public GameObject Spear
        {
            get { return spear; }
            set { spear = value; }
        }
        //Hands
        [SerializeField]
        private GameObject hands;
        public GameObject Hands
        {
            get { return hands; }
            set { hands = value; }
        }
        

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _weaponTypes = WeaponTypes.Bow;
                SyncGunSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _weaponTypes = WeaponTypes.Axe;
                SyncGunSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                _weaponTypes = WeaponTypes.Spear;
                SyncGunSelection();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                _weaponTypes = WeaponTypes.Hands;
                SyncGunSelection();
            }
        }

        void SyncGunSelection()
        {
            switch (_weaponTypes)
            {
                case WeaponTypes.Bow:
                    Bow.SetActive(true);
                    Axe.SetActive(false);
                    Spear.SetActive(false);
                    if(Hands != null)
                        Hands.SetActive(false);
                    break;
                case WeaponTypes.Axe:
                    Bow.SetActive(false);
                    Axe.SetActive(true);
                    Spear.SetActive(false);
                    if(Hands != null)
                        Hands.SetActive(false);
                    break;
                case WeaponTypes.Spear:
                    Bow.SetActive(false);
                    Axe.SetActive(false);
                    Spear.SetActive(true);
                    if(Hands != null)
                        Hands.SetActive(false);
                    break;
                case WeaponTypes.Hands:
                    Bow.SetActive(false);
                    Axe.SetActive(false);
                    Spear.SetActive(false);
                    if(Hands != null)
                        Hands.SetActive(true);
                    break;
            }
        }
    }
}