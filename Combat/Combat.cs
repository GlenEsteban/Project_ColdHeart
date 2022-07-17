using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace coldheart_combat {
    [RequireComponent(typeof(AbilityRunner))]
    public class Combat : MonoBehaviour {
        AbilityRunner abilityRunner;
        void Start() {
            abilityRunner = GetComponent<AbilityRunner>();
        }
        public void CallInstantAbility() {
            // if time since last used ability is greater than ability hold time, use ability.
            abilityRunner.UseInstantAbility();
        }
        public void CallChargedAbility() {
            abilityRunner.UseChargedAbility();
        }
    }
}
