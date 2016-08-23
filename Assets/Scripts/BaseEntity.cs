using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseEntity : MonoBehaviour {
    public IList<Effect> Effects;

    public virtual void Awake()
    {
        Effects = new List<Effect>();
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	public virtual void Update () {
	    foreach (var effect in Effects)
        {
            if (!effect.IsEffectComplete)
            {
                effect.ApplyEffect();
            }
        }
	}
}
