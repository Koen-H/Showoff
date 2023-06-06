using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour
{


    public float duration = 1f;
    public float strength = 1f;
    [SerializeField, Tooltip("If it's a reference it will be used for applying and not doing anything by iteself.")] 
    bool isReference = false;

    [SerializeField] public EffectType effectTypes;
    [HideInInspector] public EffectManager manager;


    /// <summary>
    /// Simple countdown for when the effect should wear off.
    /// The effects duration can change mid-effect.
    /// </summary>
    /// <returns></returns>
    private IEnumerator EffectCountdown()
    {
        while (duration > 0)
        {
            duration -= 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        manager.RemoveEffect(this);
    }

    /// <summary>
    /// Reset the effect if the duration is longer
    /// Also increases the strength of the ffect if thew new effect is stronger.
    /// </summary>
    /// <param name="effect"></param>
    public void ResetEffect(Effect effect)
    {
        if (effect.duration > duration) duration = effect.duration;
        if (effect.strength > strength)
        {
            strength = effect.strength;
        }
    }

    /// <summary>
    /// At the start of the effect, could add some particles around the player
    /// </summary>
    public virtual void ApplyEffect()
    {
        StartCoroutine(EffectCountdown());
    }

    /// <summary>
    /// At the end of the effect, could remove some particles 
    /// </summary>
    public virtual void RemoveEffect()
    {
       
    }

    public virtual void CopyFrom(Effect original)
    {
        duration = original.duration;
        strength = original.strength;
    }

    /// <summary>
    /// Applies the buff to the damage
    /// </summary>
    /// <param name="damage">The damage</param>
    /// <returns>The effect applied damage</returns>
    public virtual float ApplyAttackEffect(float damage)
    {
        Debug.LogWarning("Apply Damage Effect called but not implemented!");
        return damage;
    }

    /// <summary>
    /// Applies the buff to the movementSpeed
    /// </summary>
    /// <param name="movementSpeed">returns the buffed movementSpeed</param>
    /// <returns>The effect applied movement</returns>
    public virtual float ApplyMovementEffect(float movementSpeed)
    {
        Debug.LogWarning("Apply Movement Effect called but not implemented!");
        return movementSpeed;
    }

    /// <summary>
    /// Applies the resistance effect to the incoming damage
    /// </summary>
    /// <param name="incDamage">the damage</param>
    /// <returns>the effect applied incoming damage</returns>
    public virtual float ApplyResistanceEffect(float incDamage)
    {
        Debug.LogWarning("Apply Resistance Effect called but not implemented!");
        return incDamage;
    }

}
[System.Flags]
public enum EffectType { NONE = 0, ATTACK = 1, MOVEMENT = 2, RESISTANCE = 4, }