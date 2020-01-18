using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(DiverModel))]
public class DiverMovementController : MonoBehaviour
{
    public float horSpeed = 3f;
    public float vertSpeed = 3f;
    public float yBottomOffset = 1f;

    private DiverModel diverModel;

    void Awake()
    {
        diverModel = GetComponent<DiverModel>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();        
    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(x * horSpeed, y * vertSpeed);
        Vector3 targetPos = transform.position + movement;

        var diverBounds = diverModel.collider.bounds;
        var moveableBounds = CameraRigManager.Instance.cinemachineBrain.OutputCamera.OrthographicBounds();

        var newExtents = moveableBounds.extents;
        newExtents.y -= yBottomOffset / 2f;
        var newCenter = moveableBounds.center;
        newCenter.y += yBottomOffset / 2f;
        moveableBounds.extents = newExtents;
        moveableBounds.center = newCenter;

        diverBounds.center = targetPos;

        var dispToFit = diverBounds.DisplacementToFitInside(moveableBounds);
        dispToFit.z = 0;

        if (dispToFit.magnitude > 0)
        {
            targetPos += dispToFit;
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime);
    }
}
