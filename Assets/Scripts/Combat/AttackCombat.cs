using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.DeveloperTools;

namespace RPG.Combat
{
    public class AttackCombat : MonoBehaviour, IAction, ISaveable, IModifierProvider {

        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        SlowLoad<Weapon> currentWeapon;

        private void Awake() {
            currentWeapon = new SlowLoad<Weapon>(SetDefaultWeapon);
        }

        private void Start() 
        {
            currentWeapon.Initialize();
        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (target.IsDead()) return;

            if (!GetIsInRange())
            {
                GetComponent<CharaterMovement>().MoveTo(target.transform.position, 1f);
            }
            else
            {
                GetComponent<CharaterMovement>().Cancel();
                AttackBehavoir();

            }
        }

        private Weapon SetDefaultWeapon()
        {
           AttachWeapon(defaultWeapon);
           return defaultWeapon;
        }

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null)
            {
                Debug.Log($"{name} is trying to equip a null weapon.");
            }
            currentWeapon.value = weapon;
            AttachWeapon(weapon);

        }

        private void AttachWeapon(Weapon weapon)
        {
            Animator animator = GetComponent<Animator>();
            weapon.Spawn(rightHandTransform, leftHandTransform, animator);
        }

        public Health GetTarget()
        {
             return target;
        }

        private void AttackBehavoir()
        {
            transform.LookAt(target.transform.position);

            if (timeSinceLastAttack > timeBetweenAttacks)
            {
                // This will do damage with the Hit trigger from animation
                TriggerAttack();
                timeSinceLastAttack = 0;

            }

        }

        private void TriggerAttack()
        {
            GetComponent<Animator>().ResetTrigger("stopAttack");
            GetComponent<Animator>().SetTrigger("attack");
        }

        private bool GetIsInRange()
        {
            return (Vector3.Distance(transform.position, target.transform.position) < currentWeapon.value.GetWeaponRange());
        }

        public bool CanAttack(GameObject attackTarget)
        {
            if (attackTarget == null)
            {
                return false;
            }
            Health targetToTest = attackTarget.GetComponent<Health>();
            return(targetToTest != null && !targetToTest.IsDead());
        }

        public void Attack(GameObject combatTarget)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.GetComponent<Health>();

        }

        public void Cancel()
        {
            StopAttack();
            target = null;
            GetComponent<CharaterMovement>().Cancel();
        }

        private void StopAttack()
        {
            GetComponent<Animator>().ResetTrigger("attack");
            GetComponent<Animator>().SetTrigger("stopAttack");
        }

        //Add animation hit
        void Hit()
        {
            if (target == null)
            {
                return;
            }

            float damage = GetComponent<BaseStats>().GetStat(Stat.Damage);
            
            if (currentWeapon.value.HasProjectile())
            {
                currentWeapon.value.LaunchProjectile(rightHandTransform,leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        public object CaptureState()
        {
            if (currentWeapon == null)
            {
                Debug.Log($"{name} does not have a weapon equipped in CaptureState()");
            }
            return currentWeapon.value.name;
        }

        public void RestoreState(object state)
        {
            string defaultWeaponName = (string) state;
            Weapon weapon = Resources.Load<Weapon>(defaultWeaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
           if (stat == Stat.Damage)
           {
               yield return currentWeapon.value.GetWeaponDamage();
           }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeapon.value.GetPercentageBonus();
            }
        }
    }
}