using System.Collections;
using System.Collections.Generic;
using UnityEditor.Presets;
using UnityEngine;

public class Whirlwind : SkillBase
{
    Coroutine routine;                  // 실행한 코루틴
    GameObject vfxPrefabInst;

    public Whirlwind(SkillData data, MonoBehaviour coroutineRunner) : base(data, coroutineRunner) { }

    public override void Cast(GameObject caster, GameObject target = null)
    {
        if (caster.GetComponent<BattleSystem>().m_curMagicPoint < data.Mp) return;
        if (isCasting) return;
        isCasting = true;
        SkillManager.instance.SetUsingSkill(true);
        coroutineRunner.StartCoroutine(DoWhirlwind(caster));

        CharacterProperty animator = caster.GetComponent<CharacterProperty>();
        if (animator != null)
        {
            animator.m_myAnim.SetBool("SkillWarPath", true);
        }
    }

    public override void StopCast()
    {
        base.StopCast();
        if (routine != null && coroutineRunner != null)
        {
            coroutineRunner.StopCoroutine(routine);
            routine = null;
        }

        CharacterProperty animator = coroutineRunner.GetComponent<CharacterProperty>();
        if (animator != null)
        {
            animator.m_myAnim.SetBool("SkillWarPath", false);
        }

        if (vfxPrefabInst)
        {
            ObjectPool.Inst.Push<GameObject>(vfxPrefabInst);
        }
        SkillManager.instance.SetUsingSkill(false);
    }

    IEnumerator DoWhirlwind(GameObject caster)
    {
        float tickRate = 0.5f;

        if (data.vfxPrefab)
        {
            Transform spawnPoint = vfxSpawnPoint != null ? vfxSpawnPoint : caster.transform;
            vfxPrefabInst = ObjectPool.Inst.Pull<GameObject>(data.vfxPrefab);
            vfxPrefabInst.transform.position = spawnPoint.position;
        }
        if (data.sfx)
        {
            SoundManager.Inst.PlaySfx("WarPath_Playing_Sound");
        }
        
        while (SkillManager.instance.IsPressedKey)
        {
            ConsumeMana(data.Mp);
            if (caster.GetComponent<BattleSystem>().m_curMagicPoint < data.Mp) break;
            Collider[] hits = Physics.OverlapSphere(caster.transform.position, data.radius, LayerMask.GetMask("Monster", "Boss"));
            foreach (Collider hit in hits)
            {
                //if (hit.gameObject == caster) continue;
                IBattle ib = hit.GetComponent<IBattle>();
                if (ib != null)
                {
                    ib.Damaged(data.Dmg);
                }
            }
            yield return new WaitForSeconds(tickRate);
        }
        StopCast();
        caster.GetComponent<Player>().RecoveMP();
    }
    //public void Skill_WarPath(KeyCode inputKey)
    //{
    //    // 스킬 키 누르고 있으면 마나를 다 쓸 때 까지 스킬 발동
    //    // 마우스 방향으로 이동가능
    //    if (Input.GetKey(inputKey) && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //    {
    //        if (!m_usingSkill)
    //        {
    //            if (!m_isSoundPlay)
    //            {
    //                SoundManager.Inst.PlaySfx("WarPath_Playing_Sound");
    //                m_isSoundPlay = true;
    //            }
    //            if (m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //            {
    //                m_stopMovingAct?.Invoke();
    //                return;
    //            }
    //            //StopAllCoroutines();
    //            StopRecover();
    //            m_myAnim.SetBool("Move", false);
    //            m_usingSkill = true;
    //            m_stopMovingAct?.Invoke();
    //            //RecoverMp(m_usingSkill);
    //            UsingSkillMp(SkillDataManager.m_skillDataDic["Warpath"].Mp * Time.deltaTime * SkillDataManager.m_skillDataDic["Warpath"].Channeling);
    //            if (!m_warPathUse)
    //            {
    //                m_warpathEffect.SetActive(true);

    //                m_warPathUse = true;
    //                m_myAnim.SetBool("SkillWarPath", true);
    //            }
    //        }

    //        SkillMove();
    //    }


    //    if (Input.GetKeyUp(inputKey) || m_player.m_curMagicPoint < SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //    {
    //        m_isSoundPlay = false;
    //        SoundManager.Inst.StopSfxSound("WarPath_Playing_Sound");
    //        m_warpathEffect.SetActive(false);
    //        m_myAnim.SetBool("SkillWarPath", false);
    //        m_warPathUse = false;
    //        m_usingSkill = false;
    //        //RecoverMp(m_usingSkill);
    //        Invoke("DelayMp", m_delayTime);
    //    }
    //}

    //void DelayMp()
    //{
    //    RecoverMp(m_usingSkill);
    //}

    //public void SkillMove()
    //{
    //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //    if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, m_enemyMask | m_backgroundMask))
    //    {
    //        StopAllCoroutines();
    //        StartCoroutine(SkillMoving(hit.point));
    //    }
    //    //StopAllCoroutines();
    //    //StartCoroutine(SkillMoving(target));
    //}

    //public IEnumerator SkillMoving(Vector3 target)
    //{
    //    Vector3 dir = target - transform.position;
    //    dir.Normalize();
    //    dir.y = 0;

    //    while (m_warPathUse && m_usingSkill && m_player.m_curMagicPoint >= SkillDataManager.m_skillDataDic["Warpath"].Mp)
    //    {
    //        UsingSkillMp(SkillDataManager.m_skillDataDic["Warpath"].Mp * Time.deltaTime * SkillDataManager.m_skillDataDic["Warpath"].Channeling);
    //        float delta = Time.deltaTime * m_player.m_moveStat.moveSpeed;
    //        transform.Translate(dir * delta, Space.World);
    //        yield return null;
    //    }
    //}
}
