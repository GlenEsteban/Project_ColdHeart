using System;
using UnityEngine;

namespace coldheart_combat
{
    [RequireComponent(typeof(AbilityRunner))]
    public class Combat : MonoBehaviour {
        [SerializeField] [Range (.02f, 10f)] float instantAbilityCoolDown;
        [SerializeField] [Range (.02f, 10f)] float chargeUpAbilityChargeTime;
        public void SetIsChargingUpAbility(bool state) {
            isChargingUpAbility = state;
        } 
        AbilityRunner abilityRunner;
        float timeSinceLastInstantAbility = Mathf.Infinity;
        bool isChargingUpAbility;
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
    }
}