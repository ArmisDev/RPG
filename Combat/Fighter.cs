using UnityEngine;
using RPG.Movement;
using RPG.Control;
using RPG.Core;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler), typeof(Animator))]
    public class Fighter : MonoBehaviour, IAction
    {
        [Header("Combat Parameters")]
        [SerializeField] private float weaponDamage = 25f;
        [SerializeField] private float weaponRange = 2f;

        [Header("Animation Parameters")]
        [SerializeField] private float timeBetweenAttacks;

        //Private Data
        private float timeSinceLastAttack = Mathf.Infinity;
        private Health target;
        private Health healthComponenet;
        private PlayerController playerController;
        private bool inRange;

        private void Start()
        {
            playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            //Moves Character to target and then stops when at the weaponRange.
            if (!GetIsInRange())
            {
                GetComponent<Mover>().MoveTo(target.transform.position);
            }
            else
            {
                GetComponent<Mover>().Cancel();
                AttackBehaviour();
            }
        }

        #region - Attack/Range Logic -
        /*Boolean which checks the distance between player & target position.
         * Returns true when the total sum of that is less then weapon range.
         */
        private bool GetIsInRange()
        {
            return Vector3.Distance(transform.position, target.transform.position) < weaponRange;
        }

        //----!Used By PlayerController Class!----
        //Assigns our target to a gameobject with combat target attached to it.
        public bool CanAttack(GameObject combatTarget)
        {
            if (combatTarget == null) return false;
            Health healthToTest = combatTarget.GetComponent<Health>();
            return healthToTest != null && !healthToTest.IsDead();
        }

        
        //This is a IAction event as it may need to be canceled when
        //clicking off the enemy to move to a non target position
        public void Attack(GameObject combatTarget)
        {
            target = combatTarget.GetComponent<Health>();
            GetComponent<ActionScheduler>().StartAction(this);
        }
        #endregion

        //IAction Event
        public void Cancel()
        {
            GetComponent<Animator>().SetTrigger("CancelAttack");
            target = null;
        }

        #region - Animation/Damage -
        void AttackBehaviour()
        {
            transform.LookAt(target.transform.position);

            if (timeSinceLastAttack > timeBetweenAttacks && !target.IsDead())
            {
                //This triggers the Hit() animation event.
                GetComponent<Animator>().SetTrigger("Attack");
                timeSinceLastAttack = 0f;
            }

            else return;
        }

        //Animation Event
        void Hit()
        {
            if (target == null) return;
            target.TakeDamage(weaponDamage);
        }
        #endregion
    }
}
