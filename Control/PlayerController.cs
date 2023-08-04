using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Movement;
using RPG.Combat;
using RPG.Core;
using System;
using UnityEngine.AI;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        public bool canAttack;
        private Health health;

        private void Start()
        {
            health = GetComponent<Health>();
        }
        private void Update()
        {
            /*
             * Here we add priority to the intereact with combat boolean method
             * If InteractWithcombat returns true than we return and boot out of the update
             * else we continue onto the movement method.
             */
            if (health.IsDead()) return;
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) continue;
                if (!GetComponent<Fighter>().CanAttack(target.gameObject)) continue;
                 
                if(Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                return true;
            }
            //This returns false if target was never detected in the hits array.
            return false;
        }

        private bool InteractWithMovement()
        {
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if(hasHit)
            {
                if(Input.GetMouseButton(0) && !health.IsDead())
                {
                    GetComponent<Mover>().StartMoveAction(hit.point);
                }
                return true;
            }
            Debug.LogWarning("Area is not traversable");
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }
    }
}
