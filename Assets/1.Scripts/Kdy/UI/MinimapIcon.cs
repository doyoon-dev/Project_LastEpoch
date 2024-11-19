using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MinimapIcon : MonoBehaviour
{
    public Image m_icon;
    Transform m_target;
    float m_size;
    Vector3 m_posForward;
    // Start is called before the first frame update
    void Start()
    {
        m_posForward = new Vector3(-1, 0, 0);
        RectTransform rt = SceneData.Inst.m_minimap as RectTransform;
        if (rt != null)
        {
            m_size = rt.sizeDelta.x;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_target != null)
        {
            Vector2 pos = Camera.allCameras[1].WorldToViewportPoint(m_target.position) * m_size;
            (transform as RectTransform).anchoredPosition = pos;
        }
    }

    public void Initialize(Transform target)
    {
        m_target = target;
        // 플레이어, 토템에 public Image Icon 변수 만들어서 아이콘 바꾸기
        // m_icon = target.Icon;
    }

    public void Rotate(Vector3 pos)
    {
        StopAllCoroutines();
        StartCoroutine(Rotating(pos));
    }

    // 회전
    public IEnumerator Rotating(Vector3 pos)
    {
        float angle = Vector3.Angle(m_posForward, pos);
        if (angle > 180.0f) angle -= 360.0f;
        float rotDir = 1.0f;
        if (Vector3.Dot(m_icon.rectTransform.up, pos) < 0.0f)
        {
            rotDir = -1.0f;
        }

        while (angle > 0.0f)
        {
            float delta = Time.deltaTime * 360.0f;
            if (delta > angle) delta = angle;
            angle -= delta;
            m_icon.rectTransform.Rotate(m_icon.rectTransform.forward * rotDir * delta, Space.World);
            yield return null;
        }
    }
}
