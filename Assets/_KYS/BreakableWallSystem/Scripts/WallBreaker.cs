using UnityEngine;
using System.Collections;

public class WallBreaker : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

        //Test wall breaking
        if (Input.GetKeyDown(KeyCode.Alpha1)) Break1();
        if (Input.GetKeyDown(KeyCode.Alpha2)) Break2();
        if (Input.GetKeyDown(KeyCode.Alpha3)) Break3();
        if (Input.GetKeyDown(KeyCode.Alpha4)) Break4();

    }

    private void Break1()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            WallColliderHandler bw = hit.transform.GetComponent<WallColliderHandler>();
            if (bw != null)
            {
                bw.BreakWall(1, ray);
            }
        }
    }

    private void Break2()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            WallColliderHandler bw = hit.transform.GetComponent<WallColliderHandler>();
            if (bw != null)
            {
                bw.BreakWall(2, ray);
            }
        }
    }

    private void Break3()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            WallColliderHandler bw = hit.transform.GetComponent<WallColliderHandler>();
            if (bw != null)
            {
                bw.BreakWall(3, ray);
            }
        }
    }

    private void Break4()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            WallColliderHandler bw = hit.transform.GetComponent<WallColliderHandler>();
            if (bw != null)
            {
                bw.BreakWall(4, ray);
            }
        }
    }
}
