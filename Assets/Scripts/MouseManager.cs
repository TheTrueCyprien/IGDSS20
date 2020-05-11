using System;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Transform cameraRig;
    public Transform cameraObj;

    public float movementSpeed;
    public float movementTime;
    public float zoomAmount;
    public float rotateAmount;

    public Vector2 limitPanX;
    public Vector2 limitPanZ;
    public Vector2 limitZoom;

    private Vector3 dragStart;
    private Vector3 dragCurrent;
    private Vector3 rotateStart;
    private Vector3 rotateCurrent;
    private Vector3 zoomVector;

    private void Start()
    {
        zoomVector = new Vector3(0, -zoomAmount, zoomAmount);
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
                Debug.Log("Clicked on: " + hit.collider.name);
            }
        }

        //zoom
        if (Input.mouseScrollDelta.y != 0)
        {
            zoom += Input.mouseScrollDelta.y * zoomVector;
        }

        //pan
        if (Input.GetMouseButtonDown(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStart = ray.GetPoint(entry);
            }
        }
        if (Input.GetMouseButton(1))
        {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrent = ray.GetPoint(entry);
                pos += dragStart - dragCurrent;
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
