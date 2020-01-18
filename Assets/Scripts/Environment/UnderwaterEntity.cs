using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnderwaterEntity : EnvironmentEntity
{
    public Rigidbody2D rb2d { get; private set; }

    public Vector2 defaultVelocity = Vector2.zero;

    new void Awake()
    {
        base.Awake();

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        rb2d.velocity = defaultVelocity;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
