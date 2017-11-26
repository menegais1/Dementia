﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : MonoBehaviour
{
    [SerializeField] private float currentStamina;
    [SerializeField] public float maxStamina;
    [SerializeField] private float staminaRegenRatePerSecond;
    [SerializeField] private float timeToStartStaminaRegen;
    [SerializeField] private float currentLife;
    [SerializeField] private float maxLife;
    [SerializeField] private Slider staminaBar;

    private bool canSave;
    private bool activeMorphin;
    private bool activeAdrenaline;
    private int diaryState;
    private bool canUseDiary;

    private float currentTimeToStartStaminaRegen;
    private float timeTrackerForSpendingStamina;

    void Start()
    {
        currentStamina = maxStamina;
        currentLife = maxLife;
        timeTrackerForSpendingStamina = Time.time;
        staminaBar.maxValue = maxStamina;
        staminaBar.minValue = 0;
        staminaBar.value = currentStamina;
    }

    void Update()
    {
        RegenStamina();
    }

    public void SaveGame()
    {
    }

    public void ArchiveNotes()
    {
    }

    public void LoadGame()
    {
    }

    public void SpendStamina(float percentOfStaminaToSpent, bool imediate)
    {
        if (CoroutineManager.CheckIfCoroutineExists("RegenStaminaCoroutine"))
        {
            CoroutineManager.DeleteCoroutine("RegenStaminaCoroutine");
        }

        if (imediate)
        {
            currentStamina -= maxStamina * percentOfStaminaToSpent / 100;
        }
        else if (Time.time >= timeTrackerForSpendingStamina)
        {
            timeTrackerForSpendingStamina = Time.time + Time.fixedDeltaTime;
            currentStamina -= maxStamina * percentOfStaminaToSpent / 100 * Time.fixedDeltaTime;
        }

        currentStamina = currentStamina <= 0 ? 0 : currentStamina;
        staminaBar.value = currentStamina;

        currentTimeToStartStaminaRegen = Time.time + timeToStartStaminaRegen;
    }

    private void RegenStamina()
    {
        if (Time.time >= currentTimeToStartStaminaRegen && currentStamina < maxStamina &&
            !CoroutineManager.CheckIfCoroutineExists("RegenStaminaCoroutine"))
        {
            CoroutineManager.AddCoroutine(RegenStaminaCoroutine(), "RegenStaminaCoroutine");
        }
    }

    private IEnumerator RegenStaminaCoroutine()
    {
        var t = Time.fixedDeltaTime;

        while (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRatePerSecond * t;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }
            staminaBar.value = currentStamina;
            yield return new WaitForFixedUpdate();
        }

        CoroutineManager.DeleteCoroutine("RegenStaminaCoroutine");
    }


    public void TakeDamage(float lifeToDecrease)
    {
        currentLife -= lifeToDecrease;

        if (currentLife < 0)
        {
            currentLife = 0;
        }
    }

    public bool RegenLife(float lifeToRegen)
    {
        if (LifeIsFull())
        {
            return false;
        }

        currentLife += lifeToRegen;
        if (currentLife > maxLife)
        {
            currentLife = maxLife;
        }

        return true;
    }

    public bool LifeIsFull()
    {
        return MathHelpers.Approximately(currentLife, maxLife, float.Epsilon);
    }

    public void Die()
    {
    }

    public void UseDiary()
    {
    }

    public void CheckSaveItem()
    {
    }

    public bool CheckStamina(float percentOfStaminaToSpent, bool imediate)
    {
        return currentStamina >= (maxStamina * percentOfStaminaToSpent / 100) * (imediate ? 1 : Time.fixedDeltaTime);
    }

    public void SelectItens()
    {
    }

    public void LoseLife()
    {
    }
}