using UnityEngine;
using System.Collections;

public class WallColliderHandler : MonoBehaviour {

    private BreakableWall parent;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //Transmit events to the parent
    public void BreakWall(int breakType, Ray ray)
    {
        if(parent==null) parent = transform.parent.GetComponent<BreakableWall>();

        if (parent != null) parent.BreakWall(breakType, ray);
    }
}
