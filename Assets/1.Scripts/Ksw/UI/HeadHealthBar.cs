using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeadHealthBar : MonoBehaviour
{

    public GameObject headHealthBarUI;// 몬스터 머리 위 체력바 ui 오브젝트
    public Image headHealthImage; //머리 위 체력바 이미지
    private Camera mainCamera;  // 카메라를 참조할 변수

    // 체력바 초기화
    public void Initialize(float maxHealth)
    {      
        headHealthImage.fillAmount = 1;  // 머리 위 체력바를 가득 채움
        headHealthBarUI.SetActive(true); // 머리 위 체력바 UI 활성화
    }

    // 체력바 업데이트
    public void UpdateHeadHealth(int currentHealth, float maxHealth)
    {
        float healthRatio = (float)currentHealth / maxHealth;
        headHealthImage.fillAmount = healthRatio;    // 머리 위 체력바 업데이트;
    }

    // 체력바를 활성화
    public void ShowHeadHealthBar()
    {
        
        headHealthBarUI.SetActive(true);  // 머리 위 체력바 UI 활성화
    }

    // 체력바 비활성화
    public void HideHeadHealthBar()
    {
       
        headHealthBarUI.SetActive(false);  // 머리 위 체력바 UI 비활성화

    }
    // Start is called before the first frame update
    void Start()
    {
        // 메인 카메라를 가져옴
        mainCamera = Camera.main;
        // 초기 상태에서 체력바 숨기기
        headHealthBarUI.SetActive(false);
    }

    void LateUpdate()
    {
        // 체력바가 카메라를 향하도록 설정 (카메라 방향으로 LookAt 회전)
        if (mainCamera != null)
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
                             mainCamera.transform.rotation * Vector3.up);
        }
    }
}

