using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnderwaterEntity : MonoBehaviour
{
    public Rigidbody2D rb2d { get; private set; }

    public float vertSpeed = 1f;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d.velocity = Vector2.up * vertSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
