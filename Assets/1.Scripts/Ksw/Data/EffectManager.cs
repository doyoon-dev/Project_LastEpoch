using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager>
{
    public List<EffectData> effectDataList; //이펙트 데이터

    private Dictionary<string, Queue<GameObject>> effectPool = new Dictionary<string, Queue<GameObject>>();

    protected override void OnAwake()
    {
        InitializeEffectPool();
    }

    // 이펙트 풀을 초기화하는 메소드
    // 각 이펙트를 미리 생성하여 큐에 추가해두어, 필요할 때 빠르게 사용 가능하게 함
    private void InitializeEffectPool()
    {
        foreach (var effectData in effectDataList)
        {
            // 각 이펙트 이름을 키로 하여 큐를 생성
            effectPool[effectData.effectName] = new Queue<GameObject>();

            // 기본적으로 3개 생성하여 풀에 추가
            for (int i = 0; i < 2; i++)
            {
                GameObject obj = Instantiate(effectData.prefab);
                obj.transform.localScale = effectData.defaultScale; // 이펙트의 기본 크기 설정
                obj.SetActive(false);                               // 풀에 넣기 전 비활성화
                effectPool[effectData.effectName].Enqueue(obj);     // 큐에 추가하여 풀에 저장
            }
        }
    }

   
    // 이펙트를 요청하여 반환하는 메소드
    public GameObject GetEffect(string effectName, Vector3 position, Quaternion rotation)
    {
        // 해당 이펙트가 풀에 존재하지 않으면 경고 메시지 출력
        if (!effectPool.ContainsKey(effectName))
        {
            Debug.LogWarning($"Effect '{effectName}' 이펙트 없음.");
            return null;
        }

        // 풀에 사용 가능한 이펙트가 없으면 새로 생성하여 추가
        if (effectPool[effectName].Count == 0)
        {
            var data = effectDataList.Find(e => e.effectName == effectName);
            if (data != null)
            {
                GameObject newEffect = Instantiate(data.prefab);       // 이펙트 프리팹 생성
                newEffect.transform.localScale = data.defaultScale;    // 기본 크기 설정
                newEffect.SetActive(false);                            // 풀에 넣기 전 비활성화
                effectPool[effectName].Enqueue(newEffect);             // 큐에 추가하여 풀에 저장
            }
        }

        // 큐에서 이펙트를 꺼내 위치와 회전 값을 설정하고 활성화
        GameObject effect = effectPool[effectName].Dequeue();
        effect.transform.position = position;
        effect.transform.rotation = rotation;
        effect.SetActive(true);

        // 이펙트의 유지 시간을 `EffectData`에서 가져와 코루틴을 통해 풀로 반환
        var effectData = effectDataList.Find(e => e.effectName == effectName);
        if (effectData != null)
        {
            if (effectData.duration > 0)
            {
                StartCoroutine(ReturnEffectAfterTime(effect, effectName, effectData.duration));
            }
            else
            {
                Debug.LogWarning($"Effect '{effectName}'의 유지 시간이 설정되지 않았습니다.");
            }
        }

        return effect; // 활성화된 이펙트를 반환하여 외부에서 지속 시간 제어 가능
    }

    // 설정된 시간이 지나면 이펙트를 비활성화하고 풀에 되돌리는 메소드
    private IEnumerator ReturnEffectAfterTime(GameObject effect, string effectName, float duration)
    {
        yield return new WaitForSeconds(duration);

        // 이펙트를 비활성화하고 풀에 되돌림
        effect.SetActive(false);
        effectPool[effectName].Enqueue(effect);
    }
}