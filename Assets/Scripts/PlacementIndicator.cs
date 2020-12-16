using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlacementIndicator : MonoBehaviour
{
    private ARRaycastManager rayManager;
    private GameObject visual;
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

            if (!visual.activeInHierarchy)
            {
                visual.SetActive(true);
            }
        }
    }

    public Vector3 GetPosition()
    {
        if (hits.Count > 0)
        {
            return hits[0].pose.position;
        }
        else return new Vector3(-1,-1,-1);
    }
    public Quaternion GetRotation()
    {
        if (hits.Count > 0)
        {
            return hits[0].pose.rotation;
        }
        else return new Quaternion();
    }

}
