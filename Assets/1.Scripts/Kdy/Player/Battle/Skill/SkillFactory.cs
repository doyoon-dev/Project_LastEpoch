using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SkillFactory
{
    public static ISkill Create(SkillData data, MonoBehaviour coroutineRunner)
    {
        switch (data.skillType)
        {
            case SkillType.Warpath: return new Whirlwind(data, coroutineRunner);
            case SkillType.Lunge: return new Lunge(data, coroutineRunner);
            case SkillType.ErasingStrike: return null;
            default: return null;
        }
    }
}
