using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DynamicMeshCutter
{

    [RequireComponent(typeof(LineRenderer))]
    public class ControllerBehavior1 : CutterBehaviour
    {
        private string ownerPlayer;
        

        public LineRenderer LR => GetComponent<LineRenderer>();

        /// <summary>
        /// ���⿡ �޸� �� �ΰ�(���� ����)
        /// </summary>
        public Transform weaponPos1, weaponPos2;

        /// <summary>
        /// ���⿡ �޸� �� �ϳ� (�����)
        /// </summary>
        public Transform weaponPos3;

        /// <summary>
        /// Cut�Լ��� ������ ����
        /// </summary>
        private Vector3 _from1, _from2, _to;
        
        private bool _isDragging;

  
        protected override void Update()
        {
            base.Update();
        }

        //���� ��ǥ ����
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Zombi")) { // Į�� ��ü�� ���������� ������ ���
                Debug.Log("Enter");
                _from1 = weaponPos1.position; // ���Ͱ����� ����
                _from2 = weaponPos2.position;
            }
        }

        //�߰� ��ǥ ��� �ֽ�ȭ
        private void OnTriggerStay(Collider other)
        {
            if (other.transform.CompareTag("Zombi"))
            {
                Debug.Log("Stay");
                _to = weaponPos3.position; // ����� ���� ����
                VisualizeLine(true);
            }

            if (other.transform.CompareTag("Grabber")) {
                ownerPlayer = other.gameObject.transform.root.name;
            }
        }

        //Į�� ������� ��� Cut ����
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Zombi"))
            {
                // ���� ������ ��Ʈ�� �±װ��� ������� �ش� ���� ����ǰ�
                other.gameObject.transform.root.tag = "CutZombi"; 
                VisualizeLine(false);
                Debug.Log("Exit");
                Cut();
            }
        }


       
        private void Cut()
        {
            Plane plane = new Plane(_from1, _from2, _to); // ���� ����, ������� ������ �̷���� ��� ����

            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                
                if (!root.activeInHierarchy)
                    continue;
                if (root.CompareTag("CutZombi")){

                    var targets = root.GetComponentsInChildren<MeshTarget>();
                    foreach (var target in targets)
                    {
                        if (root.transform.CompareTag("CutZombi")) // ��Ʈ ���ӿ�����Ʈ�� �±װ� �������� ��� �� ����
                        {
                            root.gameObject.tag = "DeadZombi";
                            
                             Cut(target, _to, plane.normal, null, OnCreated);
                            ActionSingleCheck asc = GameObject.Find("SingleCheck").GetComponent<ActionSingleCheck>();
                            asc.GetKill();

                            Debug.Log("Cut");
                            //ųī��Ʈ
                            
                            

                        }
                    }


                }
            }
            
        }

        void OnCreated(Info info, MeshCreationData cData)
        {
            MeshCreation.TranslateCreatedObjects(info, cData.CreatedObjects, cData.CreatedTargets, Separation);
        }
        private void VisualizeLine(bool value)
        {
            if (LR == null)
                return;

            LR.enabled = value;

            if (value)
            {   /*
                LR.positionCount = 2;
                LR.SetPosition(0, _from);
                LR.SetPosition(1, _to);
                */
            }
        }

        
    }



}