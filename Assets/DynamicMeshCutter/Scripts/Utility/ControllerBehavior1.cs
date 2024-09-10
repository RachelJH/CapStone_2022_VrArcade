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
        /// 무기에 달릴 점 두개(무기 직선)
        /// </summary>
        public Transform weaponPos1, weaponPos2;

        /// <summary>
        /// 무기에 달릴 점 하나 (관통부)
        /// </summary>
        public Transform weaponPos3;

        /// <summary>
        /// Cut함수에 보내줄 벡터
        /// </summary>
        private Vector3 _from1, _from2, _to;
        
        private bool _isDragging;

  
        protected override void Update()
        {
            base.Update();
        }

        //시작 좌표 저장
        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Zombi")) { // 칼과 물체가 접촉했을때 좀비일 경우
                Debug.Log("Enter");
                _from1 = weaponPos1.position; // 벡터값들을 저장
                _from2 = weaponPos2.position;
            }
        }

        //중간 좌표 계속 최신화
        private void OnTriggerStay(Collider other)
        {
            if (other.transform.CompareTag("Zombi"))
            {
                Debug.Log("Stay");
                _to = weaponPos3.position; // 관통부 벡터 저장
                VisualizeLine(true);
            }

            if (other.transform.CompareTag("Grabber")) {
                ownerPlayer = other.gameObject.transform.root.name;
            }
        }

        //칼이 관통됐을 경우 Cut 실행
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.transform.root.CompareTag("Zombi"))
            {
                // 접촉 좀비의 루트의 태그값을 변경시켜 해당 좀비만 적용되게
                other.gameObject.transform.root.tag = "CutZombi"; 
                VisualizeLine(false);
                Debug.Log("Exit");
                Cut();
            }
        }


       
        private void Cut()
        {
            Plane plane = new Plane(_from1, _from2, _to); // 검의 직선, 관통부의 점으로 이루어진 평면 생성

            var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var root in roots)
            {
                
                if (!root.activeInHierarchy)
                    continue;
                if (root.CompareTag("CutZombi")){

                    var targets = root.GetComponentsInChildren<MeshTarget>();
                    foreach (var target in targets)
                    {
                        if (root.transform.CompareTag("CutZombi")) // 루트 게임오브젝트의 태그가 컷좀비일 경우 컷 실행
                        {
                            root.gameObject.tag = "DeadZombi";
                            
                             Cut(target, _to, plane.normal, null, OnCreated);
                            ActionSingleCheck asc = GameObject.Find("SingleCheck").GetComponent<ActionSingleCheck>();
                            asc.GetKill();

                            Debug.Log("Cut");
                            //킬카운트
                            
                            

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