using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEffectData", menuName = "Effect/EffectData")]
public class EffectData : ScriptableObject
{
    public string effectName;         // 이펙트 이름
    public GameObject prefab;         // 이펙트 프리팹
    public Vector3 defaultScale = Vector3.one;   // 기본 크기
    public float duration;      // 기본 유지 시간 (초)

}
