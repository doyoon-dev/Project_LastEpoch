using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageUI : MonoBehaviour
{
    private float moveSpeed = 2.0f;  // 텍스트가 올라가는 속도
    private float alphaSpeed = 2.0f; // 알파 값이 서서히 줄어드는 속도
  
    public TextMeshProUGUI damageText;
    private Color alpha;

    void Start()
    {
        alpha = damageText.color;
        alpha.a = 1.0f;  // 초기 알파값을 1로 설정 (완전히 불투명)
        damageText.color = alpha;

    }

    public void SetDamage(float damage)
    {
        damageText.text = damage.ToString();  // 데미지 값을 텍스트로 설정
    }

    void Update()
    {
        // 텍스트가 위로 이동
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));

        // 텍스트 알파값이 서서히 줄어듦
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed);
        damageText.color = alpha;  // 텍스트의 색상에 반영
    }
    
    public void DestroyAfter(float duration)
    {
        Destroy(gameObject, duration);  // 오브젝트 즉시 파괴
    }
}
