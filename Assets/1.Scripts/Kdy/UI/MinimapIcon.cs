using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public interface IDestroyObj
{
    void DestroyObj();
}

public class MinimapIcon : MonoBehaviour, IDestroyObj
{
    public Sprite[] m_icons;
    public Image m_icon;
    public Transform m_target;
    public float m_size;
    public LayerMask m_moveMask;
    public LayerMask m_enemyMask;

    // Start is called before the first frame update
    void Start()
    {
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
            m_icon.rectTransform.localEulerAngles = new Vector3(0, 0, -m_target.localEulerAngles.y);
        }
    }

    public void Initialize(Transform target, int i)
    {
        m_target = target;
        // 플레이어, 토템에 public Image Icon 변수 만들어서 아이콘 바꾸기
        // 플레이어 아이콘
        if (i == 0)
        {
            m_icon.sprite = m_icons[0];
            m_icon.SetNativeSize();
        }
        // 토템 아이콘
        else
        {
            m_icon.sprite = m_icons[1];
            m_icon.rectTransform.sizeDelta = new Vector2(50, 50);
            m_icon.color = Color.red;
        }
    }

    public void DestroyObj()
    {
        Destroy(gameObject);
    }
}
