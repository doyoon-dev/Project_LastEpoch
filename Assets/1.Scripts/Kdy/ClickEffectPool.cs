using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushObject
{
    void PushObject();
}

public class ClickEffectPool : MonoBehaviour, IPushObject
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PushObject()
    {
        ObjectPool.Inst.Push<ClickEffectPool>(gameObject);
    }
}
