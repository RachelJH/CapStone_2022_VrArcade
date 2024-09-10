using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace DynamicMeshCutter
{

    [RequireComponent(typeof(LineRenderer))]
    public class ControllerBehavior : CutterBehaviour, IPunObservable
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

        PhotonView pv;

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                stream.SendNext(_from1);
                stream.SendNext(_from2);
                stream.SendNext(_to);
            }
            else {
                _from1 = (Vector3)stream.ReceiveNext();
                _from2 = (Vector3)stream.ReceiveNext();
                _to = (Vector3)stream.ReceiveNext();
            }
        }

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
                photonView.RPC("Cut", RpcTarget.All);
                pv = GameObject.Find("MyRemotePlayer").GetComponent<PhotonView>();
                if (pv != null)
                {
                    //pv.RPC("ScoreUp", RpcTarget.MasterClient, pv.Owner.ToString());
                    pv.RPC("KillZombieOnServer", RpcTarget.MasterClient);
                }
                //Cut();
            }
        }


        [PunRPC]
        private void Cut()
        {
            if (PhotonNetwork.IsMasterClient)
                Debug.Log("�����Ͱ� ������");

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
                            PhotonView pv = root.GetComponent<PhotonView>();
                             Cut(target, _to, plane.normal, null, OnCreated);
                               
                            
                            Debug.Log("Cut");
                            //ųī��Ʈ
                            Debug.Log(ownerPlayer + "�� ���� ����");
                            

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