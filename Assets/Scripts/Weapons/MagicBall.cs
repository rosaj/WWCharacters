using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBall : MonoBehaviour
{
    public GameObject explodeEffect;

    // Use this for initialization
    void Start()
    {
        if(explodeEffect)  explodeEffect.SetActive(false);
      //  GetComponent<CombatWeapon>().OnDamage += Explode;
    }
    /*
    void Explode(GameObject source, GameObject enemy)
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        if(explodeEffect) explodeEffect.SetActive(true);
        Destroy(gameObject, 1);
    }*/
    private void OnTriggerEnter(Collider other)
    {
        var rb = GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        if (explodeEffect)
        {
            explodeEffect.SetActive(true);
            explodeEffect.transform.SetParent(transform.parent);
            Destroy(explodeEffect, 1);
        }
        Destroy(gameObject);

    }




}
