using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class BreakData
{
    public int typeID;
    public Texture2D transition;
    [Range(0, 1)]
    public float holeWidth = 0.5f, holeHeight = 0.5f;
}

public class ColliderData
{
    public int colliderStartX;
    public int colliderStartY;
    public int colliderWidth;
    public int colliderHeight;

    public ColliderData()
    {

    }

    public ColliderData(int colliderStartX,int colliderStartY,int colliderWidth,int colliderHeight)
    {
        this.colliderStartX = colliderStartX;
        this.colliderStartY = colliderStartY;
        this.colliderWidth = colliderWidth;
        this.colliderHeight = colliderHeight;
    }

    public ColliderData Merge(ColliderData cd)  //merge colliders
    {
        int startY = Mathf.Min(colliderStartY, cd.colliderStartY);
        int height = (colliderStartY == startY) ? colliderHeight : cd.colliderHeight;

        if(colliderStartX==cd.colliderStartX && colliderWidth==cd.colliderWidth && (startY+height==colliderStartY || startY + height == cd.colliderStartY))
        {
            colliderStartY = Mathf.Min(colliderStartY, cd.colliderStartY);
            colliderHeight = colliderHeight + cd.colliderHeight;
            return cd;
        }

        return null;
    }
}

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class BreakableWall : MonoBehaviour {

    public GameObject wallFragmentPrefab;

    [SerializeField]
    private int breakTextWidth = 512, breakTextHeight = 512;

    [SerializeField]
    private int colliderResX = 20, colliderResY = 20;

    [SerializeField]
    private List<BreakData> breakTypes;
    
    private int wallMatIndex = 1;
    private int borderMatIndex = 0;

    private Texture2D breakMap;

    private bool isError;

    private MeshCollider meshCollider;

    private List<GameObject> colliders;
    private Bounds baseColliderBounds;

    private bool[,] colliderMatrix;

	// Use this for initialization
	void Start () {

        meshCollider = GetComponent<MeshCollider>();
        if (meshCollider == null)
        {
            isError = true;
            Debug.LogError("Attach MeshCollider to the wall object!");
            return;
        }
        meshCollider.enabled = false;

        CreateBaseMaps();
        CreateBaseCollider();

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    //Init hole texture
    private void CreateBaseMaps()
    {
        if (isError) return;

        breakMap = new Texture2D(breakTextWidth, breakTextHeight);

        Color[] whiteArray = new Color[breakTextWidth * breakTextHeight];
        for (int i = 0; i < whiteArray.Length; i++) whiteArray[i] = new Color(1, 1, 1, 1);

        breakMap.SetPixels(whiteArray);
        breakMap.Apply();        

        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (!meshRenderer)
        {
            isError = true;
            Debug.LogError("Add MeshRenderer component to the breakable wall object!");
            return;
        }

        //assign materials with created textures
        meshRenderer.materials[wallMatIndex].SetTexture("_CutoutMap", breakMap);
        meshRenderer.materials[borderMatIndex].SetTexture("_CutoutMap", breakMap);        

    }

    //Calculate hole texture
    private void CreateWallBreak(int breakType, Vector2 uvPosition, Ray ray)
    {
        if (isError) return;

        uvPosition = new Vector2(Mathf.Clamp(uvPosition.x, 0f, 1f), Mathf.Clamp(uvPosition.y, 0f, 1f));

        BreakData breakData = breakTypes.Find(i => i.typeID == breakType);
        if (breakData == null || uvPosition == new Vector2(0, 0) || uvPosition == new Vector2(1, 1))
        {
            Debug.Log("This BreakType doesn't exist!");
            return;
        }

        //calculate mask position and visible area
        int patternStartMapX = (int)(breakMap.width * uvPosition.x) - (breakData.transition.width / 2);
        int patternStartMaskX = 0;
        if (patternStartMapX < 0)
        {
            patternStartMaskX = patternStartMapX * -1;
            patternStartMapX = 0;
        }

        int patternStartMapY = (int)(breakMap.height * uvPosition.y) - (breakData.transition.height / 2);
        int patternStartMaskY = 0;
        if (patternStartMapY < 0)
        {
            patternStartMaskY = patternStartMapY * -1;
            patternStartMapY = 0;
        }

        int patternWidth = breakData.transition.width - patternStartMaskX;
        if ((int)(breakMap.width * uvPosition.x) + (breakData.transition.width / 2) > breakMap.width) patternWidth -= (((int)(breakMap.width * uvPosition.x) + (breakData.transition.width / 2)) - breakMap.width);

        int patternHeight = breakData.transition.height - patternStartMaskY;
        if ((int)(breakMap.height * uvPosition.y) + (breakData.transition.height / 2) > breakMap.height) patternHeight -= (((int)(breakMap.height * uvPosition.y) + (breakData.transition.height / 2)) - breakMap.height);

        //create new hole texture map
        Color[] breakColorArray = breakData.transition.GetPixels(patternStartMaskX, patternStartMaskY, patternWidth, patternHeight);
        Color[] baseColorArray = breakMap.GetPixels(patternStartMapX, patternStartMapY, patternWidth, patternHeight);
        Color[] newColorArray = new Color[breakColorArray.Length];
        for (int i = 0; i < newColorArray.Length; i++) newColorArray[i] = breakColorArray[i] * baseColorArray[i];

        breakMap.SetPixels(patternStartMapX, patternStartMapY, patternWidth, patternHeight, newColorArray);
        breakMap.Apply();

        //create wall fragment
        if (wallFragmentPrefab != null)
        {
            GameObject wallFrag = Instantiate(wallFragmentPrefab) as GameObject;
            WallFragment wf = wallFrag.GetComponent<WallFragment>();
            if (wf == null) return;
            wf.Init(breakColorArray, new Vector2(patternWidth, patternHeight), transform.localScale, new Vector2(breakMap.width, breakMap.height), ray.origin, ray.direction);
        }

        //refresh colliders
        CreateNewCollisionBreak(breakType, patternStartMapX, patternStartMapY, patternWidth, patternHeight, patternStartMaskX, patternStartMaskY);

    }

    //Create hole on the wall
    public void BreakWall(int breakType, Ray ray)
    {
        if (isError) return;

        meshCollider.enabled = true;
        RaycastHit hit;
        if(Physics.Raycast(ray,out hit, Mathf.Infinity))
        {
            if (hit.transform == transform)
            {
                Vector2 uvPosition = hit.textureCoord;
                if (uvPosition != null) CreateWallBreak(breakType, uvPosition, new Ray(hit.point, ray.direction));
            }
        }
        meshCollider.enabled = false;       

    }

    //Calculate start collider
    private void CreateBaseCollider()
    {
        GameObject tempObject = Instantiate(this.gameObject, Vector3.zero, Quaternion.Euler(0, 0, 0)) as GameObject;

        //create base object
        BoxCollider tempCollider = tempObject.AddComponent<BoxCollider>();
        tempCollider.size = new Vector3(tempCollider.size.x, tempCollider.size.y, tempCollider.size.z * 0.9f);
        baseColliderBounds = tempCollider.bounds;
        Destroy(tempCollider);
        Destroy(tempObject);

        //create base collider
        GameObject baseCollider = new GameObject();        
        baseCollider.name = "wall_collider";
        baseCollider.transform.rotation = transform.rotation;
        baseCollider.transform.parent = this.gameObject.transform;
        baseCollider.transform.localPosition = new Vector3(0, 0, 0);
        baseCollider.transform.localRotation = Quaternion.Euler(0, 0, 0);
        BoxCollider collider = baseCollider.AddComponent<BoxCollider>();
        collider.size = new Vector3(baseColliderBounds.size.x, baseColliderBounds.size.y, baseColliderBounds.size.z);
        collider.center = baseColliderBounds.center;


        baseCollider.AddComponent<WallColliderHandler>();

        colliders = new List<GameObject>();
        colliders.Add(baseCollider);

        colliderMatrix = new bool[colliderResX, colliderResY];
    }

    //Calculate collider matrix
    private void CreateNewCollisionBreak(int breakType, int patternStartMapX,int patternStartMapY, int patternWidth, int patternHeight, int patternMaskStartX, int patternMaskStartY)
    {
        BreakData breakData = breakTypes.Find(i => i.typeID == breakType);
        if (breakData == null)
        {
            Debug.Log("This BreakType doesn't exist!");
            return;
        }

        float scaleWidth= (float)colliderResX / (float)breakTextWidth;
        float scaleHeight= (float)colliderResY / (float)breakTextHeight;

        int collX = Mathf.RoundToInt(patternStartMapX * scaleWidth);
        int collY = Mathf.RoundToInt(patternStartMapY * scaleHeight);
        int collWidth = Mathf.RoundToInt(patternWidth * scaleWidth);
        int collHeight = Mathf.RoundToInt(patternHeight * scaleHeight);
                
        if (collWidth <= 0 || collHeight <= 0) return;

        bool[,] patternMatrix = new bool[Mathf.RoundToInt(breakData.transition.width * scaleWidth), Mathf.RoundToInt(breakData.transition.height * scaleHeight)];

        //create new collision matrix
        if (patternMatrix.GetLength(0) == 0 || patternMatrix.GetLength(1) == 0) return;
        else if (patternMatrix.GetLength(0) == 1 || patternMatrix.GetLength(1) == 1) patternMatrix[0, 0] = true;
        else
        {
            int holeWidth = Mathf.RoundToInt(patternMatrix.GetLength(0) * breakData.holeWidth);
            int holeHeight = Mathf.RoundToInt(patternMatrix.GetLength(1) * breakData.holeHeight);

            for(int i=(patternMatrix.GetLength(0)-holeWidth)/2; i<(patternMatrix.GetLength(0) - holeWidth) / 2 + holeWidth; i++)
            {
                for (int j = (patternMatrix.GetLength(1) - holeHeight) / 2; j < (patternMatrix.GetLength(1) - holeHeight) / 2 + holeHeight; j++)
                {
                    patternMatrix[i, j] = true;
                }
            }
        }

        int patternMStartX = Mathf.RoundToInt(patternMaskStartX * scaleWidth);
        int patternMStartY = Mathf.RoundToInt(patternMaskStartY * scaleHeight);

        //merge matrices
        for(int i=patternMStartX; i<collWidth; i++)
        {
            for(int j=patternMStartY; j<collHeight; j++)
            {
                colliderMatrix[collX + (i-patternMStartX), collY + (j-patternMStartY)] = (colliderMatrix[collX + (i - patternMStartX), collY + (j - patternMStartY)] || patternMatrix[i, j]);
            }
        }
        
        //destroy previous colliders
        for(int i=0; i<colliders.Count; i++)
        {
            Destroy(colliders[i]);
        }
        colliders.Clear();

        int colliderSize = 0;
        int colliderStart = 0;

        List<ColliderData> collidersData = new List<ColliderData>();

        //calculate new colliders
        for(int i=0; i<colliderMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < colliderMatrix.GetLength(1); j++)
            {
                if (!colliderMatrix[i, j])
                {
                    if (colliderSize == 0) colliderStart = j;
                    colliderSize++;
                }
                else if (colliderSize > 0)
                {
                    collidersData.Add(new ColliderData(colliderStart, i, colliderSize, 1));
                    colliderSize = 0;
                }

                if(colliderSize>0 && j >= colliderMatrix.GetLength(1) - 1)
                {
                    collidersData.Add(new ColliderData(colliderStart, i, colliderSize, 1));
                    colliderSize = 0;
                }
            }

            colliderSize = 0;
            colliderStart = 0;
        }

        //simplify colliders data
        for(int i=0; i<collidersData.Count; i++)
        {
            for (int j = 0; j < collidersData.Count; j++)
            {
                if (i == j) continue;

                if (collidersData[i].Merge(collidersData[j])!=null)
                {
                    collidersData.RemoveAt(j);
                    i = 0;
                    break;
                }
            }
        }

        //create colliders
        foreach(ColliderData cd in collidersData)
        {
            CreateCollider(cd.colliderStartX, cd.colliderStartY, cd.colliderWidth, cd.colliderHeight);
        }

    }

    //Create collider objects
    private void CreateCollider(int colliderStartX, int colliderStartY, int colliderSizeX, int colliderSizeY)
    {
        float colliderScaleWidth = baseColliderBounds.size.x / colliderResY;
        float colliderScaleHeight = baseColliderBounds.size.y / colliderResX;

        GameObject newCollider = new GameObject();
        newCollider.name = "wall_collider";        
        newCollider.transform.rotation = transform.rotation;
        newCollider.transform.parent = this.gameObject.transform;
        newCollider.transform.localPosition = new Vector3(0, baseColliderBounds.extents.y / transform.localScale.y, 0);
        newCollider.transform.localRotation = Quaternion.Euler(180, 0, 270);
        BoxCollider collider = newCollider.AddComponent<BoxCollider>();
        collider.size = new Vector3(colliderSizeX * colliderScaleWidth, colliderSizeY * colliderScaleHeight, baseColliderBounds.size.z);
        collider.center = new Vector3((colliderStartX + colliderSizeX / 2f) * colliderScaleWidth-baseColliderBounds.extents.x,(colliderStartY + colliderSizeY / 2f) * colliderScaleHeight - baseColliderBounds.extents.y, 0);

        newCollider.AddComponent<WallColliderHandler>();

        colliders.Add(newCollider);
    }
    
}
