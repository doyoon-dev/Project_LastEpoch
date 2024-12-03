using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using static UnityEngine.GraphicsBuffer;

public class DamageUI : MonoBehaviour
{
    private float moveSpeed = 1.0f;  // 텍스트가 올라가는 속도
    private float alphaSpeed = 3.0f; // 알파 값이 서서히 줄어드는 속도
    public LayerMask m_playerMask;
    public TextMeshProUGUI damageText;
    private Color alpha;

    // 데미지 텍스트 위치를 저장할 변수 (월드 좌표 기반)
    private Transform damagePosition;

    void Start()
    {
        alpha = damageText.color;
        alpha.a = 1f;  // 알파값 초기화
        damageText.color = alpha;
    }
    public void SetDamageTextColor(Color color)
    {
        damageText.color = color;
    }
    public void DMUISetDamage(float damage)
    {
        damageText.text = damage.ToString("F2");  // 데미지 값을 텍스트로 설정
        
    }
    // 데미지 텍스트를 화면에 표시할 위치 설정 (월드 좌표로 받음)
    public void DMUISetPosition(Transform pos)
    {
        damagePosition = pos;
    }
    private void OnEnable()
    {
        ResetState(Color.white);
    }

    public void ResetState(Color defaultColor)
    {
        // 알파 값 및 위치 초기화
        alpha = defaultColor;
        alpha.a = 1f; // 알파를 초기화
        damageText.color = alpha;

        // 위치 초기화
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = Vector2.zero; // 필요 시 기본 위치로 초기화
    }
    void Update()
    {
        if (damagePosition != null)
        {
            // 월드 좌표를 스크린 좌표로 변환
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(damagePosition.position);
            screenPosition.y += moveSpeed * Time.deltaTime * 100; // 이동 속도를 조정하여 부드럽게 올라가도록 설정

            // RectTransform을 통해 스크린 좌표 위치로 변환된 값을 반영
            RectTransform rect = GetComponent<RectTransform>();
            rect.position = screenPosition;  // 스크린 좌표로 위치 설정
        }

        // 텍스트가 위로 이동
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition += new Vector2(0, moveSpeed * Time.deltaTime);

        // 텍스트 알파값이 서서히 줄어듦
        alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * (alphaSpeed)); 
        damageText.color = alpha;  // 텍스트의 색상에 반영
    }

    public void DestroyAfter(float duration)
    {
        StartCoroutine(ReturnToPoolAfter(duration));
    }

    private IEnumerator ReturnToPoolAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        ObjectPool.Inst.Push<DamageUI>(gameObject);
    }
}
