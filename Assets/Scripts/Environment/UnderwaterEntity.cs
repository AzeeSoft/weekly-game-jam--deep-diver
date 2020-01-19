using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class UnderwaterEntity : EnvironmentEntity
{
    public Rigidbody2D rb2d { get; private set; }

    public float speed = 0.9f;
    public float speedModifier = 0.3f;

    new void Awake()
    {
        base.Awake();

        rb2d = GetComponent<Rigidbody2D>();
        rb2d.gravityScale = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMoveDirection(Vector3 dir)
    {
        rb2d.velocity = dir.normalized * (speed + Random.Range(-speedModifier, speedModifier));

        var newScale = transform.localScale;
        newScale.x = Mathf.Sign(rb2d.velocity.x) * Mathf.Abs(transform.localScale.x);
        transform.localScale = newScale;

        var facingDir = rb2d.velocity.normalized * Mathf.Sign(transform.localScale.x);
        //transform.right = facingDir;
    }
}