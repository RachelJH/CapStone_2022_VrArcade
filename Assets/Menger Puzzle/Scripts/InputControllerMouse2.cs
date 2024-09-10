using UnityEngine;

// Getting user mouse input and communicating with the WallRotation class.
// The mechanism is based on the concept of a container, which is assigned to the wall of the cube indicated by the mouse cursor.
// Its position and scale is determined by the surface you clicked and the next surface over which the mouse cursor appears.
// All surfaces within its volume that are to rotate are assigned to it and then rotate with it (see WallActivation class).


namespace BNG {
    public class InputControllerMouse2 : MonoBehaviour
    {
        [SerializeField] private GameObject floatingWall;
        private BoxCollider floatingCol;
        protected InputBridge input;
        public Transform handPos;
        private WallRotation wr;
        private bool wallAllowed;
        private GameObject temp;
        private GameObject first;                   // clicked surface
        private GameObject second;                  // nearest surface where the mouse cursor appears while dragging 

        private void Awake()
        {
            wr = GetComponent<WallRotation>();
            floatingCol = floatingWall.GetComponent<BoxCollider>();
            input = InputBridge.Instance;

        }

        private void Update() => GetMouseMove();

        private void GetMouseMove()
        {
            
            
            if (Input.GetKeyDown("z"))
                {
                    temp = null;
                    second = null;
                    wallAllowed = false;

                    DetectFirstSurface();               // detect the clicked surface (first surface)
                }

            if (Input.GetKey("z"))
                DetectSecondSurface();              // detect the second surface

            if (Input.GetKeyUp("z") && second != null)
                wallAllowed = true;

            ////////////////
            if (Input.GetMouseButtonDown(0))
            {
                temp = null;
                second = null;
                wallAllowed = false;

                DetectFirstSurface();               // detect the clicked surface (first surface)
            }

            if (Input.GetMouseButton(0))
                DetectSecondSurface();              // detect the second surface

            if (Input.GetMouseButtonUp(0) && second != null)
                wallAllowed = true;

            //////////
            ///
            
            if (input.RightTriggerDown)
            {
                temp = null;
                second = null;
                wallAllowed = false;

                DetectFirstSurface();               // detect the clicked surface (first surface)
            }

            if (input.RightTrigger == 1)
                DetectSecondSurface();              // detect the second surface

            if (input.RightTriggerUp && second != null)
                wallAllowed = true;
            
            ////////////////
            if (wallAllowed)
                DetermineRotation();
        }

        private void DetectFirstSurface()
        {
            Ray ray = new Ray(handPos.position, handPos.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 10))
            {
                Collider col = hit.collider;

                if (col.CompareTag("Surface"))
                    first = col.gameObject;         // clicked surface
            }
        }

        private void DetectSecondSurface()
        {
            if (second == null)
            {
                Ray ray = new Ray(handPos.position, handPos.forward);

                if (Physics.Raycast(ray, out RaycastHit hit, 10))
                {
                    Collider col = hit.collider;

                    if (col.CompareTag("Surface"))
                    {
                        if (temp != col.gameObject && temp != null)
                            second = col.gameObject;                    // the second surface while dragging

                        temp = col.gameObject;
                    }
                }
            }
        }

        private void DetermineRotation()    // we determine the rotations on the basis of two key surfaces (first and second)
        {
            int direction = 1;
            Vector3 axis = Vector3.zero;
            wallAllowed = false;

            int colX = 3;
            int colY = 3;
            int colZ = 3;

            float posX = 0;
            float posY = 0;
            float posZ = 0;

            float firstX = first.transform.position.x;
            float firstY = first.transform.position.y;
            float firstZ = first.transform.position.z;

            float secondX = second.transform.position.x;
            float secondY = second.transform.position.y;
            float secondZ = second.transform.position.z;

            float dX = firstX - secondX;
            float dY = firstY - secondY;
            float dZ = firstZ - secondZ;

            float xAbs = Mathf.Abs(dX);
            float yAbs = Mathf.Abs(dY);
            float zAbs = Mathf.Abs(dZ);

            void GetValues(Vector3 vector, float pos, int col, float d, int a, ref float poss, ref int coll)
            {
                axis = vector;
                poss = pos;
                coll = col;
                direction = (d > 0) ? a : -a;
            }

            if (xAbs < 0.2f && yAbs < 0.2f && firstX > 1.43f) GetValues(Vector3.up, firstY, 1, dZ, 1, ref posY, ref colY);        // green
            else if (xAbs < 0.2f && zAbs < 0.2f && firstX > 1.43f) GetValues(Vector3.forward, firstZ, 1, dY, -1, ref posZ, ref colZ);
            else if (xAbs < 0.2f && yAbs < 0.2f && firstX < -1.43f) GetValues(Vector3.up, firstY, 1, dZ, -1, ref posY, ref colY);       // blue
            else if (xAbs < 0.2f && zAbs < 0.2f && firstX < -1.43f) GetValues(Vector3.forward, firstZ, 1, dY, 1, ref posZ, ref colZ);
            else if (yAbs < 0.2f && zAbs < 0.2f && firstZ > 1.43f) GetValues(Vector3.up, firstY, 1, dX, -1, ref posY, ref colY);       // orange
            else if (xAbs < 0.2f && zAbs < 0.2f && firstZ > 1.43f) GetValues(Vector3.right, firstX, 1, dY, 1, ref posX, ref colX);
            else if (yAbs < 0.2f && zAbs < 0.2f && firstZ < -1.43f) GetValues(Vector3.up, firstY, 1, dX, 1, ref posY, ref colY);        // red
            else if (xAbs < 0.2f && zAbs < 0.2f && firstZ < -1.43f) GetValues(Vector3.right, firstX, 1, dY, -1, ref posX, ref colX);
            else if (xAbs < 0.2f && yAbs < 0.2f && firstY > 1.43f) GetValues(Vector3.right, firstX, 1, dZ, -1, ref posX, ref colX);    // white
            else if (yAbs < 0.2f && zAbs < 0.2f && firstY > 1.43f) GetValues(Vector3.forward, firstZ, 1, dX, 1, ref posZ, ref colZ);
            else if (xAbs < 0.2f && yAbs < 0.2f && firstY < -1.43f) GetValues(Vector3.right, firstX, 1, dZ, 1, ref posX, ref colX);     // yellow
            else if (yAbs < 0.2f && zAbs < 0.2f && firstY < -1.43f) GetValues(Vector3.forward, firstZ, 1, dX, -1, ref posZ, ref colZ);
            //------------------------------------------------------------ dragging with the change of faces
            else if (xAbs < 0.2f && yAbs > 0.2f && zAbs > 0.2f)         // white-orange
            {
                if (secondZ < -1.43f || firstZ < -1.43f) GetValues(Vector3.right, firstX, 1, dY, -1, ref posX, ref colX);
                else if (secondZ > 1.43f || firstZ > 1.43f) GetValues(Vector3.right, firstX, 1, dY, 1, ref posX, ref colX);
            }
            else if (xAbs > 0.2f && yAbs > 0.2f && zAbs < 0.2f)         // white-green
            {
                if (secondX > 1.43f || firstX > 1.43f) GetValues(Vector3.forward, firstZ, 1, dY, -1, ref posZ, ref colZ);
                else if (secondX < -1.43f || firstX < -1.43f) GetValues(Vector3.forward, firstZ, 1, dY, 1, ref posZ, ref colZ);
            }
            else if (xAbs > 0.2f && yAbs < 0.2f && zAbs > 0.2f)         // orange-green
            {
                if ((secondX > 1.43f && firstZ < -1.43f) || (secondZ < -1.43f && firstX > 1.43f)) GetValues(Vector3.up, firstY, 1, dX, 1, ref posY, ref colY);
                else if ((secondX < -1.43f && firstZ > 1.43f) || (secondZ > 1.43f && firstX < -1.43f)) GetValues(Vector3.up, firstY, 1, dX, -1, ref posY, ref colY);
                else if ((secondZ > 1.43f && firstX > 1.43f) || (secondX > 1.43f && firstZ > 1.43f)) GetValues(Vector3.up, firstY, 1, dX, -1, ref posY, ref colY);
                else if ((secondZ < -1.43f && firstX < -1.43f) || (secondX < -1.43f && firstZ < -1.43f)) GetValues(Vector3.up, firstY, 1, dX, 1, ref posY, ref colY);
            }

            if (!(colX == colY && colX == colZ))                        // if the face is not a cube
            {
                floatingWall.transform.rotation = Quaternion.identity;
                floatingWall.transform.position = new Vector3(posX, posY, posZ);
                floatingCol.size = new Vector3(colX, colY, colZ);

                wr.RotateCube(direction, axis, floatingWall);
            }
        }
    }
}