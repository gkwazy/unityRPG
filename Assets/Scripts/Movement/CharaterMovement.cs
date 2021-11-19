using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Core;
using RPG.Saving;
using RPG.HealthObject;

namespace RPG.Movement
{
    public class CharaterMovement : MonoBehaviour, IMode, ISaveable
    {
        [SerializeField] Transform target;
        [SerializeField] float maxSpeed = 5f;
        [SerializeField] float maxNavPathLength = 40f;

        NavMeshAgent navMeshAgent;
        Health health;

        Ray lastRay;

        private void Start() {
            
        }

        private void Awake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            health = GetComponent<Health>();
        }
        
        // Update is called once per frame
        void Update()
        {
            navMeshAgent.enabled = !health.Killed();
            playAnimator();
        }

        public void StartMoveAction(Vector3 destination, float speedFraction)
        {
            GetComponent<ActionContoller>().StartAction(this);
            Move(destination, speedFraction);
        }

        public void Move(Vector3 destination, float speedFraction)
        {
            navMeshAgent.destination = destination;
            navMeshAgent.speed = maxSpeed * Mathf.Clamp01(speedFraction);
            navMeshAgent.isStopped = false;
        }

        public void Cancel()
        {
            navMeshAgent.isStopped = true;
        }

        public bool AbleToMove(Vector3 destination)
        {
            NavMeshPath path = new NavMeshPath();
            bool hasPath = NavMesh.CalculatePath(transform.position, destination, NavMesh.AllAreas, path);
            if (!hasPath) return false;
            if (path.status != NavMeshPathStatus.PathComplete) return false;
            if (GetTravelDistance(path) > maxNavPathLength) return false;
            return true;
        }

        private float GetTravelDistance(NavMeshPath path)
        {
            float total = 0;
            if (path.corners.Length < 2) return total;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                total += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return total;
        }

        private void playAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        public object GetWeaponState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreWeaponState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            navMeshAgent.enabled = false;
            transform.position = position.ToVector();
            navMeshAgent.enabled = true;
            GetComponent<ActionContoller>().CancelCurrentAction();
        }
    }

}