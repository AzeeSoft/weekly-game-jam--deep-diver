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
    public bool flipDirection = false;

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

    void LateUpdate()
    {
        var diverBounds = diverModel.collider.bounds;
        var moveableBounds = CameraRigManager.Instance.cinemachineBrain.OutputCamera.OrthographicBounds();

        var newExtents = moveableBounds.extents;
        newExtents.y -= yBottomOffset / 2f;
        var newCenter = moveableBounds.center;
        newCenter.y += yBottomOffset / 2f;
        moveableBounds.extents = newExtents;
        moveableBounds.center = newCenter;

        var dispToFit = diverBounds.DisplacementToFitInside(moveableBounds);
        dispToFit.z = 0;

        transform.position += dispToFit;
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

        float lerpValue = Time.deltaTime;
        if (dispToFit.magnitude > 0)
        {
            float origMoveDistance = Vector3.Distance(transform.position, targetPos);
            targetPos += dispToFit;

            float newMoveDistance = Vector3.Distance(transform.position, targetPos);
            lerpValue /= (newMoveDistance / origMoveDistance);
        }

        transform.position = Vector3.Lerp(transform.position, targetPos, lerpValue);

        if (flipDirection && Mathf.Abs(movement.x) > 0)
        {
            var newScale = transform.localScale;
            newScale.x = -Mathf.Sign(movement.x) * Mathf.Abs(transform.localScale.x);
            transform.localScale = newScale;
        }
    }
}
