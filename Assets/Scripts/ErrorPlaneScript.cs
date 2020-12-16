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

    public void flashError()
    {
        //StartCoroutine(FadeImage(false));
        //StartCoroutine(FadeImage(true));
        if (ready) StartCoroutine(FadeImage(1, 0));
    }

    public void flashConfirm()
    {
        if (ready) StartCoroutine(FadeImage(0, 1));
    }

    IEnumerator FadeImage(int red, int green)
    {
        ready = false;
        for (float i = 1; i >= 0; i -= Time.deltaTime * 2)
        {
            mat.color = new Color(red, green, 0, i);
            yield return null;
        }
        ready = true;
    }

    IEnumerator FadeImage(bool fadeAway)
    {
        if (fadeAway)
        {
            for (float i = 1; i >= 0; i -= Time.deltaTime)
            {
                mat.color = new Color(1, 0, 0, i);
                yield return null;
            }
        }
        else
        {
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                mat.color = new Color(1, 0, 0, i);
                yield return null;
            }
        }
    }

    public void UpdatePosition(int x, int y, int z)
    {
        if (ready)
        {
            x -= 85;
            z -= 85;

            x *= 2;
            z *= 2;
            transform.position = new Vector3(x, y + 0.1f, z);
        }
    }
}
