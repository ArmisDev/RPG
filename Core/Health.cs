using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private float health = 100f;
        public bool hasDied = false;

        public void TakeDamage(float damage)
        {
            health = Mathf.Max(health - damage, 0);
            if(health == 0)
            {
                HandleDeath();
            }
        }

        public bool IsDead()
        {
            return hasDied;
        }

        void HandleDeath()
        {
            if (hasDied) return;

            hasDied = true;
            GetComponent<Animator>().SetTrigger("Killed");
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }
    }
}
