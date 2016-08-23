using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Health : BaseAttribute
    {
        public Health(float maxValue)
        {
            _maxValue = maxValue;
            _currentValue = maxValue;
        }

        public bool IsDead
        {
            get
            {
                return _currentValue <= 0;
            }
        }

        public void Damage(float damageAmount)
        {
            var _previousHealth = _currentValue;
            _currentValue -= damageAmount;
            Debug.Log("Health changed from " + _previousHealth + " to " + _currentValue + ".");
        }

        public void Heal(float healAmount)
        {
            _currentValue += healAmount;
            if (_currentValue > _maxValue)
            {
                _currentValue = _maxValue;
            }
        }
    }
}
