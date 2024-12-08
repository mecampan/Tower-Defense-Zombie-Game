using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Shot : MonoBehaviour
{
    // public GameObject hitPrefab;
    // public GameObject muzzlePrefab;
    public float speed;
    public float range = 3;
    Rigidbody rb;
    Vector3 velocity;

    void Awake()
    {
        TryGetComponent(out rb);
    }

    void Start()
    {
        // var muzzleEffect = Instantiate(muzzlePrefab, transform.position, transform.rotation);
        // Destroy(muzzleEffect, 5f);
        velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        var displacement = velocity * Time.deltaTime;
        rb.MovePosition(rb.position + displacement);
        //if (Mathf.Abs(rb.position.x) > (range + 1) || Mathf.Abs(rb.position.y) > (range + 1) || Mathf.Abs(rb.position.z) > (range + 1)){
        //    Destroy(gameObject);
        //}
    }

    void OnCollisionEnter(Collision other)
    {
        // var hitEffect = Instantiate(hitPrefab, other.GetContact(0).point, Quaternion.identity);
        // Destroy(hitEffect, 5f);
        //print("other object: " + other.gameObject.ToString());
        Destroy(gameObject);
    }

}