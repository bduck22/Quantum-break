using System.Collections.Generic;
using System.Threading.Tasks;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class EnemySliceExecutor : MonoBehaviour
{
    [Header("Animated Model")]
    public GameObject animatedModelRoot;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    [Header("Slice Proxy")]
    public BzSliceableObject sliceableObject;
    public MeshFilter sliceMeshFilter;
    public MeshRenderer sliceMeshRenderer;

    [Header("Collider Template")]
    public CapsuleCollider capsuleColliderTemplate;

    [Header("Fixed Slice Point")]
    [SerializeField] private float sliceHeightOffset = 1.8f;

    [Header("Gravity")]
    [SerializeField] private bool enableGravityAfterSlice = true;
    [SerializeField] private float slicedMass = 1f;
    [SerializeField] private float linearDamping = 0.5f;
    [SerializeField] private float angularDamping = 1.5f;

    [Header("Horizontal Velocity")]
    [SerializeField] private bool addRandomHorizontalVelocity = true;
    [SerializeField] private float randomHorizontalSpeed = 2f;
    [SerializeField] private bool oppositeDirectionForPieces = true;

    [Header("Created Collider Offset")]
    [SerializeField] private float createdColliderCenterZOffset = 1f;

    [Header("Mesh Channel Strip")]
    [SerializeField] private bool stripUnusedMeshChannels = true;
    [SerializeField] private bool stripTangents = true;
    [SerializeField] private bool stripVertexColors = true;
    [SerializeField] private bool stripSecondaryUVs = true;
    [SerializeField] private bool stripMainUV = false;
    [SerializeField] private bool stripBoneData = true;

    private static readonly List<Vector2> EmptyVector2List = new List<Vector2>(0);
    private static readonly Vector4[] EmptyVector4Array = new Vector4[0];
    private static readonly Color[] EmptyColorArray = new Color[0];
    private static readonly Color32[] EmptyColor32Array = new Color32[0];
    private static readonly BoneWeight[] EmptyBoneWeightArray = new BoneWeight[0];
    private static readonly Matrix4x4[] EmptyMatrixArray = new Matrix4x4[0];

    private Mesh bakedMesh;
    private bool sliced;

    private Vector3 savedColliderCenter;
    private float savedColliderRadius;
    private float savedColliderHeight;
    private int savedColliderDirection;

    public bool IsSliced => sliced;

    public async Task<bool> TrySliceBySwordYAtFixedPoint(Transform swordTransform)
    {
        if (sliced)
        {
            return false;
        }

        if (swordTransform == null)
        {
            return false;
        }

        if (!CacheReferences())
        {
            return false;
        }

        SaveColliderTemplate();
        BakeCurrentSkinnedMeshToProxy();
        SetModelState(false, true);

        Vector3 slicePoint = transform.position + Vector3.up * sliceHeightOffset;
        Vector3 planeNormal = swordTransform.up;

        bool success = await TrySlice(slicePoint, planeNormal);

        if (!success)
        {
            SetModelState(true, false);
        }

        return success;
    }

    private bool CacheReferences()
    {
        if (skinnedMeshRenderer == null)
        {
            skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>(true);
        }

        if (sliceableObject == null)
        {
            sliceableObject = GetComponentInChildren<BzSliceableObject>(true);
        }

        if (sliceableObject == null || skinnedMeshRenderer == null)
        {
            return false;
        }

        GameObject sliceObject = sliceableObject.gameObject;

        if (sliceMeshFilter == null)
        {
            sliceMeshFilter = sliceObject.GetComponent<MeshFilter>();
        }

        if (sliceMeshRenderer == null)
        {
            sliceMeshRenderer = sliceObject.GetComponent<MeshRenderer>();
        }

        if (capsuleColliderTemplate == null)
        {
            capsuleColliderTemplate = sliceObject.GetComponent<CapsuleCollider>();
        }

        return sliceMeshFilter != null &&
               sliceMeshRenderer != null &&
               capsuleColliderTemplate != null;
    }

    private void SaveColliderTemplate()
    {
        savedColliderCenter = capsuleColliderTemplate.center;
        savedColliderRadius = capsuleColliderTemplate.radius;
        savedColliderHeight = capsuleColliderTemplate.height;
        savedColliderDirection = capsuleColliderTemplate.direction;
    }

    private void BakeCurrentSkinnedMeshToProxy()
    {
        if (bakedMesh == null)
        {
            bakedMesh = new Mesh();
            bakedMesh.name = $"{name}_BakedSliceMesh";
        }
        else
        {
            bakedMesh.Clear();
        }

        MatchWorldTransform(sliceableObject.transform, skinnedMeshRenderer.transform);

        skinnedMeshRenderer.BakeMesh(bakedMesh, true);

        if (stripUnusedMeshChannels)
        {
            StripUnusedMeshChannels(bakedMesh);
        }

        bakedMesh.RecalculateBounds();

        sliceMeshFilter.sharedMesh = bakedMesh;
        sliceMeshRenderer.sharedMaterials = skinnedMeshRenderer.sharedMaterials;
    }

    private void StripUnusedMeshChannels(Mesh mesh)
    {
        if (mesh == null)
        {
            return;
        }

        if (stripTangents)
        {
            mesh.tangents = EmptyVector4Array;
        }

        if (stripVertexColors)
        {
            mesh.colors = EmptyColorArray;
            mesh.colors32 = EmptyColor32Array;
        }

        if (stripSecondaryUVs)
        {
            mesh.SetUVs(1, EmptyVector2List);
            mesh.SetUVs(2, EmptyVector2List);
            mesh.SetUVs(3, EmptyVector2List);
            mesh.SetUVs(4, EmptyVector2List);
            mesh.SetUVs(5, EmptyVector2List);
            mesh.SetUVs(6, EmptyVector2List);
            mesh.SetUVs(7, EmptyVector2List);
        }

        if (stripMainUV)
        {
            mesh.SetUVs(0, EmptyVector2List);
        }

        if (stripBoneData)
        {
            mesh.boneWeights = EmptyBoneWeightArray;
            mesh.bindposes = EmptyMatrixArray;
        }
    }

    private void MatchWorldTransform(Transform target, Transform source)
    {
        target.position = source.position;
        target.rotation = source.rotation;

        Vector3 sourceScale = source.lossyScale;
        Vector3 parentScale = Vector3.one;

        if (target.parent != null)
        {
            parentScale = target.parent.lossyScale;
        }

        target.localScale = new Vector3(
            SafeDivide(sourceScale.x, parentScale.x),
            SafeDivide(sourceScale.y, parentScale.y),
            SafeDivide(sourceScale.z, parentScale.z)
        );
    }

    private float SafeDivide(float value, float divisor)
    {
        if (Mathf.Abs(divisor) < 0.0001f)
        {
            return value;
        }

        return value / divisor;
    }

    private void SetModelState(bool animatedModelActive, bool sliceProxyActive)
    {
        if (animatedModelRoot != null)
        {
            animatedModelRoot.SetActive(animatedModelActive);
        }

        if (sliceableObject != null)
        {
            sliceableObject.gameObject.SetActive(sliceProxyActive);
        }
    }

    private async Task<bool> TrySlice(Vector3 slicePoint, Vector3 planeNormal)
    {
        if (sliced)
        {
            return false;
        }

        if (sliceableObject == null)
        {
            return false;
        }

        if (planeNormal.sqrMagnitude < 0.0001f)
        {
            return false;
        }

        planeNormal.Normalize();
        sliced = true;

        Plane plane = new Plane(planeNormal, slicePoint);
        BzSliceTryResult result = await sliceableObject.SliceAsync(plane, null);

        if (result == null || !result.sliced)
        {
            sliced = false;
            return false;
        }

        SpawnSliceParticle(slicePoint);

        await Task.Yield();

        if (enableGravityAfterSlice)
        {
            EnableSlicedObjectsPhysics(result);
        }

        return true;
    }

    private void SpawnSliceParticle(Vector3 slicePoint)
    {
        if (SpawnManagers.Instance == null || SpawnManagers.Instance.Particle == null)
        {
            return;
        }

        ParticleController particle = SpawnManagers.Instance.Particle.SpawnParticle(
            Particle_Type.BulletParring,
            slicePoint,
            Quaternion.identity
        );

        if (particle != null)
        {
            particle.Play();
        }
    }

    private void EnableSlicedObjectsPhysics(BzSliceTryResult result)
    {
        if (result == null || result.resultObjects == null)
        {
            return;
        }

        Vector3 randomHorizontalDirection = GetRandomHorizontalDirection();

        for (int i = 0; i < result.resultObjects.Length; i++)
        {
            if (result.resultObjects[i] == null)
            {
                continue;
            }

            GameObject slicedRootObject = result.resultObjects[i].gameObject;

            if (slicedRootObject == null)
            {
                continue;
            }

            GameObject colliderTargetObject = GetColliderTargetObject(slicedRootObject);

            RemoveChildRigidbodies(slicedRootObject);
            EnsureCapsuleCollider(colliderTargetObject);

            Rigidbody rigidbody = EnableRootRigidbody(slicedRootObject);

            if (!addRandomHorizontalVelocity)
            {
                continue;
            }

            Vector3 moveDirection = randomHorizontalDirection;

            if (oppositeDirectionForPieces && i % 2 == 1)
            {
                moveDirection = -moveDirection;
            }

            ApplyHorizontalVelocity(rigidbody, moveDirection);
        }
    }

    private GameObject GetColliderTargetObject(GameObject rootObject)
    {
        if (rootObject == null)
        {
            return null;
        }

        MeshFilter rootMeshFilter = rootObject.GetComponent<MeshFilter>();

        if (rootMeshFilter != null)
        {
            return rootObject;
        }

        MeshFilter childMeshFilter = rootObject.GetComponentInChildren<MeshFilter>(true);

        if (childMeshFilter != null)
        {
            return childMeshFilter.gameObject;
        }

        return rootObject;
    }

    private void EnsureCapsuleCollider(GameObject targetObject)
    {
        if (targetObject == null)
        {
            return;
        }

        CapsuleCollider capsuleCollider = targetObject.GetComponent<CapsuleCollider>();
        bool createdNewCollider = false;

        if (capsuleCollider == null)
        {
            capsuleCollider = targetObject.AddComponent<CapsuleCollider>();
            createdNewCollider = true;
        }

        Vector3 center = savedColliderCenter;

        if (createdNewCollider)
        {
            center.z += createdColliderCenterZOffset;
        }

        capsuleCollider.center = center;
        capsuleCollider.radius = savedColliderRadius;
        capsuleCollider.height = savedColliderHeight;
        capsuleCollider.direction = savedColliderDirection;
        capsuleCollider.isTrigger = false;
        capsuleCollider.enabled = true;
    }

    private void RemoveChildRigidbodies(GameObject rootObject)
    {
        Rigidbody[] rigidbodies = rootObject.GetComponentsInChildren<Rigidbody>(true);

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            Rigidbody rigidbody = rigidbodies[i];

            if (rigidbody == null)
            {
                continue;
            }

            if (rigidbody.gameObject == rootObject)
            {
                continue;
            }

            Destroy(rigidbody);
        }
    }

    private Rigidbody EnableRootRigidbody(GameObject rootObject)
    {
        Rigidbody rigidbody = rootObject.GetComponent<Rigidbody>();

        if (rigidbody == null)
        {
            rigidbody = rootObject.AddComponent<Rigidbody>();
        }

        rigidbody.isKinematic = false;
        rigidbody.useGravity = true;
        rigidbody.mass = slicedMass;

#if UNITY_6000_0_OR_NEWER
        rigidbody.linearVelocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.linearDamping = linearDamping;
        rigidbody.angularDamping = angularDamping;
#else
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        rigidbody.drag = linearDamping;
        rigidbody.angularDrag = angularDamping;
#endif

        return rigidbody;
    }

    private void ApplyHorizontalVelocity(Rigidbody rigidbody, Vector3 direction)
    {
        if (rigidbody == null)
        {
            return;
        }

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.0001f)
        {
            direction = Vector3.forward;
        }

        direction.Normalize();

        Vector3 horizontalVelocity = direction * randomHorizontalSpeed;

#if UNITY_6000_0_OR_NEWER
        rigidbody.linearVelocity = horizontalVelocity;
#else
        rigidbody.velocity = horizontalVelocity;
#endif
    }

    private Vector3 GetRandomHorizontalDirection()
    {
        Vector3 direction = new Vector3(
            Random.Range(-1f, 1f),
            0f,
            Random.Range(-1f, 1f)
        );

        if (direction.sqrMagnitude < 0.0001f)
        {
            return Vector3.forward;
        }

        return direction.normalized;
    }

    public void ResetSliceState()
    {
        sliced = false;
        SetModelState(true, false);

        if (bakedMesh != null)
        {
            bakedMesh.Clear();
        }
    }

    private void OnDestroy()
    {
        if (bakedMesh == null)
        {
            return;
        }

        Destroy(bakedMesh);
        bakedMesh = null;
    }
}