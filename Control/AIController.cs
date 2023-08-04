using UnityEngine;
using System.Collections;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine.AI;
using System;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private float suspsionStateTime = 7f;
        [SerializeField] private float chaseRange = 5f;
        [SerializeField] private GameObject target;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float partolWaitime;
        [SerializeField] private float wayPointTolerence = 1f;
        [SerializeField] [Tooltip("Set false if attaching a patrol path")] private bool isGuarding = true;

        private Fighter fighter;
        private Health health;
        private Mover mover;
        private NavMeshAgent navMeshAgent;

        //Chase Timer
        private float timeSinceLastSawPlayer = Mathf.Infinity;

        //Private patrol timer
        public float patrolTimeIncrement = 0f;

        //Ammount position can be off
        private float positionOffsetTolerence = 0.15f;

        private Vector3 guardPosition;
        private Vector3 lastPosition;
        private Quaternion guardRotation;
        private int currentIndexWaypoint = 0;

        private float currentAISpeed;
        private float returnSpeed = 1.895f;

        private void Start()
        {
            //Grabs fighter class
            fighter = GetComponent<Fighter>();
            //Assigns player as target
            target = GameObject.FindGameObjectWithTag("Player");
            //Grabs Health
            health = GetComponent<Health>();
            mover = GetComponent<Mover>();

            guardPosition = transform.position;
            guardRotation = transform.rotation;

            navMeshAgent = GetComponent<NavMeshAgent>();
            currentAISpeed = navMeshAgent.speed;
        }

        private void Update()
        {
            if (health.IsDead()) return;
            //Checks to see if AI should chase
            if (InAttackRange() && fighter.CanAttack(target))
            {
                //Sets AI speed to base speed if it was resetting during guard behavior
                if(navMeshAgent.speed != currentAISpeed)
                {
                    navMeshAgent.speed = currentAISpeed;
                }

                AttackBehavior();
                timeSinceLastSawPlayer = 0f;
            }

            else if(timeSinceLastSawPlayer < suspsionStateTime)
            {
                SuspsionBehavior();
            }

            else
            {
                PatrolBehavior();
            }

            timeSinceLastSawPlayer += Time.deltaTime;
        }

        private void AttackBehavior()
        {
            fighter.Attack(target);
        }

        private void SuspsionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = guardPosition;

            if(patrolPath != null)
            {
                if(AtWayPoint())
                {
                    //Cycle to next waypoint
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }

            mover.StartMoveAction(nextPosition);

            //Rotates player if they have moved back to their position
            if (isGuarding)
            {
                navMeshAgent.speed = returnSpeed;

                if (HasReturnedFromChase() && transform.rotation != guardRotation)
                {
                    Debug.Log("Should Rotate");
                    transform.rotation = guardRotation;
                }
            }
        }

        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentIndexWaypoint);
        }

        private void CycleWaypoint()
        {
            currentIndexWaypoint = patrolPath.GetNextIndex(currentIndexWaypoint);
        }

        private bool AtWayPoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < wayPointTolerence;
        }

        //Checks to see if player is within attacking range.
        private bool InAttackRange()
        {
            float distanceCheck = Vector3.Distance(transform.position, target.transform.position);
            return distanceCheck < chaseRange;
        }

        //Returns true if AI is near their original position
        //Essentially is equivilent to Mathf.Approximetly
        private bool HasReturnedFromChase()
        {
            float xDifference = Mathf.Abs(transform.position.x - guardPosition.x);
            return xDifference <= positionOffsetTolerence;
        }

        //Called by Unity
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chaseRange);
        }
    }
}