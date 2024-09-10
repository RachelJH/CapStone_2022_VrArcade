using UnityEngine;
using UnityEngine.UI;

// Mixing the sponge and detecting its correct arrangement.

public class CubeComposition : MonoBehaviour
{
    [SerializeField] private Text scoreTxt;
    [SerializeField] private GameObject successTxt;
    [SerializeField] private Button hashBtn;

    public static bool hashing;

    private GameObject[] surfaces;              // 54 fields
    private Color[] lastWallColor;              // last getted color of a given wall
    private bool wellArranged;
    private int steps;

    private void Awake ()
    {
        WallRotation.OnWallRotated += WallRotation_OnWallRotated;
        hashBtn.onClick.AddListener(MixCube);
        surfaces = GameObject.FindGameObjectsWithTag("Surface");
    }

    private void OnDestroy ()
    {
        WallRotation.OnWallRotated -= WallRotation_OnWallRotated;
        hashBtn.onClick.RemoveListener(MixCube);
    }

    private void MixCube()      // mixing cube
    {
        hashing = !hashing;

        if (successTxt.activeSelf)
        {
            steps = 0;
            scoreTxt.text = "Moves: 0";
        }

        successTxt.SetActive(false);
    }
    
    private void WallRotation_OnWallRotated ()      // detection of correct cube arrangement and update of movement statistics
    {
        UpdateSteps();
        DetectComposition();
    }

    private void UpdateSteps()
    {
        if(!hashing)
            if(InputController.allowed)
                scoreTxt.text = "Moves: " + (++steps).ToString();
    }

    private void DetectComposition()    // check whether the cube is placed correctly
    {
        lastWallColor = new Color[9];
        wellArranged = true;

        foreach (GameObject surface in surfaces)        // we examine the color of the fields of successive walls
        {
            if (!wellArranged)  // if an incorrect arrangement of any wall is detected
                break;

            if (surface.transform.position.x >=  1.45f)  wellArranged = IsWallComposed(0, surface);
            if (surface.transform.position.x <= -1.45f)  wellArranged = IsWallComposed(1, surface);
            if (surface.transform.position.y >=  1.45f)  wellArranged = IsWallComposed(2, surface);
            if (surface.transform.position.y <= -1.45f)  wellArranged = IsWallComposed(3, surface);
            if (surface.transform.position.z >=  1.45f)  wellArranged = IsWallComposed(4, surface);
            if (surface.transform.position.z <= -1.45f)  wellArranged = IsWallComposed(5, surface);
        }

        if (wellArranged)                // properly arranged
        {
            successTxt.SetActive(true);
            SoundManager.GetSoundEffect(2, 1f);
        }
    }

    private bool IsWallComposed(int index, GameObject surface)
    {
        Color currentColor = surface.GetComponent<MeshRenderer>().material.color;

        if (lastWallColor[index] == Color.clear)
            lastWallColor[index] = currentColor;
        else if (lastWallColor[index] != currentColor)    // wall not properly arranged
            return false;

        return true;
    }
}