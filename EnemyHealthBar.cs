using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Slider slider;
    public Image fillImage;  // 체력바 Fill 이미지 연결

    public Color fullHealthColor = Color.green;  // 체력 100% 이상일 때 색
    public Color lowHealthColor = Color.red;     // 체력 25 이하일 때 색

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        slider.value = health;
        UpdateHealthColor();
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        UpdateHealthColor();
    }

    // 체력 상태에 따라 색 변경 (물리 영향 없음)
    private void UpdateHealthColor()
    {
        if (slider.value <= 25f)
            fillImage.color = lowHealthColor;
        else
            fillImage.color = fullHealthColor;
    }

    void Update()
    {
        // 체력바가 항상 카메라를 향하도록
        transform.forward = Camera.main.transform.forward;
    }
}