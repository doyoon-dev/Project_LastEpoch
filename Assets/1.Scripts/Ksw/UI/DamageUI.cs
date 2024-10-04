using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageUI : MonoBehaviour
{
    public Text damageText;
    private Camera mainCamera;  // 메인 카메라를 참조


    public void SetDamage(float damage)
    {
        damageText.text = damage.ToString();  // 데미지 값을 텍스트로 설정
    }

    // 애니메이션 또는 이동 후에 자동으로 텍스트를 비활성화
    public void DestroyAfter(float duration)
    {
        Destroy(gameObject, duration);
    }

    void Start()
    {
        // 메인 카메라를 참조
        mainCamera = Camera.main;

    }
    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 텍스트가 항상 카메라를 바라보게 만듦
            Vector3 lookDirection = transform.position - mainCamera.transform.position;

            // Y축 회전을 고정하여 텍스트가 휘어지지 않게 함
            lookDirection.y = 0;

            // 카메라를 향해 회전
            transform.rotation = Quaternion.LookRotation(lookDirection);

            // 텍스트가 뒤집히지 않도록 Z축을 기준으로 180도 회전
            transform.Rotate(0, 180, 0);

            // 카메라의 Z축 회전을 가져와 텍스트에 반영하여 기울기 조정
            Vector3 cameraEulerAngles = mainCamera.transform.eulerAngles;
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, -cameraEulerAngles.z);

            // 텍스트가 휘어져 보일 경우 스케일 보정
            if (transform.localScale.x < 0 || transform.localScale.z < 0)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, Mathf.Abs(transform.localScale.z));
            }
        }

        
    }
}
