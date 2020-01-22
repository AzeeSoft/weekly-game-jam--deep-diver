using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PowerUp : MonoBehaviour
{
    [Serializable]
    public class PowerUpEvent : UnityEvent<DiverModel>
    {
    }

    public AudioClip powerUpSound;
    public PowerUpEvent onPowerUpPicked;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Diver"))
        {
            var diverModel = other.GetComponent<DiverModel>();
            onPowerUpPicked?.Invoke(diverModel);
            SoundEffectsManager.Instance.Play(powerUpSound);
            Destroy(gameObject);
        }
    }
}