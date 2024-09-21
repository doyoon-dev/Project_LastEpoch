using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    public Text monNameText; // 몬스터 이름 표시
    public Image healthImage; // 체력바
    public GameObject healthBarUI; // 중앙체력바 ui 오브젝트


    // 체력바 초기화
    public void Initialize(string monsterName, float maxHealth)
    {
        monNameText.text = monsterName;  // 몬스터 이름 설정
        healthImage.fillAmount = 1;      // 중앙 체력바를 가득 채움     
        healthBarUI.SetActive(true);     // 중앙 체력바 UI 활성화
        
    }

    // 체력바 업데이트
    public void UpdateHealth(int currentHealth, float maxHealth)
    {
        float healthRatio = (float)currentHealth / maxHealth;
        healthImage.fillAmount = healthRatio;        // 중앙 체력바 업데이트
    }

    // 체력바를 활성화
    public void ShowHealthBar()
    {
        healthBarUI.SetActive(true);      // 중앙 체력바 UI 활성화
        
    }


    // 체력바 비활성화
    public void HideHealthBar()
    {
        healthBarUI.SetActive(false);      // 중앙 체력바 UI 비활성화
      
    }


}
