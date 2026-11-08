using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;

//using Unity.VisualScripting.Dependencies.Sqlite;
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
    Dictionary<string, Transform> m_myExplore = new Dictionary<string, Transform>();

    // 오브젝트 풀에서 꺼내기
    public GameObject Pull<T>(GameObject org, Transform parent = null)
    {
        string name = $"{typeof(T).Name}_{org.name.Replace("(Clone)", "")}";
        if (m_myPool.ContainsKey(name))
        {
            if (m_myPool[name].Count > 0)
            {
                GameObject obj = m_myPool[name].Dequeue();
                obj.SetActive(true);
                obj.transform.SetParent(parent);

                return obj;
            }
        }
        return Instantiate(org, parent);
    }

    // 오브젝트 풀에 넣기
    public void Push<T>(GameObject obj)
    {
        obj.SetActive(false);
        string name = $"{typeof(T).Name}_{obj.name.Replace("(Clone)", "")}";

        if (!m_myPool.ContainsKey(name))
        {
            m_myPool[name] = new Queue<GameObject>();
            GameObject directory = new GameObject(name);
            m_myExplore[name] = directory.transform;
        }

        obj.transform.SetParent(m_myExplore[name]);

        m_myPool[name].Enqueue(obj);
    }
}
