using coldheart_core;
using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] float projectileDamage;
    [SerializeField] float projectileDecayTime = 3f;
    [SerializeField] bool isAbleToStickToObjects;
    public void SetIsAPlayerProjectile() {
        isAPlayerProjectile = true;
    }
    public void SetIsAnEnemyProjectile() {
        isAnEnemyProjectile = true;
    }
    bool isAPlayerProjectile;
    bool isAnEnemyProjectile;
    void OnCollisionEnter(Collision other) {
        if (isAnEnemyProjectile && other.gameObject.tag == "Player") {
            other.gameObject.GetComponent<Health>().ReduceHealth(projectileDamage);
        }
        else if (isAPlayerProjectile && other.gameObject.tag == "Enemy")
        {
            other.gameObject.GetComponent<Health>().ReduceHealth(projectileDamage);
        }

        if (isAbleToStickToObjects) {
            StickToObject(other.gameObject);
        }

        Destroy(gameObject, projectileDecayTime);
    }
    void StickToObject(GameObject objectToStickTo) {
        gameObject.transform.parent = objectToStickTo.transform;
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        Destroy(rigidbody);
    }
}