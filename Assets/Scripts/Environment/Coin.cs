using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int value = 100;
    public float moveDuration = 1f;

    private bool isActive = true;

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
        if (isActive && other.CompareTag("Diver"))
        {
            isActive = false;

            var diverModel = other.GetComponent<DiverModel>();
            MoveToDiver(diverModel);
        }
    }

    void MoveToDiver(DiverModel diverModel)
    {
        var seq = DOTween.Sequence();
        var moveTween = transform.DOMove(diverModel.transform.position, moveDuration);
        var scaleTween = transform.DOScale(Vector3.zero, moveDuration);

        seq.Insert(0, moveTween);
        seq.Insert(0, scaleTween);

        seq.AppendCallback(() => OnCoinCollected(diverModel));

        seq.Play();
    }

    void OnCoinCollected(DiverModel diverModel)
    {
        diverModel.UpdateCoins(value);

        Destroy(gameObject);
    }
}