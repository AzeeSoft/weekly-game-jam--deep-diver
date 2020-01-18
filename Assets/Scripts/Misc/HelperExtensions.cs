﻿using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Cinemachine.Utility;
using UnityEngine;

public static class HelperExtensions
{
    #region MonoBehaviour

    public static void WaitAndExecute(this MonoBehaviour self, Action action, float delay)
    {
        self.StartCoroutine(HelperUtilities.WaitAndExecute(action, delay));
    }

    public static void WaitForFrameAndExecute(this MonoBehaviour self, Action action)
    {
        self.StartCoroutine(HelperUtilities.WaitForFrameAndExecute(action));
    }

    #endregion

    #region Transform

    public static Bounds TransformBounds(this Transform self, Bounds bounds)
    {
        var center = self.TransformPoint(bounds.center);
        var points = bounds.GetCorners();

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.TransformPoint(point));
        return result;
    }

    public static Bounds InverseTransformBounds(this Transform self, Bounds bounds)
    {
        var center = self.InverseTransformPoint(bounds.center);
        var points = bounds.GetCorners();

        var result = new Bounds(center, Vector3.zero);
        foreach (var point in points)
            result.Encapsulate(self.InverseTransformPoint(point));
        return result;
    }

    public static Bounds GetBoundsFromRenderers(this Transform self)
    {
        List<Renderer> renderers = new List<Renderer>();

        var selfRenderer = self.GetComponent<Renderer>();
        if (selfRenderer != null)
        {
            renderers.Add(selfRenderer);
        }

        renderers.AddRange(self.GetComponentsInChildren<Renderer>());

        Bounds bounds = new Bounds();
        if (renderers.Count > 0)
        {
            bounds = renderers[0].bounds;
            foreach (var r in renderers) bounds.Encapsulate(r.bounds);
        }

        return bounds;
    }

    public static void DestroyAllChildren(this Transform self)
    {
        foreach (Transform child in self)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    #endregion

    #region Bounds

    public struct Sides
    {
        public Vector3 left;
        public Vector3 right;
        public Vector3 up;
        public Vector3 down;
        public Vector3 forward;
        public Vector3 back;
    }

    public static Sides GetSides(this Bounds obj, bool includePosition = true) 
    {
        return new Sides()
        {
            left = (includePosition ? obj.center : Vector3.zero) + (Vector3.left * obj.extents.x),
            right = (includePosition ? obj.center : Vector3.zero) + (Vector3.right * obj.extents.x),
            up = (includePosition ? obj.center : Vector3.zero) + (Vector3.up * obj.extents.y),
            down = (includePosition ? obj.center : Vector3.zero) + (Vector3.down * obj.extents.y),
            forward = (includePosition ? obj.center : Vector3.zero) + (Vector3.forward * obj.extents.z),
            back = (includePosition ? obj.center : Vector3.zero) + (Vector3.back * obj.extents.z),
        };
    }

    public static List<Vector3> GetCorners(this Bounds obj, bool includePosition = true)
    {
        var result = new List<Vector3>();
        for (int x = -1; x <= 1; x += 2)
        for (int y = -1; y <= 1; y += 2)
        for (int z = -1; z <= 1; z += 2)
            result.Add((includePosition ? obj.center : Vector3.zero) + obj.extents.Times(new Vector3(x, y, z)));
        return result;
    }

    public static float GetVolume(this Bounds obj)
    {
        return obj.size.x * obj.size.y * obj.size.z;
    }

    public static Vector3 DisplacementToFitInside(this Bounds obj, Bounds other)
    {
        if (obj.GetVolume() > other.GetVolume())
        {
            return Vector3.zero;
        }

        var objSides = obj.GetSides();
        var otherSides = other.GetSides();

        Vector3 displacement = Vector3.zero;

        if (objSides.left.x < otherSides.left.x)
        {
            displacement.x = otherSides.left.x - objSides.left.x;
        }
        else if (objSides.right.x > otherSides.right.x)
        {
            displacement.x = otherSides.right.x - objSides.right.x;
        }

        if (objSides.up.y > otherSides.up.y)
        {
            displacement.y = otherSides.up.y - objSides.up.y;
        }
        else if (objSides.down.y < otherSides.down.y)
        {
            displacement.y = otherSides.down.y - objSides.down.y;
        }

        if (objSides.forward.z > otherSides.forward.z)
        {
            displacement.z = otherSides.forward.z - objSides.forward.z;
        }
        else if (objSides.back.z < otherSides.back.z)
        {
            displacement.z = otherSides.back.z - objSides.back.z;
        }

        return displacement;
    }

    #endregion

    #region Vector3

    public static Vector3 Times(this Vector3 self, Vector3 other)
    {
        return new Vector3(self.x * other.x, self.y * other.y, self.z * other.z);
    }

    #endregion

    #region Camera

    public static Bounds OrthographicBounds(this Camera camera)
    {
        float screenAspect = (float) Screen.width / (float) Screen.height;
        float cameraHeight = camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    #endregion

    #region CinemachinePOV

    public static void ResetRotation(this CinemachinePOV self, Quaternion targetRot)
    {
        Vector3 up = self.VcamState.ReferenceUp;
        Vector3 fwd = Vector3.forward;
        Transform parent = self.VirtualCamera.transform.parent;
        if (parent != null)
            fwd = parent.rotation * fwd;

        self.m_HorizontalAxis.Value = 0;
        self.m_HorizontalAxis.Reset();
        Vector3 targetFwd = targetRot * Vector3.forward;
        Vector3 a = fwd.ProjectOntoPlane(up);
        Vector3 b = targetFwd.ProjectOntoPlane(up);
        if (!a.AlmostZero() && !b.AlmostZero())
            self.m_HorizontalAxis.Value = Vector3.SignedAngle(a, b, up);

        self.m_VerticalAxis.Value = 0;
        self.m_VerticalAxis.Reset();
        fwd = Quaternion.AngleAxis(self.m_HorizontalAxis.Value, up) * fwd;
        Vector3 right = Vector3.Cross(up, fwd);
        if (!right.AlmostZero())
            self.m_VerticalAxis.Value = Vector3.SignedAngle(fwd, targetFwd, right);
    }

    #endregion
}