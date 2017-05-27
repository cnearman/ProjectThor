using UnityEngine;

namespace Assets.Scripts
{
    public class Health : BaseAttribute
    {
        public bool IsDead
        {
            get
            {
                return currentValue <= 0;
            }
        }

        public void Damage(float damageAmount)
        {
            var _previousHealth = currentValue;
            currentValue -= damageAmount;
            Debug.Log("Health changed from " + _previousHealth + " to " + currentValue + ".");
        }

        public void Heal(float healAmount)
        {
            currentValue += healAmount;
            if (currentValue > maxValue)
            {
                currentValue = maxValue;
            }
        }
    }
}
