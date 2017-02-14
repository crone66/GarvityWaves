using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;
public class DamageAbleObject : MonoBehaviour
{
    [SerializeField]
    [Range(0.1f, 100000f)]
    private float health = 10;

    [SerializeField]
    [Range(0.1f, 100000f)]
    private float maxHealth = 10;

    public event EventHandler OnDeath;
    public event EventHandler<OnHealthChangedArgs> OnReceiveDamage;
    public event EventHandler<OnHealthChangedArgs> OnReceiveHealth;

    public float Health
    {
        get { return health; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

	// Use this for initialization
	private void Start ()
    {
        if(health > maxHealth)
            health = maxHealth;
	}
	
	// Update is called once per frame
	private void Update ()
    {
		
	}

    public virtual void DoDamage(float damage, GameObject statusEffect)
    {
        OnHealthChangedArgs args = new OnHealthChangedArgs(damage, statusEffect);
        if (OnReceiveDamage != null)
            OnReceiveDamage(this, args);

        if (!args.Cancel)
        {
            health -= args.ChangeValue;
            if (health <= 0)
            {
                if (OnDeath != null)
                    OnDeath(this, EventArgs.Empty);
            }
        }
    }

    public virtual void DoDamage(float damage)
    {
        DoDamage(damage, null);
    }

    public virtual void Heal(float addHealth)
    {
        OnHealthChangedArgs args = new OnHealthChangedArgs(addHealth, null);
        if (OnReceiveHealth != null)
            OnReceiveHealth(this, args);

        if (!args.Cancel)
        {
            health += args.ChangeValue;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }
}
