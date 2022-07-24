using UnityEngine;

namespace coldheart_combat {
    public class ThrowAbility : MonoBehaviour, IAbility
    {
        [SerializeField] abilityTypes abilityType;
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] Transform projectileSpawnPoint;
        [SerializeField] float throwForce = 10f;

        void Start() {
            GetComponent<AbilityRunner>().AssignAbility(this, abilityType);
        }
        public void Use(GameObject currentGameObject)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, gameObject.transform.rotation, gameObject.transform);
            projectile.GetComponent<Rigidbody>().velocity += projectile.transform.forward * throwForce;
            if (tag == "Player") {
                projectile.GetComponent<Projectile>().SetIsAPlayerProjectile();
            }
            else if (tag == "Enemy") {
                projectile.GetComponent<Projectile>().SetIsAPlayerProjectile();
            }
            projectile.transform.parent = null;
        }
    }
}