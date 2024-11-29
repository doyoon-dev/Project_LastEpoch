using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class Icon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Rotate(Vector3 pos)
    {
        StopAllCoroutines();
        StartCoroutine(Rotating(pos));
    }

    // È¸Àü
    public IEnumerator Rotating(Vector3 pos)
    {
        float angle = Vector3.Angle(-transform.right, pos);
        if (angle > 180.0f) angle -= 360.0f;
        float rotDir = 1.0f;
        if (Vector3.Dot(transform.right, pos) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (angle > 0.0f)
        {
            float delta = Time.deltaTime * 360.0f;
            if (delta > angle) delta = angle;
            angle -= delta;
            transform.Rotate(transform.forward * delta, Space.World);
            yield return null;
        }
    }
}
