using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using UnityEngine.AI;
using System;
using RPG.Attributes;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;

        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        //[SerializeField] float attackSpeed = 3f;
        //[SerializeField] float patrolSpeed = 2f;


        AttackCombat attackCombat;
        GameObject player;
        Health health;
        CharaterMovement movement;
        NavMeshAgent navMeshAgent;

        Vector3 guardPosition;
        float timeSincLastSawPlayer = Mathf.Infinity;
        float timeSincArrivedAtWaypoint = Mathf.Infinity;
        int currentWaypointIndex = 0;

        private void Start() {
            attackCombat = GetComponent<AttackCombat>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            guardPosition = transform.position;
            movement = GetComponent<CharaterMovement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            if (health.IsDead())
            {
                return;
            }

            if (InAttackRangeOfPlayer() && attackCombat.CanAttack(player))
            {
               
                AttackBehavoir();
            }
            else if (timeSincLastSawPlayer < suspicionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolBehaviour();

            }

            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSincLastSawPlayer += Time.deltaTime;
            timeSincArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehaviour()
        {
            // navMeshAgent.speed = patrolSpeed;
            Vector3 nextPosition = guardPosition;
            if (patrolPath != null)
            {
                if (AtWaypoint())
                {
                    timeSincArrivedAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if ( timeSincArrivedAtWaypoint > waypointDwellTime)
            {
                movement.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
           
        }


//Methonds to help with enemy partoling 
        private Vector3 GetCurrentWaypoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.getNextWaypoint(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTolerance;
        }

// methonds to help have the enemy look for player for a short time or attack player
        private void SuspicionBehaviour()
        {
            // navMeshAgent.speed = attackSpeed;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavoir()
        {
            //navMeshAgent.speed = attackSpeed;
            timeSincLastSawPlayer = 0;
            attackCombat.Attack(player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return ( distanceToPlayer < chaseDistance);
        }

 //Called By unity for the programmer
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }

    }
}