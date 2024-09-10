using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class WallFragment : MonoBehaviour {

    public GameObject particleEffect;

    [SerializeField]
    private float impactForce = 5f;
    [SerializeField]
    private float lifeTime = 0;

    private int wallMatIndex = 1;
    private int borderMatIndex = 0;

    private float scaleAdjust = 0.95f;

    private Collider fragCollider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Create wall fragment
    public void Init(Color[] shapeColorArray, Vector2 shapeTextRes, Vector3 wallScale, Vector2 wallTextRes, Vector3 position, Vector3 impactDirection)
    {
        Texture2D shapeTexture = new Texture2D((int)shapeTextRes.x, (int)shapeTextRes.y);
        shapeTexture.SetPixels(shapeColorArray);
        shapeTexture.Apply();

        //calculate border mask
        Texture2D borderMask = new Texture2D((int)shapeTextRes.x, (int)shapeTextRes.y);
        for (int i = 0; i < shapeColorArray.Length; i++) shapeColorArray[i] = new Color(1.0f - shapeColorArray[i].r, 1.0f - shapeColorArray[i].g, 1.0f - shapeColorArray[i].b, 1.0f);
        borderMask.SetPixels(shapeColorArray);
        borderMask.Apply();

        //calculate scale
        float scaleWidth = (shapeTextRes.x / wallTextRes.x) * wallScale.x * scaleAdjust;
        float scaleHeight = (shapeTextRes.y / wallTextRes.y) * wallScale.y * scaleAdjust;
        transform.localScale = new Vector3(scaleWidth, scaleHeight, wallScale.z);

        //disable temporarily the collider
        fragCollider = GetComponent<Collider>();
        if (fragCollider!=null)
        {
            fragCollider.enabled = false;
            Invoke("EnableCollision", 0.2f);
        }

        transform.position = position;
        GetComponent<Rigidbody>().AddForce(impactDirection * impactForce, ForceMode.Impulse);

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            Debug.Log("Attach MeshRenderer to the WallFragment object!");
            Destroy(this.gameObject);
            return;
        } 

        //assign materials with created textures
        meshRenderer.materials[wallMatIndex].SetTexture("_ShapeMask", shapeTexture);
        meshRenderer.materials[borderMatIndex].SetTexture("_CutoutMap", borderMask);

        //create particle effect
        if (particleEffect != null)
        {
            GameObject pEffect = Instantiate(particleEffect, transform.position, Quaternion.LookRotation(-impactDirection)) as GameObject;
            Destroy(pEffect, 1f);
        }

        if (lifeTime > 0) Destroy(this.gameObject, lifeTime);

    }

    private void EnableCollision()
    {
        fragCollider.enabled = true;
    }
}
