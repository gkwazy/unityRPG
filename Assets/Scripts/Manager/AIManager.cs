using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Fighting;
using RPG.Core;
using RPG.Movement;
using UnityEngine.AI;
using System;
using RPG.HealthObject;
using RPG.DeveloperTools;

namespace RPG.Manager
{
    public class AIManager : MonoBehaviour
    {
        [SerializeField] float pursuitDistance = 5f;
        [SerializeField] float aggressionTime = 3f;
        [SerializeField] float aggroCoolDown = 3f;
        [SerializeField] float waypointDwellTime = 3f;
        [SerializeField] PatrolRoute patrolRoute;
        [SerializeField] float waypointTime = 1f;
        [SerializeField] float allyAggro = 3f;

        [Range(0,1)]
        [SerializeField] float patrolSpeedFraction = 0.2f;

        //[SerializeField] float attackSpeed = 3f;
        //[SerializeField] float patrolSpeed = 2f;


        AttackCombat attackCombat;
        GameObject player;
        Health health;
        CharaterMovement movement;
        NavMeshAgent navMeshAgent;

        SlowLoad<Vector3> guardPosition;
        float timePlayerSeen = Mathf.Infinity;
        float timeAtWaypoint = Mathf.Infinity;
        float timeAggrevated = Mathf.Infinity;
        int currentWaypointIndex = 0;


        private void Awake() {
            attackCombat = GetComponent<AttackCombat>();
            player = GameObject.FindWithTag("Player");
            health = GetComponent<Health>();
            movement = GetComponent<CharaterMovement>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            guardPosition = new SlowLoad<Vector3>(GetEnemyPosition);
        }

        private void Start() {
            guardPosition.Initialize();
        }

        private void Update()
        {
            if (health.Killed())
            {
                return;
            }
            print(IsAggrevated());
            if (IsAggrevated() && attackCombat.AbleToFight(player))
            {
                AttackMode();
            }
            else if (timePlayerSeen < aggressionTime)
            {
                SuspicionBehaviour();
            }
            else
            {
                PatrolMode();
            }

            UpdateTimers();
        }

        private Vector3 GetEnemyPosition()
        {
            return transform.position;
        }

        private void UpdateTimers()
        {
            timePlayerSeen += Time.deltaTime;
            timeAtWaypoint += Time.deltaTime;
            timeAggrevated += Time.deltaTime;
        }

        private void PatrolMode()
        {
            // navMeshAgent.speed = patrolSpeed;
            Vector3 nextPosition = guardPosition.value;
            if (patrolRoute != null)
            {
                if (AtWaypoint())
                {
                    timeAtWaypoint = 0f;
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWaypoint();
            }
            if ( timeAtWaypoint > waypointDwellTime)
            {
                movement.StartMoveAction(nextPosition, patrolSpeedFraction);
            }
           
        }


//Methonds to help with enemy partoling 
        private Vector3 GetCurrentWaypoint()
        {
            return patrolRoute.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolRoute.getNextWaypoint(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWaypoint < waypointTime;
        }

// methonds to help have the enemy look for player for a short time or attack player
        private void SuspicionBehaviour()
        {
            // navMeshAgent.speed = attackSpeed;
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackMode()
        {
            //navMeshAgent.speed = attackSpeed;
            timePlayerSeen = 0;
            attackCombat.Attack(player);
            AggrevateNearbyEnemies();

        }

        private bool IsAggrevated()
        {
            float distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
            return ( distanceToPlayer < pursuitDistance || timeAggrevated < aggroCoolDown);
        }

        private void AggrevateNearbyEnemies()
        {
          RaycastHit[] hits = Physics.SphereCastAll(transform.position, allyAggro, Vector3.up, 0);
          foreach (RaycastHit hit in hits)
          {
              AIManager ai = hit.collider.GetComponent<AIManager>();
              if (ai == null) continue;

              ai.Aggrevate();
          }

        }

 //Called By unity for the programmer
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(transform.position, pursuitDistance);
        }

        public void Aggrevate()
       
        {
            timeAggrevated = 0;
        }

    }
}