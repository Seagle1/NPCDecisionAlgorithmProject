using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour, IHealth
{
    [SerializeField]private HealthSO healthSO;
    [SerializeField] private Image healthImage;
    private float currentHealth;
    private float maxHealth = 100f;

    private void Start()
    {
        currentHealth = maxHealth;
        HealthUIStatus();
        UpdateHealthUI();
    }

    private void Update()
    {
        DecreaseHealth(Time.deltaTime);
    }

    public void DecreaseHealth(float amount)
    {
        currentHealth -= amount;
        float minHealth = 0;
        currentHealth = Mathf.Clamp(currentHealth, minHealth, maxHealth);
        UpdateHealthUI();
    }

    private void UpdateHealthUI()
    {
        if (healthImage != null)
        {
            healthImage.fillAmount = currentHealth / maxHealth;
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }
    private void HealthUIStatus()
    {
        Debug.Log("Health UI for Canvas:" + healthSO.canvasName);
    }
}
