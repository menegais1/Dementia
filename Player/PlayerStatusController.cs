using System.Collections;
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
    private int diaryState;
    private bool canUseDiary;

    private float staminaRegenMultiplier;
    private float originalHealth;
    private float regenLifeTemporaryTime;

    private float currentTimeToStartStaminaRegen;
    private float timeTrackerForSpendingStamina;

    private PlayerStatusVariables playerStatusVariables;

    public float StaminaRegenMultiplier
    {
        get { return staminaRegenMultiplier; }
        set { staminaRegenMultiplier = value; }
    }

    public float CurrentLife
    {
        get { return currentLife; }
        set { currentLife = value; }
    }

    void Start()
    {
        currentStamina = maxStamina;
        currentLife = maxLife;
        timeTrackerForSpendingStamina = Time.time;
        staminaBar.maxValue = maxStamina;
        staminaBar.minValue = 0;
        staminaBar.value = currentStamina;
        playerStatusVariables = GetComponent<PlayerStatusVariables>();
        StaminaRegenMultiplier = 1;
    }

    void Update()
    {
        RegenStamina();
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
            currentStamina += staminaRegenRatePerSecond * StaminaRegenMultiplier * t;

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
        currentLife = CurrentLife - lifeToDecrease;

        if (CurrentLife < 0)
        {
            currentLife = 0;
        }
    }

    public void RegenLife(float lifeToRegen)
    {
        currentLife = CurrentLife + lifeToRegen;
        if (CurrentLife > maxLife)
        {
            currentLife = maxLife;
        }
    }

    public void RegenLifeTemporary(float duration, float lifeToRegen, bool deactivateMorphin)
    {
        regenLifeTemporaryTime = Time.time + duration;
        CoroutineManager.AddCoroutine(RegenLifeTemporaryCoroutine(lifeToRegen, deactivateMorphin),
            "RegenLifeTemporaryCoroutine");
    }

    public IEnumerator RegenLifeTemporaryCoroutine(float lifeToRegen, bool deactivateMorphin)
    {
        originalHealth = CurrentLife;
        currentLife = CurrentLife + lifeToRegen;

        while (regenLifeTemporaryTime > Time.time)
        {
            if (CurrentLife > originalHealth)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                regenLifeTemporaryTime = Time.time;
            }
        }
        if (deactivateMorphin)
            playerStatusVariables.isMorphinActive = false;

        currentLife = originalHealth;
        CoroutineManager.DeleteCoroutine("RegenLifeTemporaryCoroutine");
    }

    public bool LifeIsFull()
    {
        return MathHelpers.Approximately(CurrentLife, maxLife, float.Epsilon);
    }

    public void Die()
    {//Necessário uma tela de morte
        GameManager.instance.LoadData(true);
    }

    public bool CheckStamina(float percentOfStaminaToSpent, bool imediate)
    {
        return currentStamina >= (maxStamina * percentOfStaminaToSpent / 100) * (imediate ? 1 : Time.fixedDeltaTime);
    }
}