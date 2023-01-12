using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public CombatController Target;
    public float Damage;
    [SerializeField] float projectileSpeed;


    private void Update()
    {
        transform.position += transform.forward * projectileSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Target.gameObject)
        {
            Target.TakeDamage(Damage);
        }
        Destroy(this.gameObject);
    }
}
