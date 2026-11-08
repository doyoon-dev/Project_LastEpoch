using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// ISkill 인터페이스에는 Skill에 꼭 필요한 최소한의 기능들만 넣어야함
// SkillBase에 공격스킬, 패시브스킬, 버프스킬 관련 기능들이 들어갈 수 있기 때문에
public interface ISkill
{
    SkillData data { get; }
    void Cast(GameObject caster, GameObject target = null);
    void StopCast();
}

public abstract class SkillBase : ISkill
{
    public SkillData data { get; private set; }

    protected MonoBehaviour coroutineRunner;
    public Transform vfxSpawnPoint;
    protected bool isCasting = false;
    UnityAction<float> useManaCallBack;
    UnityAction<KeyCode> stopSkillCast;

    public SkillBase(SkillData data, MonoBehaviour coroutineRunner)
    {
        this.data = data;
        this.coroutineRunner = coroutineRunner;
    }

    // Player or SkillManager에서 이 함수를 호출해 콜백 등록
    public void SetUseManaCallBack(UnityAction<float> useManaCallBack)
    {
        if (this.useManaCallBack == null)
        {
            this.useManaCallBack = useManaCallBack;
        }
    }

    // Skill에서 스킬 사용 시 호출
    protected void ConsumeMana(float amount)
    {
        useManaCallBack?.Invoke(amount);
    }

    public abstract void Cast(GameObject caster, GameObject target = null);

    public virtual void StopCast()
    {
        isCasting = false;
    }

    public void SetStopSkillCastCallBack(UnityAction<KeyCode> stopSkillCastCallBack)
    {
        if (stopSkillCast == null)
        {
            stopSkillCast = stopSkillCastCallBack;
        }
    }
}
