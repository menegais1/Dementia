using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class Player : MonoBehaviour
{

    #region Váriaveis Gerais

    private bool canSave;

    [SerializeField]
    private float currentStamina;
    [SerializeField]
    public float maxStamina;
    [SerializeField]
    private float staminaRegenRate;
    [SerializeField]
    private float timeToStartStaminaRegen;
    private float currentTimeToStartStaminaRegen;

    private float life;
    private bool activeMorphin;
    private bool activeAdrenaline;
    private int diaryState;
    private bool canUseDiary;

    #endregion



    #region Métodos Unity


    // Use this for initialization
    void Start()
    {
        currentStamina = maxStamina;
    }

    // Update is called once per frame
    void Update()
    {
        //print(currentStamina);
        regenStamina();
    }

    #endregion

    #region Métodos Gerais

    public void saveGame()
    {

    }

    public void archiveNotes()
    {

    }

    public void loadGame()
    {

    }

    public void spendStamina(float staminaToSpent)
    {
        if (currentStamina >= staminaToSpent)
        {
            currentStamina -= staminaToSpent;
            if (currentStamina < 0)
            {
                currentStamina = 0;
            }
            currentTimeToStartStaminaRegen = Time.time + timeToStartStaminaRegen;
        }
    }


    public void regenStamina()
    {
        if (currentStamina < maxStamina)
        {

            if (Time.time >= currentTimeToStartStaminaRegen)
            {
                currentStamina += staminaRegenRate * Time.deltaTime;
                if (currentStamina > maxStamina)
                {
                    currentStamina = maxStamina;
                }
            }
        }
    }

    public void takeDamage()
    {

    }

    public void regenLife()
    {

    }

    public void useDiary()
    {

    }

    public void checkSaveItem()
    {

    }

    public bool checkStamina(float staminaToUseAction)
    {
        if (currentStamina < staminaToUseAction)
        {
            return false;
        }

        return true;
    }

    public void checkLife()
    {

    }

    public void checkBullets()
    {

    }

    public void selectItens()
    {

    }

    public void loseLife()
    {

    }

    #endregion





}
