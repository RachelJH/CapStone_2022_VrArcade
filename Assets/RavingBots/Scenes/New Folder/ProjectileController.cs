using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace BigRookGames.Weapons
{
    public class ProjectileController : MonoBehaviourPun
    {
        PhotonView pv;

        // --- Config ---
        public float speed = 100;
        public LayerMask collisionLayerMask;

        // --- Explosion VFX ---
        public GameObject rocketExplosion;
        public GameObject playerRocketExplosion;
        // --- Projectile Mesh ---
        public MeshRenderer projectileMesh;

        // --- Script Variables ---
        private bool targetHit;

        // --- Audio ---
        public AudioSource inFlightAudioSource;

        // --- VFX ---
        public ParticleSystem disableOnHit;

        public LayerMask layerMask;
        public float explosionForce;
        public float searchPlayer;

        private void Update()
        {
            // --- Check to see if the target has been hit. We don't want to update the position if the target was hit ---
            if (targetHit) return;

            // --- moves the game object in the forward direction at the defined speed ---
            transform.position += transform.forward * (speed * Time.deltaTime);
        }

        private void Awake()
        {
            pv = GetComponent<PhotonView>();
            if (pv.IsMine)
                StartCoroutine(DestroyAfter(gameObject, 10f));
        }

        /// <summary>
        /// Explodes on contact.
        /// </summary>
        /// <param name="collision"></param>
        private void OnCollisionEnter(Collision collision)
        {
            // --- return if not enabled because OnCollision is still called if compoenent is disabled ---
            if (!enabled) return;

            // --- Explode when hitting an object and disable the projectile mesh ---
            photonView.RPC("Explode", RpcTarget.MasterClient);
            

            //Explode();
            projectileMesh.enabled = false;
            targetHit = true;
            inFlightAudioSource.Stop();
            foreach(Collider col in GetComponents<Collider>())
            {
                col.enabled = false;
            }
            
            Collider[] colliders = Physics.OverlapSphere(transform.position, searchPlayer, layerMask);
            foreach (Collider collider in colliders)
            {
                if (collider != null)
                {
                    Debug.Log("Boom Player Check1");
                    Vector3 dir = collider.transform.position - transform.position;
                    Debug.Log(dir + " / " + transform.position + " / " + collider.transform.position);
                    NggImpactReceiver impacter = collider.GetComponent<NggImpactReceiver>();
                    Debug.Log("Boom" + collider.name);
                    photonView.RPC("PlayerExplodeEffect", RpcTarget.MasterClient);
                    impacter.AddImpact(dir, explosionForce);
                }
            }

            disableOnHit.Stop();


            // --- Destroy this object after 2 seconds. Using a delay because the particle system needs to finish ---
            //Destroy(gameObject, 5f);
            if(pv.IsMine)
            StartCoroutine(DestroyAfter(gameObject, 5f));
        }


        /// <summary>
        /// Instantiates an explode object.
        /// </summary>
        /// 
        
        [PunRPC]
        private void Explode()
        {
            // --- Instantiate new explosion option. I would recommend using an object pool ---
            GameObject newExplosion = PhotonNetwork.Instantiate(rocketExplosion.name, transform.position, rocketExplosion.transform.rotation);
            //GameObject newExplosion = Instantiate(rocketExplosion, transform.position, rocketExplosion.transform.rotation, null);
        }

        [PunRPC]
        private void PlayerExplodeEffect() {
            GameObject playerExplosion = PhotonNetwork.Instantiate(playerRocketExplosion.name, transform.position, rocketExplosion.transform.rotation);
        }


        IEnumerator DestroyAfter(GameObject target, float delay)
        {
            // delay 만큼 쉬고
            yield return new WaitForSeconds(delay);

            // target이 아직 파괴되지 않았다면
            if (target != null)
            {
                // target을 모든 네트워크 상에서 파괴
               
                PhotonNetwork.Destroy(target);
            }
        }
    }
}