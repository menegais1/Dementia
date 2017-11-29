using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Morphin : Item
{
    [SerializeField] private float lifeRegenAmount;
    [SerializeField] private float velocityMultiplier;
    [SerializeField] private float staminaRegenMultiplier;
    [SerializeField] private float duration;

    private PlayerStatusVariables playerStatusVariables;
    private PlayerStatusController playerStatusController;
    private PlayerHorizontalMovement playerHorizontalMovement;

    private float currentTimeMorphinEffect;

    private void Update()
    {
        if (!playerStatusVariables.isMorphinActive)
        {
            Destroy(gameObject);
        }
    }

    public void Effect(PlayerStatusController playerStatusController,
        PlayerHorizontalMovement playerHorizontalMovement, PlayerStatusVariables playerStatusVariables)
    {
        this.playerHorizontalMovement = playerHorizontalMovement;
        this.playerStatusController = playerStatusController;
        this.playerStatusVariables = playerStatusVariables;


        if (playerStatusVariables.isAdrenalineActive)
        {
            var range = Random.Range(0, 11);
            if (range <= 5)
            {
                playerStatusController.Die();
            }
            else
            {
                playerStatusVariables.isMorphinActive = true;
                playerStatusController.RegenLifeTemporary(duration, lifeRegenAmount, true);
            }
        }
        else
        {
            playerStatusVariables.isMorphinActive = true;
            playerStatusController.RegenLifeTemporary(duration, lifeRegenAmount, false);
            currentTimeMorphinEffect = Time.time + duration;
            CoroutineManager.AddCoroutine(MorphinEffectCoroutine(staminaRegenMultiplier, velocityMultiplier),
                "MorphinEffectCoroutine", this);
        }
    }


    private IEnumerator MorphinEffectCoroutine(float staminaRegenMultiplier, float velocityMultiplier)
    {
        playerStatusController.StaminaRegenMultiplier = staminaRegenMultiplier;
        playerHorizontalMovement.VelocityMultiplier = velocityMultiplier;

        while (Time.time <= currentTimeMorphinEffect)
        {
            yield return new WaitForFixedUpdate();
        }

        playerStatusController.StaminaRegenMultiplier = 1f;
        playerHorizontalMovement.VelocityMultiplier = 1f;

        playerStatusVariables.isMorphinActive = false;
        CoroutineManager.DeleteCoroutine("MorphinEffectCoroutine", this);
    }
}