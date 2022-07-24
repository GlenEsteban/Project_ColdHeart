using System;
using System.Collections;
using UnityEngine;

namespace coldheart_combat
{
    [RequireComponent(typeof(AbilityRunner))]
    public class Combat : MonoBehaviour {
        [SerializeField] [Range(0f, 1f)] float instantToChargeUpRatio = 0.5f;
        [SerializeField] [Range (.02f, 10f)] float instantAbilityCoolDown;
        [SerializeField] [Range (.02f, 10f)] float chargeUpAbilityChargeTime;
        public void SetIsChargingUpAbility(bool state) {
            isChargingUpAbility = state;
        } 
        public void SetInstantToChargeUpRatio(float ratio) {
            if (ratio > 1 || ratio < 0) {return;}

            instantToChargeUpRatio = ratio;
        }
        AbilityRunner abilityRunner;
        bool isChargingUpAbility;
        float timeSinceLastInstantAbility = Mathf.Infinity;
        float timerForChargeUpAbilityChargeUpTime;
        void Awake() {
            abilityRunner = GetComponent<AbilityRunner>();
        }
        void Update() {
            timeSinceLastInstantAbility += Time.deltaTime;
            
            ControlChargeUpAbility();
        }
        public void CallThrottledInstantAbility() {
            if (timeSinceLastInstantAbility >= instantAbilityCoolDown) {
                abilityRunner.UseInstantAbility();
                timeSinceLastInstantAbility = 0;
            }
        }
        public void ControlChargeUpAbility() {
            if(isChargingUpAbility) {
                timerForChargeUpAbilityChargeUpTime += Time.deltaTime;
                if (timerForChargeUpAbilityChargeUpTime > chargeUpAbilityChargeTime) {
                    abilityRunner.UseChargeUpAbility();
                    timerForChargeUpAbilityChargeUpTime = 0;
                }
            }
            else {
                timerForChargeUpAbilityChargeUpTime = 0;
            }
        }
        public void DecideAttackTypeBasedOnRatio() {
            if (isChargingUpAbility) {return;}
            if (timeSinceLastInstantAbility < instantAbilityCoolDown) {return;}

            bool chooseInstantAbility = (UnityEngine.Random.value < instantToChargeUpRatio);
            if (chooseInstantAbility == true) {
                abilityRunner.UseInstantAbility();
                timeSinceLastInstantAbility = 0;
                print(gameObject + " is using instant abi!");
            }
            else {
                StartCoroutine("CallChargeUpAbilityOnce");
                print(gameObject + " is using charge abi!");
            }
        }
        IEnumerator CallChargeUpAbilityOnce() {
            if (!isChargingUpAbility) {
                isChargingUpAbility = true;
                yield return new WaitForSeconds(chargeUpAbilityChargeTime);
                isChargingUpAbility = false;
            }
        }
    }
}