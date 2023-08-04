using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent), typeof(Animator), typeof(ActionScheduler))]
    public class Mover : MonoBehaviour, IAction
    {
        public NavMeshAgent navMeshAgent;
        [SerializeField] private Animator animator;

        private Health health;

        private void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            animator = GetComponent<Animator>();
            health = GetComponent<Health>();
        }
        // Update is called once per frame
        private void Update()
        {
            navMeshAgent.enabled = !health.IsDead();
            //Update animation
            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination);
        }

        public void MoveTo(Vector3 destination)
        {
            navMeshAgent.isStopped = false;
            navMeshAgent.SetDestination(destination);
        }

        //Public cancel method which connects to IAction
        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        private void UpdateAnimator()
        {
            /*
             * Goal: Get the velocity of our NavMeshAgent (represented in world space) and deliver it to a Vector3.
             * The new Vector3 should represent the players local position.
             * The forward postion of the localized vector (ie. Z axis) will then be passed into a float.
             * This float shall represent the speed, or velocity.
             */
            Vector3 velocity = navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;

            //Grab Animator and equate our speed variable to the ForwardMovement float in our BlendTree
            animator.SetFloat("ForwardMovement", speed);
        }
    }
}
