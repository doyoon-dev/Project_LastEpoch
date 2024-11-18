using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayFire;
using UnityEngine.UIElements.Experimental;

public class Totem : BattleSystem
{


    private RayfireRigid rayfireRigid;
    // Start is called before the first frame update
    public WaypointController assignedWaypoint;// 토템이 속한 웨이포인트 지정

    public HeadHealthBar headHealthBar;

    void Start()
    {       
        rayfireRigid = GetComponent<RayfireRigid>();
        rayfireRigid.simulationType = SimType.Dynamic; // 파괴 가능한 오브젝트로 설정
        rayfireRigid.objectType = ObjectType.SkinnedMesh;
        rayfireRigid.demolitionType = DemolitionType.Runtime; // 런타임에 파괴가 일어나도록 설정
        rayfireRigid.clusterDemolition.demolishable = true; // Cluster Demolition 활성화
        rayfireRigid.clusterDemolition.connectivity = ConnectivityType.ByBoundingBox; // 연결성을 BoundingBox 기반으로 설정
        Initalize();
    }
    int cnt = 5;
    // SetDamage 메서드를 BattleSystem에서 오버라이드
    public override void SetDamage(SkillData skillData)
    {
        //if (rayfireRigid == null) return;

        //// 데미지 계산
        //float damage = skillData.Dmg; // 스킬의 데미지 값
        //m_curHealPoint -= damage;

        //if (m_curHealPoint <= 0)
        //{
        //    m_curHealPoint = 0;
        //    SoundManager.Inst.StopSfxSound("Rock_Hit");
        //    DestroyObject();
        //}
        //SoundManager.Inst.PlaySfx("Rock_Hit");
        cnt--;
        if(cnt <= 0)
        {
            SoundManager.Inst.StopSfxSound("Rock_Hit");
            DestroyObject();
        }
        else
        {
            SoundManager.Inst.PlaySfx("Rock_Hit");
        }
    }
    public override void Initalize()
    {
        m_curHealPoint = m_stat.MaxHp;
        m_curMagicPoint = m_stat.MaxMp;
    }



    private void DestroyObject()
    {
        //Debug.Log("Totem이 완전히 파괴되었습니다!");
        SoundManager.Inst.PlaySfx("Rock_Broke");
        // 파괴 실행
        rayfireRigid.Demolish();
        // RayfireMan에서 파편 제거 코루틴 실행
        if (RayfireMan.inst != null)
        {
            RayfireMan.inst.StartCoroutine(ClearFragmentsAfterDelay(5f));
            // MonsterManager에 해당 웨이포인트 비활성화 요청
            MonsterManager.Instance.DisableWaypoint(assignedWaypoint);
            // MonsterManager에 해당 토템이 파괴되었음을 알림
            MonsterManager.Instance.OnTotemDestroyed();
            // **웨이포인트에 소환된 몬스터 제거**
            MonsterManager.Instance.RemoveMonstersAtWaypoint(assignedWaypoint);
        }

        // 본체 오브젝트 제거
        Destroy(gameObject, 2f);
    }


    private IEnumerator ClearFragmentsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // 모든 파편을 RayfireMan을 통해 삭제
        if (RayfireMan.inst != null)
        {
            RayfireMan.inst.DestroyStorage();
        }


    }

    // Update is called once per frame
    void Update()
    {

    }
}
