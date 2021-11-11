using UnityEngine;
using RPG.Movement;
using RPG.Core;
using System;
using RPG.Saving;
using RPG.Attributes;
using RPG.Stats;
using System.Collections.Generic;
using RPG.DeveloperTools;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class AttackCombat : MonoBehaviour, IAction, ISaveable, IModifierProvider {

        
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeapon = null;
        [SerializeField] Boolean canEquipWeapon;
        [SerializeField] UnityEvent noWeaponAttackSound;
        
        Health target;
        float timeSinceLastAttack = Mathf.Infinity;
        WeaponConfig currentWeaponConfig;
        SlowLoad<Weapon> currentWeapon;

        private void Awake() {
            currentWeaponConfig = defaultWeapon;
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

            if (!GetIsInRange(target.transform))
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
          return AttachWeapon(defaultWeapon);
        }

        public void EquipWeapon(WeaponConfig weapon)
        {
            currentWeaponConfig = weapon;
            currentWeapon.value = AttachWeapon(weapon);

        }

        private Weapon AttachWeapon(WeaponConfig weapon)
        {
            Animator animator = GetComponent<Animator>();
            return weapon.Spawn(rightHandTransform, leftHandTransform, animator);
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

        private bool GetIsInRange(Transform targetTransform)
        {
            return (Vector3.Distance(transform.position, targetTransform.position) < currentWeaponConfig.GetWeaponRange());
        }

        public bool CanAttack(GameObject attackTarget)
        {
            if (attackTarget == null)
            {
                return false;
            }
            if (!GetComponent<CharaterMovement>().CanMoveTo(attackTarget.transform.position) &&
                !GetIsInRange(attackTarget.transform)) 
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
            
            if (currentWeapon.value != null)
            {
               currentWeapon.value.OnHit(); 
            }
            else
            {
                noWeaponAttackSound.Invoke();
            }

            if (currentWeaponConfig.HasProjectile())
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform,leftHandTransform, target, gameObject, damage);
            }
            else
            {
                target.TakeDamage(gameObject, damage);
            }
        }

        public object CaptureState()
        {
            if (currentWeaponConfig == null)
            {
                Debug.Log($"{name} does not have a weapon equipped in CaptureState()");
            }
            return currentWeaponConfig.name;
        }

        public void RestoreState(object state)
        {
            string defaultWeaponName = (string) state;
            WeaponConfig weapon = Resources.Load<WeaponConfig>(defaultWeaponName);
            EquipWeapon(weapon);
        }

        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
           if (stat == Stat.Damage)
           {
               yield return currentWeaponConfig.GetWeaponDamage();
           }
        }

        public IEnumerable<float> GetPercentageModifiers(Stat stat)
        {
            if (stat == Stat.Damage)
            {
                yield return currentWeaponConfig.GetPercentageBonus();
            }
        }
    }
}