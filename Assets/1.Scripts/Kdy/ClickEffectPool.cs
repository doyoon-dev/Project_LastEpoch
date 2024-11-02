using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPushObject
{
    void PushObject();
}

public class ClickEffectPool : MonoBehaviour, IPushObject
{
    public void PushObject()
    {
        ObjectPool.Inst.Push<ClickEffectPool>(gameObject);
    }
}
