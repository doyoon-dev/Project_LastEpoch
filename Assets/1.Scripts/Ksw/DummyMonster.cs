using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.Universal;

public class DummyMonster : MonsterController
{

    protected override void Start()
    {
        base.Start();

    }

    public override void BehaviourProcess()
    {

        switch (m_state)
        {
            // Idle 상태에서 플레이어를 바라보기만 함
            case BehaviourState.Idle:
                
                break;
            case BehaviourState.Damaged:
                break;
            case BehaviourState.Die:
                if (!IsDie) // IsDie가 false인 경우에만 처리
                {
                    HandleDeath();
                    IsDie = true; // IsDie를 true로 설정하여 다시 호출되지 않도록 방지
                }
                break;
        }

    }


    //플레이어한테 몬스터 공격을 받았을떄임
    public override void SetDamage(SkillData skillData)
    {
        if (IsDie || isTransitioning) return; // 무적 상태면 데미지 무시) return;
        if (m_state == BehaviourState.Gathering || m_state == BehaviourState.SpecialAttack || m_state == BehaviourState.Damaged) return;

        // 피 흘리는 이펙트 소환
        SpawnBleedEffect(transform.position);

        // 맞는 사운드
        SoundManager.Inst.PlaySfx("Hit_Sound");

        // 체력 감소 처리
        m_curHealPoint -= skillData.Dmg;

        // 몬스터는 흰색 데미지 텍스트 표시
        ShowDamageText(skillData.Dmg, Color.white);  // 색상으로 흰색 전달


        if (m_curHealPoint <= 0)
        {
            m_curHealPoint = 0;
            SetState(BehaviourState.Die);
            return;
        }

        // 일반 몬스터의 데미지 처리 (기존 로직)
        SetState(BehaviourState.Damaged);
        m_monAnimCtr.Play(MonsterAnimController.Motion.Hit, false);
        SetHitColor(0.2f);
        ApplyKnockback(skillData);// 넉백 처리
        StartCoroutine(ResumeMovementAfterDamage());


    }

   
     public override void HandleDeath()
    {
        if (IsDie) return;// 이미 죽은 상태에서 다시 처리하지 않도록 함
        IsDie = true;
        StopAllCoroutines();// 모든 코루틴 중지
        m_monAnimCtr.Play(MonsterAnimController.Motion.Die, false);  // 사망 애니메이션 재생
        StartCoroutine(Coroutine_SetDissolve(4f));  // 사라지는 효과
        DisableColiders();
        ShutDownHealthBars();
        AttackArea.SetActive(false);// 공격 범위 비활성화 (박스가 보이지 않도록 설정)                              
        GameObject bloodstainEffect = EffectManager.Instance.GetEffect("BloodSplatter02", transform.position, Quaternion.identity);// 핏자국 이펙트 생성
                                                                                                                                   // 몬스터 종류에 따라 죽음 소리 재생
        switch (monsterType)
        {
            case MonsterType.Dog:
                SoundManager.Inst.PlaySfx("Dog_Death");
                break;
            case MonsterType.Rat:
                SoundManager.Inst.PlaySfx("Rat_Death");
                break;

        }

    }

}

