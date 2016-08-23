using UnityEngine;
using System.Collections;

public abstract class Effect {
    public abstract void ApplyEffect();
    public virtual bool IsEffectComplete { get; set; }
}
