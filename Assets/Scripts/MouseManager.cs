using System;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Transform cameraRig;
    public Transform cameraObj;

    public float movementSpeed = 0.5f;
    public float movementTime = 5f; 
    public float zoomAmount = 100f;
    public float rotateAmount = 10f;

    public Vector2 limitZoom;

    private Vector2 limitPanX;
    private Vector2 limitPanZ;

    private Vector3 dragStart;
    private Vector3 rotateStart;
    private Vector3 rotateCurrent;
    private Vector3 zoomVector;

    private void Start()
    {
        zoomVector = new Vector3(0, -zoomAmount, zoomAmount);
        GameManager map_generator = transform.parent.GetComponentInChildren<GameManager>();
        Vector2 boundaries = map_generator.map_boundaries();
        limitPanX = new Vector2(0, boundaries.y);
        limitPanZ = new Vector2(0, boundaries.x);
    }

    void Update()
    {
        Vector3 pos = cameraRig.position;
        Vector3 zoom = cameraObj.localPosition;
        Quaternion rot = cameraRig.rotation;

        //mouse input
        //select tile
        if (Input.GetMouseButtonDown(0))
        {
            //only collide on layer 8 (tiles)
            int layerMask = 1 << 8;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                GameManager gm = FindObjectOfType<GameManager>();
                Tile t = hit.collider.gameObject.GetComponentInChildren<Tile>();
                Debug.Log("Tile type: " + t._type + "\npos: " + t._coordinateWidth + ", " + t._coordinateHeight);
                gm.TileClicked(t._coordinateHeight, t._coordinateWidth);
            }
        }

        //zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            zoom += Input.mouseScrollDelta.y * zoomVector;
        }

        // pan
        if (Input.GetMouseButtonDown(1))
        {
            // remember initial mouse pos
            dragStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(1))
        {
            // cast ray to initial mouse pos to get center point
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(dragStart);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                Vector3 center = ray.GetPoint(entry);
                // cast second ray to current mouse pos
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (plane.Raycast(ray, out entry))
                {
                    // calculate pan vector and scale by speed
                    pos += (ray.GetPoint(entry) - center) * movementSpeed;
                }
            }
        }

        //rotate
        if (Input.GetMouseButtonDown(2))
        {
            rotateStart = Input.mousePosition;
        }
        if (Input.GetMouseButton(2))
        {
            rotateCurrent = Input.mousePosition;

            Vector3 rotateDiff = (rotateStart - rotateCurrent) * rotateAmount;
            rotateStart = rotateCurrent;

            rot *= Quaternion.Euler(Vector3.up * (-rotateDiff.x / 5f));
        }

        pos = new Vector3(Mathf.Clamp(pos.x, limitPanX.x, limitPanX.y), pos.y,
                          Mathf.Clamp(pos.z, limitPanZ.x, limitPanZ.y));

        zoom = new Vector3(zoom.x, Mathf.Clamp(zoom.y, limitZoom.x, limitZoom.y),
                          Mathf.Clamp(zoom.z, -limitZoom.y, -limitZoom.x));

        cameraRig.position = Vector3.Lerp(cameraRig.position, pos, Time.deltaTime * movementTime);
        cameraObj.localPosition = Vector3.Lerp(cameraObj.localPosition, zoom, Time.deltaTime * movementTime);
        cameraRig.rotation = Quaternion.Lerp(cameraRig.rotation, rot, Time.deltaTime * movementTime);
    }
}
