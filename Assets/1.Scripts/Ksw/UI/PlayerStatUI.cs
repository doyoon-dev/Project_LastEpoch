using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatUI : MonoBehaviour
{
    [SerializeField]
    private Player s_player;
    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Text manaText;
    [SerializeField]
    private Text attackDmgText;
    [SerializeField]
    private Text defenseText;
    //[SerializeField]
    //private Text moveSpeedText;

    

    // Start is called before the first frame update
    void Start()
    {
        UpdateStatUI();
    }

    public void UpdateStatUI()
    {
        if(s_player != null)
        {
            healthText.text = $"{s_player.m_stat.MaxHp}";
            manaText.text = $"{s_player.m_stat.MaxMp}";
            attackDmgText.text = $"{s_player.m_stat.AttackDmg}";
            defenseText.text = $"{s_player.m_stat.Defense}";
            //moveSpeedText.text = $"{s_player.GetMoveSpeed()}";
        }

    }
    // Update is called once per frame
    void Update()
    {
        // K 키 입력을 감지
        if (Input.GetKeyDown(KeyCode.K))
        {
           
        }
    }
}
