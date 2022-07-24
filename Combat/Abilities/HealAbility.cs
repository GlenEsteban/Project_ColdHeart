using coldheart_core;
using UnityEngine;
namespace coldheart_combat
{
    public class HealAbility : MonoBehaviour, IAbility {
        [SerializeField] abilityTypes abilityType;
        [SerializeField] float healAmount;
        Health health;
        void Awake() {
            health = GetComponent<Health>();
        }
        void Start() {
            GetComponent<AbilityRunner>().AssignAbility(this, abilityType);
        }
        public void Use(GameObject currentGameObject) {
            health.RestoreHealth(healAmount);
        }
    }
}