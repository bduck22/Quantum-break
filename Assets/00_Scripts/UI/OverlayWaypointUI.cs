using System.Collections.Generic;
using UnityEngine;

public class OverlayWaypointUI : MonoBehaviour
{
    private class RuntimeWaypoint
    {
        public Transform TargetPosition;
        public WaveSpawnData WaveSpawnData;
        public RectTransform WaypointRect;
    }

    [Header("References")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private RectTransform canvasRect;
    [SerializeField] private RectTransform waypointPrefab;
    [SerializeField] private Transform player;

    [Header("Single World Info UI")]
    [SerializeField] private SpawnPointWaveInfoUI spawnPointWaveInfoUI;

    [Header("Pool")]
    [SerializeField] private int initialPoolSize = 8;

    [Header("Distance")]
    [SerializeField] private float infoOpenDistance = 5f;

    [Header("Virtual Screen Edge")]
    [SerializeField] private float virtualEdgeDistanceFromCenter = 360f;
    [SerializeField] private float screenMargin = 80f;
    [SerializeField] private float behindCenterThreshold = 10f;

    [Header("Rotation")]
    [SerializeField] private bool rotateToTargetDirection = true;

    // 화살표 이미지가 기본적으로 위쪽을 보고 있으면 -90,
    // 오른쪽을 보고 있으면 0으로 두면 됨.
    [SerializeField] private float iconAngleOffset = -90f;

    private readonly List<RuntimeWaypoint> runtimeWaypoints = new List<RuntimeWaypoint>();
    private readonly Stack<RectTransform> waypointPool = new Stack<RectTransform>();

    private RuntimeWaypoint currentOpenedWaypoint;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        CreateInitialPool();

        if (spawnPointWaveInfoUI != null)
        {
            spawnPointWaveInfoUI.Close();
        }
    }

    private void Update()
    {
        if (!IsReady())
        {
            return;
        }

        RuntimeWaypoint nearestWaypoint = GetNearestWaypointInDistance();

        UpdateInfoUI(nearestWaypoint);
        UpdateAllWaypointIcons(nearestWaypoint);
    }

    public void AddTarget(Transform targetPosition, WaveSpawnData waveSpawnData)
    {
        if (targetPosition == null)
        {
            return;
        }

        if (waypointPrefab == null || canvasRect == null)
        {
            return;
        }

        RectTransform waypointRect = GetWaypointFromPool();

        RuntimeWaypoint runtimeWaypoint = new RuntimeWaypoint
        {
            TargetPosition = targetPosition,
            WaveSpawnData = waveSpawnData,
            WaypointRect = waypointRect
        };

        runtimeWaypoints.Add(runtimeWaypoint);
    }

    public void RemoveTarget(Transform targetPosition)
    {
        if (targetPosition == null)
        {
            return;
        }

        for (int i = runtimeWaypoints.Count - 1; i >= 0; i--)
        {
            RuntimeWaypoint runtimeWaypoint = runtimeWaypoints[i];

            if (runtimeWaypoint.TargetPosition != targetPosition)
            {
                continue;
            }

            if (currentOpenedWaypoint == runtimeWaypoint)
            {
                if (spawnPointWaveInfoUI != null)
                {
                    spawnPointWaveInfoUI.Close();
                }

                currentOpenedWaypoint = null;
            }

            if (runtimeWaypoint.WaypointRect != null)
            {
                ReturnWaypointToPool(runtimeWaypoint.WaypointRect);
            }

            runtimeWaypoints.RemoveAt(i);
        }
    }

    public void ClearTargets()
    {
        if (spawnPointWaveInfoUI != null)
        {
            spawnPointWaveInfoUI.Close();
        }

        currentOpenedWaypoint = null;

        for (int i = 0; i < runtimeWaypoints.Count; i++)
        {
            RuntimeWaypoint runtimeWaypoint = runtimeWaypoints[i];

            if (runtimeWaypoint.WaypointRect != null)
            {
                ReturnWaypointToPool(runtimeWaypoint.WaypointRect);
            }
        }

        runtimeWaypoints.Clear();
    }

    private bool IsReady()
    {
        if (targetCamera == null)
        {
            return false;
        }

        if (canvasRect == null)
        {
            return false;
        }

        if (waypointPrefab == null)
        {
            return false;
        }

        if (player == null)
        {
            return false;
        }

        return true;
    }

    private void CreateInitialPool()
    {
        if (waypointPrefab == null || canvasRect == null)
        {
            return;
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            RectTransform waypointRect = Instantiate(waypointPrefab, canvasRect);
            ReturnWaypointToPool(waypointRect);
        }
    }

    private RectTransform GetWaypointFromPool()
    {
        RectTransform waypointRect;

        if (waypointPool.Count > 0)
        {
            waypointRect = waypointPool.Pop();
        }
        else
        {
            waypointRect = Instantiate(waypointPrefab, canvasRect);
        }

        waypointRect.SetParent(canvasRect, false);
        waypointRect.localScale = Vector3.one;
        waypointRect.rotation = Quaternion.identity;
        waypointRect.anchoredPosition = Vector2.zero;
        waypointRect.gameObject.SetActive(true);

        return waypointRect;
    }

    private void ReturnWaypointToPool(RectTransform waypointRect)
    {
        if (waypointRect == null)
        {
            return;
        }

        waypointRect.gameObject.SetActive(false);
        waypointRect.SetParent(canvasRect, false);
        waypointRect.localScale = Vector3.one;
        waypointRect.rotation = Quaternion.identity;
        waypointRect.anchoredPosition = Vector2.zero;

        waypointPool.Push(waypointRect);
    }

    private RuntimeWaypoint GetNearestWaypointInDistance()
    {
        RuntimeWaypoint nearestWaypoint = null;
        float nearestSqrDistance = infoOpenDistance * infoOpenDistance;

        for (int i = 0; i < runtimeWaypoints.Count; i++)
        {
            RuntimeWaypoint runtimeWaypoint = runtimeWaypoints[i];

            if (runtimeWaypoint == null || runtimeWaypoint.TargetPosition == null)
            {
                continue;
            }

            float sqrDistance = (player.position - runtimeWaypoint.TargetPosition.position).sqrMagnitude;

            if (sqrDistance > nearestSqrDistance)
            {
                continue;
            }

            nearestSqrDistance = sqrDistance;
            nearestWaypoint = runtimeWaypoint;
        }

        return nearestWaypoint;
    }

    private void UpdateInfoUI(RuntimeWaypoint nearestWaypoint)
    {
        if (spawnPointWaveInfoUI == null)
        {
            return;
        }

        if (nearestWaypoint == null)
        {
            if (currentOpenedWaypoint != null)
            {
                spawnPointWaveInfoUI.Close();
                currentOpenedWaypoint = null;
            }

            return;
        }

        if (currentOpenedWaypoint == nearestWaypoint)
        {
            return;
        }

        currentOpenedWaypoint = nearestWaypoint;

        spawnPointWaveInfoUI.Open(
            nearestWaypoint.TargetPosition,
            nearestWaypoint.WaveSpawnData
        );
    }

    private void UpdateAllWaypointIcons(RuntimeWaypoint nearestWaypoint)
    {
        for (int i = 0; i < runtimeWaypoints.Count; i++)
        {
            RuntimeWaypoint runtimeWaypoint = runtimeWaypoints[i];

            if (runtimeWaypoint == null || runtimeWaypoint.WaypointRect == null)
            {
                continue;
            }

            if (runtimeWaypoint.TargetPosition == null)
            {
                runtimeWaypoint.WaypointRect.gameObject.SetActive(false);
                continue;
            }

            bool isNearestInfoTarget = runtimeWaypoint == nearestWaypoint;

            if (isNearestInfoTarget)
            {
                runtimeWaypoint.WaypointRect.gameObject.SetActive(false);
                continue;
            }

            runtimeWaypoint.WaypointRect.gameObject.SetActive(true);
            UpdateWaypointPosition(runtimeWaypoint);
        }
    }

    private void UpdateWaypointPosition(RuntimeWaypoint runtimeWaypoint)
    {
        Vector3 screenPosition = targetCamera.WorldToScreenPoint(
            runtimeWaypoint.TargetPosition.position
        );

        Vector2 finalScreenPosition = GetWaypointScreenPosition(
            screenPosition,
            runtimeWaypoint.WaypointRect
        );

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            finalScreenPosition,
            null,
            out Vector2 canvasPosition
        );

        runtimeWaypoint.WaypointRect.anchoredPosition = canvasPosition;

        if (rotateToTargetDirection)
        {
            RotateWaypoint(runtimeWaypoint.WaypointRect, finalScreenPosition);
        }
    }

    private Vector2 GetWaypointScreenPosition(Vector3 screenPosition, RectTransform waypointRect)
    {
        bool isBehindCamera = screenPosition.z < 0f;

        Vector2 targetScreenPosition = new Vector2(
            screenPosition.x,
            screenPosition.y
        );

        if (isBehindCamera)
        {
            targetScreenPosition.x = Screen.width - targetScreenPosition.x;
            targetScreenPosition.y = Screen.height - targetScreenPosition.y;
        }

        bool isOnScreen =
            !isBehindCamera &&
            targetScreenPosition.x >= screenMargin &&
            targetScreenPosition.x <= Screen.width - screenMargin &&
            targetScreenPosition.y >= screenMargin &&
            targetScreenPosition.y <= Screen.height - screenMargin;

        if (isOnScreen)
        {
            return ClampScreenPosition(targetScreenPosition, waypointRect);
        }

        return GetVirtualEdgeScreenPosition(
            targetScreenPosition,
            waypointRect,
            isBehindCamera
        );
    }

    private Vector2 GetVirtualEdgeScreenPosition(
        Vector2 targetScreenPosition,
        RectTransform waypointRect,
        bool isBehindCamera
    )
    {
        Vector2 screenCenter = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        Vector2 direction = targetScreenPosition - screenCenter;

        if (direction.sqrMagnitude <= behindCenterThreshold * behindCenterThreshold)
        {
            direction = isBehindCamera ? Vector2.down : Vector2.up;
        }

        direction.Normalize();

        Vector2 safeVirtualHalfSize = GetSafeVirtualHalfSize(waypointRect);

        float distanceToVerticalEdge = Mathf.Abs(direction.x) > 0.001f
            ? safeVirtualHalfSize.x / Mathf.Abs(direction.x)
            : float.MaxValue;

        float distanceToHorizontalEdge = Mathf.Abs(direction.y) > 0.001f
            ? safeVirtualHalfSize.y / Mathf.Abs(direction.y)
            : float.MaxValue;

        float edgeDistance = Mathf.Min(
            distanceToVerticalEdge,
            distanceToHorizontalEdge
        );

        Vector2 edgePosition = screenCenter + direction * edgeDistance;

        return ClampScreenPosition(edgePosition, waypointRect);
    }

    private Vector2 GetSafeVirtualHalfSize(RectTransform waypointRect)
    {
        float halfWidth = 0f;
        float halfHeight = 0f;

        if (waypointRect != null)
        {
            halfWidth = waypointRect.rect.width * 0.5f;
            halfHeight = waypointRect.rect.height * 0.5f;
        }

        float safeHalfScreenWidth = Screen.width * 0.5f - screenMargin - halfWidth;
        float safeHalfScreenHeight = Screen.height * 0.5f - screenMargin - halfHeight;

        float safeX = Mathf.Min(virtualEdgeDistanceFromCenter, safeHalfScreenWidth);
        float safeY = Mathf.Min(virtualEdgeDistanceFromCenter, safeHalfScreenHeight);

        safeX = Mathf.Max(0f, safeX);
        safeY = Mathf.Max(0f, safeY);

        return new Vector2(safeX, safeY);
    }

    private Vector2 ClampScreenPosition(Vector2 screenPosition, RectTransform waypointRect)
    {
        float halfWidth = 0f;
        float halfHeight = 0f;

        if (waypointRect != null)
        {
            halfWidth = waypointRect.rect.width * 0.5f;
            halfHeight = waypointRect.rect.height * 0.5f;
        }

        float minX = screenMargin + halfWidth;
        float maxX = Screen.width - screenMargin - halfWidth;

        float minY = screenMargin + halfHeight;
        float maxY = Screen.height - screenMargin - halfHeight;

        float clampedX = Mathf.Clamp(screenPosition.x, minX, maxX);
        float clampedY = Mathf.Clamp(screenPosition.y, minY, maxY);

        return new Vector2(clampedX, clampedY);
    }

    private void RotateWaypoint(RectTransform waypointRect, Vector2 waypointScreenPosition)
    {
        Vector2 screenCenter = new Vector2(
            Screen.width * 0.5f,
            Screen.height * 0.5f
        );

        Vector2 direction = waypointScreenPosition - screenCenter;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        waypointRect.rotation = Quaternion.Euler(0f, 0f, angle + iconAngleOffset);
    }
}