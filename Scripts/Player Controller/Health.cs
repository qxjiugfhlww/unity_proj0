using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Tooltip("Amount of health")]
    public int mHealth = 100;
    private HealthBar mHealthBar;
    private int startHealth;
    private bool mCanTakeDamage = true;
    public event EventHandler PlayerDied;


    [Tooltip("Amount of food")]
    public int Food = 100;
    [Tooltip("Rate in seconds in which the hunger increases")]
    public float HungerRate = 0.5f;
    private HealthBar mFoodBar;
    private int startFood;



    public HUD Hud;

    public Animator animator;

    public void Init()
    {
        mHealthBar = Hud.transform.Find("Bars_Panel/HealthBar").GetComponent<HealthBar>();
        mHealthBar.Min = 0;
        mHealthBar.Max = mHealth;
        startHealth = mHealth;
        mHealthBar.SetValue(mHealth);

        mFoodBar = Hud.transform.Find("Bars_Panel/FoodBar").GetComponent<HealthBar>();
        mFoodBar.Min = 0;
        mFoodBar.Max = Food;
        startFood = Food;
        mFoodBar.SetValue(Food);

        InvokeRepeating("IncreaseHunger", 0, HungerRate);
    }

    public bool IsDead
    {
        get
        {
            return mHealth == 0 || Food == 0;
        }
    }

    public void Rehab(int amount)
    {
        mHealth += amount;
        if (mHealth > startHealth)
        {
            mHealth = startHealth;
        }

        mHealthBar.SetValue(mHealth);
    }

    public void TakeDamage(int amount)
    {
        if (!mCanTakeDamage)
            return;

        mHealth -= amount;
        if (mHealth < 0)
            mHealth = 0;

        mHealthBar.SetValue(mHealth);

        if (IsDead)
        {
            Die();
        }

    }

    private void Die()
    {
        animator.SetTrigger("die_tr");

        if (PlayerDied != null)
        {
            PlayerDied(this, EventArgs.Empty);
        }
    }


    public void IncreaseHunger()
    {
        Food--;
        if (Food < 0)
            Food = 0;

        mFoodBar.SetValue(Food);

        if (Food == 0)
        {
            CancelInvoke();
            Die();
        }
    }

    public void Eat(int amount)
    {
        Food += amount;
        if (Food > startFood)
        {
            Food = startFood;
        }

        mFoodBar.SetValue(Food);

    }
}
