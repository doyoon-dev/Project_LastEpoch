using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    [Header("아이템 프리팹 목록 (흔한)")]
    [SerializeField]
    private GameObject[] lowRangeItemPrefabs; 

    [Header("아이템 프리팹 목록 (중간)")]
    [SerializeField]
    private GameObject[] midRangeItemPrefabs; 

    [Header("아이템 프리팹 목록 (희귀)")]
    [SerializeField]
    private GameObject[] highRangeItemPrefabs; 

    [Header("아이템 드롭 확률 목록 (흔한 확률)")]
    [SerializeField]
    private float[] lowRangeDropChances; 
    [Header("아이템 드롭 확률 목록 (중간 확률)")]
    [SerializeField]
    private float[] midRangeDropChances; 

    [Header("아이템 드롭 확률 목록 (희귀 확률)")]
    [SerializeField]
    private float[] highRangeDropChances; 

    [Header("드롭 확률 범위 (흔한)")]
    [SerializeField]
    private Vector2 lowRange = new Vector2(1, 31);
    [SerializeField]
    private Vector2Int lowRangeItemCount = new Vector2Int(1, 4); //최대아이템 개수

    [Header("드롭 확률 범위 (중간)")]
    [SerializeField]
    private Vector2 midRange = new Vector2(31, 71);
    [SerializeField]
    private Vector2Int midRangeItemCount = new Vector2Int(1, 3);  //최대아이템 개수

    [Header("드롭 확률 범위 (희귀)")]
    [SerializeField]
    private Vector2 higRange = new Vector2(71, 100);
    [SerializeField]
    private Vector2Int higRangeItemCount = new Vector2Int(1, 2); //최대아이템 개수

    // 아이템 드롭 메서드
    public void DropItems(Vector3 position)
    {
        //낮은 확률 드롭할 아이템 개수
        int lowRangeItems = Random.Range(lowRangeItemCount.x, lowRangeItemCount.y + 1);
        //중간 확률 드롭할 아이템 개수
        int midRangeItems = Random.Range(midRangeItemCount.x, midRangeItemCount.y + 1);
        //높은 확률 드롭할 아이템 개수
        int highRangeItems = Random.Range(higRangeItemCount.x, higRangeItemCount.y + 1);

        // 흔한 확률로 아이템 드롭
        for (int i = 0; i < lowRangeItems; i++)
        {
            DropItemPer(position, lowRangeItemPrefabs, lowRangeDropChances);
        }
        // 중간 확률로 아이템 드롭
        for (int i = 0; i < midRangeItems; i++)
        {
            DropItemPer(position, midRangeItemPrefabs, midRangeDropChances);
        }
        // 희귀 확률로 아이템 드롭
        for (int i = 0; i < highRangeItems; i++)
        {
            DropItemPer(position, highRangeItemPrefabs, highRangeDropChances);
        }
    }

    // 특정 확률 범위에서 아이템 무작위 드롭
    private void DropItemPer(Vector3 position, GameObject[] itemPrefabs, float[] dropChances)
    {
        for (int i = 0; i < itemPrefabs.Length; i++)
        {
            float dropChance = dropChances[i];
            if (Random.Range(0f, 100f) <= dropChance)
            {
                GameObject dropItemObject = ObjectPool.Inst.Pool<DropItem>(itemPrefabs[i]);
                DropItem dropItem = dropItemObject.GetComponent<DropItem>();
                dropItem.transform.position = position;
                dropItem.gameObject.SetActive(true);

                // 아이템 살짝 튀어오르게 하는거 설정
                Vector3 launchForce = new Vector3(Random.Range(-1f, 1f), Random.Range(4f, 6f), Random.Range(-1f, 1f));
                dropItem.Launch(launchForce);

               
            }
        }
    }
}