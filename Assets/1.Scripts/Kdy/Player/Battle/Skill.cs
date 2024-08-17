using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IUsingSkill
{
    bool UsingSkill();
}

public class Skill : MonoBehaviour, IUsingSkill
{
    public UnityEvent<float> m_useSkill;
    [HideInInspector]
    public bool m_usingSkill = false;
    public Player m_player;    // 나중에 인터페이스로 바꿔야 될 수 있음
    public PlayerUI m_playerUI;
    // Start is called before the first frame update
    void Awake()
    {
        m_player = transform.GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    Q_SkillInputKey();
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    W_SkillInputKey();
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    E_SkillInputKey();
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    R_SkillInputKey();
        //}
    }

    protected void UsingSkillMp(float skillMp)
    {
        m_useSkill?.Invoke(skillMp);
    }

    protected virtual void Q_SkillInputKey()
    {
        // Q 스킬 창에 어떤 스킬이 있는지 확인
        // 애니메이션 실행
        // 데미지 주기
        // 스킬 바뀌었을 때 바뀐 스킬로 실행
    }

    protected virtual void W_SkillInputKey()
    {

    }

    protected virtual void E_SkillInputKey()
    {

    }

    protected virtual void R_SkillInputKey()
    {

    }

    public bool UsingSkill()
    {
        if(m_usingSkill)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // 스킬 사용 가능한지 체크하는 함수
    public bool IsSkillUsable(SkillData skillData)
    {

        return false;
    }
    
    // 쿨타임
    public void SkillCoolTime(SkillData skillData)
    {

    }
}
