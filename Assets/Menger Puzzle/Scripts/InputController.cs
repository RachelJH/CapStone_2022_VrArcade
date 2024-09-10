using UnityEngine;
using UnityEngine.UI;

// Getting user input and communicating with the WallRotation class.

public class InputController : MonoBehaviour
{
    public static bool allowed = true;

    [Header("Buttons:")]
    [SerializeField] private Button changeDirectionBtn;
    [SerializeField] private Button[] wallsButtons;     // possible buttons that rotate the walls as an alternative to the mouse
    
    [Header("Walls:")]                                  // suitable wall containers
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject centerLR;
    [SerializeField] private GameObject right;
    [SerializeField] private GameObject up;
    [SerializeField] private GameObject centerUD;
    [SerializeField] private GameObject down;
    [SerializeField] private GameObject front;
    [SerializeField] private GameObject centerFB;
    [SerializeField] private GameObject back;

    private WallRotation rr;
    private int direction = 1;
    private int index = -1;

    private void Awake() => AddButtonsListeners();

    private void AddButtonsListeners()
    {
        rr = GetComponent<WallRotation>();

        for (int i = 0; i < wallsButtons.Length; i++)
        {
            int j = i;
            wallsButtons[i].onClick.AddListener(delegate { index = j; });
        }

        if(changeDirectionBtn != null)
            changeDirectionBtn.onClick.AddListener(delegate { direction *= -1; });
    }
    
    private void Update()
    {
        if (CubeComposition.hashing)
            GetRandomWall();
        else
            GetWall(-1);
    }

    private void GetRandomWall()
    {
        if (allowed)
        {
            allowed = false;
            GetWall(Random.Range(0, 9));
        }
    }

    private void GetWall(int value)
    {
        GameObject actualWall = null;
        Vector3 axis = Vector3.zero;

        if (value != -1)
            index = value;

        switch(index)
        {
            case 0:
                axis = -Vector3.right;
                actualWall = left;
                break;
            case 1:
                axis = Vector3.right;
                actualWall = right;
                break;
            case 2:
                axis = Vector3.right;
                actualWall = centerLR;
                break;
            case 3:
                axis = Vector3.up;
                actualWall = up;
                break;
            case 4:
                axis = -Vector3.up;
                actualWall = down;
                break;
            case 5:
                axis = Vector3.up;
                actualWall = centerUD;
                break;
            case 6:
                axis = -Vector3.forward;
                actualWall = front;
                break;
            case 7:
                axis = Vector3.forward;
                actualWall = back;
                break;
            case 8:
                axis = -Vector3.forward;
                actualWall = centerFB;
                break;
        }
        
        if (index > -1)
            rr.RotateCube(direction, axis, actualWall);

        index = -1;
    }
}