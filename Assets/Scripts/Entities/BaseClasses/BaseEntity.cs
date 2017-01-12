using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using System;

public class BaseEntity : MonoBehaviour {
    public IList<Effect> Effects;

    private delegate void OnUpdateMethodsType();
    OnUpdateMethodsType OnUpdateMethods;

    private delegate void OnAwakeMethodsType();
    OnAwakeMethodsType OnAwakeMethods;

    private delegate void OnStartMethodsType();
    OnStartMethodsType OnStartMethods;

    private delegate void OnFixedUpdateMethodsType();
    OnFixedUpdateMethodsType OnFixedUpdateMethods;

    public virtual void Awake()
    {
        Effects = new List<Effect>();
        OnUpdateMethods = (OnUpdateMethodsType) AddAttributedMethodsToDelegate<OnUpdateAttribute, OnUpdateMethodsType>();
        OnAwakeMethods = (OnAwakeMethodsType)AddAttributedMethodsToDelegate<OnAwakeAttribute, OnAwakeMethodsType>();
        OnStartMethods = (OnStartMethodsType)AddAttributedMethodsToDelegate<OnStartAttribute, OnStartMethodsType>();
        OnFixedUpdateMethods = (OnFixedUpdateMethodsType)AddAttributedMethodsToDelegate<OnFixedUpdateAttribute, OnFixedUpdateMethodsType>();
        if (OnAwakeMethods != null)
        {
            OnAwakeMethods.Invoke();
        }
    }

	// Use this for initialization
	public virtual void Start () {
	    if (OnStartMethods != null)
        {
            OnStartMethods.Invoke();
        }
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
        if (OnUpdateMethods != null)
        {
            OnUpdateMethods.Invoke();
        }
	}

    public virtual void FixedUpdate()
    {
        //Debug.Log("Base FixedUpdate");
        if (OnFixedUpdateMethods != null)
        {
            OnFixedUpdateMethods.Invoke();
        }
    }

    /// <summary>
    /// Searches the class type that called this method for all methods with Attribute T and creates new delegates of type F.
    /// </summary>
    /// <typeparam name="T">Type of Attribute to search for.</typeparam>
    /// <typeparam name="F">Type of Delegate to be created.</typeparam>
    /// <returns>Delegate of all methods with attribute found combined.</returns>
    private Delegate AddAttributedMethodsToDelegate<T, F>() 
    {
        Delegate output = null;
        try
        {
            var delegates = GetType()
                .GetMethods()
                .Where(m => m.GetCustomAttributes(typeof(T), true).Any())
                .Select(updateMethodInfo => Delegate.CreateDelegate(typeof(F), this, updateMethodInfo))
                .ToArray();
            output = Delegate.Combine(delegates);
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
        return output;
    }
}
