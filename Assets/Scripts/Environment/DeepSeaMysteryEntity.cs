using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepSeaMysteryEntity : EnvironmentEntity
{
    public bool stickToScreenBottom = true;

    private Collider2D collider2D;

    new void Awake()
    {
        base.Awake();
        collider2D = GetComponent<Collider2D>();
        collider2D.enabled = false;
    }

    void LateUpdate()
    {
        if (Time.timeScale > 0)
        {
            var colliderBottom = collider2D.bounds.GetSides().down.y;
            var camBoundsBottom = CameraRigManager.Instance.cinemachineBrain.OutputCamera
                .ViewportToWorldPoint(new Vector3(0, 0)).y;

            if (colliderBottom >= camBoundsBottom)
            {
                transform.position += Vector3.down * (colliderBottom - camBoundsBottom);
                collider2D.enabled = true;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diver"))
        {
            var diverModel = other.GetComponent<DiverModel>();
            diverModel.FoundDeepSeaMysteryObject(this);
        }
    }
}
