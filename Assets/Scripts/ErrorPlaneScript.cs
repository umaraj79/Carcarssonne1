using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorPlaneScript : MonoBehaviour
{
    Material mat;
    bool ready = true;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
        mat.color = new Color(1, 0, 0, 0);
    }

    public void flash()
    {
        //StartCoroutine(FadeImage(false));
        //StartCoroutine(FadeImage(true));
        if(ready)StartCoroutine(FadeImage());
    }

    IEnumerator FadeImage()
    {
        ready = false;
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            mat.color = new Color(1, 0, 0, i);
            yield return null;
        }
        ready = true;
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        if (fadeAway)
        {
            for(float i = 1; i >= 0; i -= Time.deltaTime)
            {
                mat.color = new Color(1, 0, 0, i);
                yield return null;
            }
        }
        else
        {
            for(float i = 0; i <= 1; i+= Time.deltaTime)
            {
                mat.color = new Color(1, 0, 0, i);
                yield return null;
            }
        }
    }

    public void UpdatePosition(int x, int y)
    {
        x -= 85;
        y -= 85;

        x *= 2;
        y *= 2;
        transform.position = new Vector3(x, 0.1f, y);
    }
}
