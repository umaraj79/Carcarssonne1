using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;
    private GameObject visual;
    private Vector3 BasePosition;
    public Quaternion BaseRotation;
    private Vector3 lastValidPosition;
    List<ARRaycastHit> hits;

    // Start is called before the first frame update
    void Start()
    {
        rayManager = FindObjectOfType<ARRaycastManager>();
        visual = transform.GetChild(0).gameObject;

        visual.SetActive(false);
    }

    // Update is called once per frame
    public void Render()
    {
        hits = new List<ARRaycastHit>();
        rayManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits, TrackableType.Planes);
        if (hits.Count > 0)
        {
            transform.position = hits[0].pose.position;
            transform.rotation = hits[0].pose.rotation;
            BaseRotation = transform.rotation;

            if (!visual.activeInHierarchy)
            {
                visual.SetActive(true);
            }
        }
    }

    public Quaternion GetRotation()
    {
        return BaseRotation;
    }

    public Vector3 PlaceBase()
    {
        transform.position = hits[0].pose.position;
        BasePosition = hits[0].pose.position;
        return hits[0].pose.position;
    }

    public Vector3 GetBasePosition()
    {
        return BasePosition;
    }

    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if(Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(0).position;
            return true;
        }

        touchPosition = default;
        return false;
    }

    public float GetXPosition()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return lastValidPosition.x;
        if (rayManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            lastValidPosition.x = hits[0].pose.position.x;
        }
        return lastValidPosition.x;
    }
    public float GetZPosition()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return lastValidPosition.z;
        if (rayManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            lastValidPosition.z = hits[0].pose.position.z;
        }
        return lastValidPosition.z;
    }

    public Vector3 GetPosition()
    {
        if (hits.Count > 0)
        {
            lastValidPosition =  hits[0].pose.position;
        }
        return lastValidPosition;
    }

}
