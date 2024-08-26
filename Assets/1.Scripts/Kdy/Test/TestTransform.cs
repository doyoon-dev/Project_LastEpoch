using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTransform : MonoBehaviour
{
    RectTransform rectTransform;
    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetMouseButtonDown(0))
        //{
        //    Debug.Log("마우스 포지션 : " + Input.mousePosition);
        //    Debug.Log("슬롯 포지션 : " + rectTransform.position);
        //    Debug.Log("그리드 좌표 : " + "( " + (Input.mousePosition.x - rectTransform.position.x) + " , " + (Input.mousePosition.y - rectTransform.position.y) + " )");
        //}
    }
}
