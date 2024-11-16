using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneData : MonoBehaviour
{
    public static SceneData Inst = null;

    public PlayerUI m_playerHpMpUI;
    public Transform m_itemNameUIPos;
    public Inventory m_inventory;
    // 데미지 UI 관련 필드 추가
    public GameObject damageUIPrefab;
    public Transform canvasTransform; // 데미지 UI가 붙을 캔버스의 Transform

    // 헤드 헬스바 관련 필드 추가
    public GameObject headHealthBarPrefab;  // 모든 몬스터가 사용할 헤드 헬스바 프리팹
    public Transform headHealthBarParent;   // 헤드 헬스바가 위치할 부모 객체 (Canvas 하위)

    public GameClearUI m_gameClearUI;
    public PlayerStatUI m_playerStatUI;

    private void Awake()
    {
        Inst = this;
    }
}
