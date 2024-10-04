using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    static ObjectPool m_inst = null;
    public static ObjectPool Inst
    {
        get
        {
            if (m_inst == null)
            {
                m_inst = FindObjectOfType<ObjectPool>();
                if (m_inst == null)
                {
                    GameObject obj = new GameObject("ObjectPool");
                    m_inst = obj.AddComponent<ObjectPool>();
                }
            }
            return m_inst;
        }
    }

    Dictionary<string, Queue<GameObject>> m_myPool = new Dictionary<string, Queue<GameObject>>();

    // 오브젝트 풀에서 꺼내기
    public GameObject Pull<T>(GameObject org, Transform parent = null)
    {
        string name = typeof(T).Name;
        if (m_myPool.ContainsKey(name))
        {
            if (m_myPool[name].Count > 0)
            {
                GameObject obj = m_myPool[name].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(parent);
                return obj;

                // **오브젝트 위치, 회전 초기화** // 메서드에서 풀에 있는 오브젝트를 꺼낼 때, 그 오브젝트가 이전 상태(위치, 회전, 스케일 등)를 그대로 유지한 채로 활성화 되서 추가함
                obj.transform.localPosition = Vector3.zero;//(성원)
                obj.transform.localRotation = Quaternion.identity;//(성원)
                obj.transform.localScale = Vector3.one;//(성원)

                return obj;
            }
        }
        return Instantiate(org, parent);
    }

    // 오브젝트 풀에 넣기
    public void Push<T>(GameObject obj)
    {
        obj.SetActive(false);

        // **오브젝트 위치, 회전 초기화** // 메서드에서 풀에 있는 오브젝트를 꺼낼 때, 그 오브젝트가 이전 상태(위치, 회전, 스케일 등)를 그대로 유지한 채로 활성화 되서 추가함
        obj.transform.SetParent(null); // 풀로 반환되므로 부모 제거(성원)
        obj.transform.localPosition = Vector3.zero;//(성원)
        obj.transform.localRotation = Quaternion.identity;//(성원)
        obj.transform.localScale = Vector3.one;//(성원)

        string name = typeof(T).Name;
        if (!m_myPool.ContainsKey(name))
        {
            m_myPool[name] = new Queue<GameObject>();
        }
        m_myPool[name].Enqueue(obj);
    }
}
