using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lunge : SkillBase
{
    GameObject obj;
    public Lunge(SkillData data, MonoBehaviour coroutineRunner) : base(data, coroutineRunner) { }

    public override void Cast(GameObject caster, GameObject target)
    {
        if (caster.GetComponent<BattleSystem>().m_curMagicPoint < data.Mp) return;
        if (isCasting) return;
        isCasting = true;

        ConsumeMana(data.Mp);

        SkillManager.instance.SetUsingSkill(true);
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(mousePos, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Background")))
        {
            Vector3 dir = hit.point - caster.transform.position;
            coroutineRunner.StartCoroutine(LungeMove(caster, dir));
        }
    }

    IEnumerator LungeMove(GameObject caster, Vector3 dir)
    {
        float delta = 0;
        dir.Normalize();
        dir.y = 0;

        caster.transform.forward = dir;

        CharacterProperty animator = caster.GetComponent<CharacterProperty>();
        if (animator != null)
        {
            animator.m_myAnim.SetBool("SkillLunge", true);
        }

        if (data.vfxPrefab)
        {
            Transform spawnPoint = vfxSpawnPoint != null ? vfxSpawnPoint : caster.transform;
            obj = ObjectPool.Inst.Pull<GameObject>(data.vfxPrefab, caster.transform);
            obj.transform.position = spawnPoint.position;
            //obj.transform.position = caster.transform.position + caster.transform.forward * 2f + Vector3.up * 0.7f;
            obj.transform.forward = caster.transform.forward;
        }
        HashSet<IBattle> hitTargets = new HashSet<IBattle>();

        while (delta < 0.5f)
        {
            delta += Time.deltaTime;
            Collider[] hits = Physics.OverlapSphere(caster.transform.position + caster.transform.forward * 1.2f, data.radius);
            foreach (Collider hit in hits)
            {
                IBattle ib = hit.GetComponent<IBattle>();
                if (ib != null && !hitTargets.Contains(ib))
                {
                    ib.Damaged(data.Dmg);
                    hitTargets.Add(ib);
                }
            }
            caster.transform.Translate(dir * 5f * Time.deltaTime, Space.World);
            yield return null;
        }
        ObjectPool.Inst.Push<GameObject>(obj);
        animator.m_myAnim.SetBool("SkillLunge", false);
        SkillManager.instance.SetUsingSkill(false);
        StopCast();
        caster.GetComponent<Player>().RecoveMP();
    }
}
