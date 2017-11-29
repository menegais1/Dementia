using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Adrenaline : Item
{
    [SerializeField] private float velocityMultiplier;
    [SerializeField] private float staminaRegenMultiplier;
    [SerializeField] private float duration;
    [SerializeField] private float velocityMultiplierAfterEffect;
    [SerializeField] private float staminaRegenMultiplierAfterEffect;
    [SerializeField] private float durationAfterEffect;

    private PlayerStatusVariables playerStatusVariables;
    private PlayerStatusController playerStatusController;
    private PlayerHorizontalMovement playerHorizontalMovement;

    private float currentTimeAdrenalineEffect;

    private void Update()
    {
        if (!playerStatusVariables.isAdrenalineActive && !playerStatusVariables.isAdrenalineAfterEffectActive)
        {
            Destroy(gameObject);
        }
        else if (playerStatusVariables.isAdrenalineAfterEffectActive &&
                 !CoroutineManager.CheckIfCoroutineIsRunning("AdrenalineEffectCoroutine"))
        {
            Debug.Log("afterEffect");
            AfterEffect();
        }
    }

    public void Effect(PlayerStatusController playerStatusController,
        PlayerHorizontalMovement playerHorizontalMovement, PlayerStatusVariables playerStatusVariables)
    {
        this.playerHorizontalMovement = playerHorizontalMovement;
        this.playerStatusController = playerStatusController;
        this.playerStatusVariables = playerStatusVariables;

        playerStatusVariables.isAdrenalineActive = true;


        if (playerStatusVariables.isMorphinActive)
        {
            var range = Random.Range(0, 11);
            if (range <= 5)
            {
                playerStatusController.Die();
            }
        }

        currentTimeAdrenalineEffect = Time.time + duration;
        CoroutineManager.AddCoroutine(AdrenalineEffectCoroutine(staminaRegenMultiplier, velocityMultiplier),
            "AdrenalineEffectCoroutine", this);
    }

    public void AfterEffect()
    {
        currentTimeAdrenalineEffect = Time.time + durationAfterEffect;
        CoroutineManager.AddCoroutine(
            AdrenalineEffectCoroutine(staminaRegenMultiplierAfterEffect, velocityMultiplierAfterEffect),
            "AdrenalineEffectCoroutine", this);
    }


    private IEnumerator AdrenalineEffectCoroutine(float staminaRegenMultiplier, float velocityMultiplier)
    {
        playerStatusController.StaminaRegenMultiplier = staminaRegenMultiplier;
        playerHorizontalMovement.VelocityMultiplier = velocityMultiplier;

        while (Time.time <= currentTimeAdrenalineEffect)
        {
            yield return new WaitForFixedUpdate();
        }

        playerStatusController.StaminaRegenMultiplier = 1f;
        playerHorizontalMovement.VelocityMultiplier = 1f;

        playerStatusVariables.isAdrenalineAfterEffectActive = playerStatusVariables.isAdrenalineActive;
        playerStatusVariables.isAdrenalineActive = false;

        CoroutineManager.DeleteCoroutine("AdrenalineEffectCoroutine", this);
    }
}