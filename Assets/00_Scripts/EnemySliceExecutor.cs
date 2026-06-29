using System.Threading.Tasks;
using BzKovSoft.CharacterSlicer;
using BzKovSoft.ObjectSlicer;
using UnityEngine;

public class EnemySliceExecutor : MonoBehaviour
{
    [Header("Slice Target")]
    public BzSliceableCharacter sliceableCharacter;

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

    private bool sliced;

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

        Vector3 slicePoint = transform.position + Vector3.up * sliceHeightOffset;
        Vector3 planeNormal = swordTransform.up;

        return await TrySlice(slicePoint, planeNormal);
    }

    private async Task<bool> TrySlice(Vector3 slicePoint, Vector3 planeNormal)
    {
        if (sliced)
        {
            return false;
        }

        if (sliceableCharacter == null)
        {
            sliceableCharacter = GetComponentInChildren<BzSliceableCharacter>();
        }

        if (sliceableCharacter == null)
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
        BzSliceTryResult result = await sliceableCharacter.SliceAsync(plane, null);

        if (result == null || !result.sliced)
        {
            sliced = false;
            return false;
        }

        SpawnSliceParticle(slicePoint);
        DisableAnimatorsOnSlicedObjects(result);

        if (enableGravityAfterSlice)
        {
            EnableGravityWithSingleRootRigidbody(result);
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

    private void DisableAnimatorsOnSlicedObjects(BzSliceTryResult result)
    {
        if (result == null || result.resultObjects == null)
        {
            return;
        }

        for (int i = 0; i < result.resultObjects.Length; i++)
        {
            if (result.resultObjects[i] == null)
            {
                continue;
            }

            GameObject slicedObject = result.resultObjects[i].gameObject;

            if (slicedObject == null)
            {
                continue;
            }

            Animator[] animators = slicedObject.GetComponentsInChildren<Animator>(true);

            for (int j = 0; j < animators.Length; j++)
            {
                if (animators[j] == null)
                {
                    continue;
                }

                animators[j].enabled = false;
            }
        }
    }

    private void EnableGravityWithSingleRootRigidbody(BzSliceTryResult result)
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

            GameObject slicedObject = result.resultObjects[i].gameObject;

            if (slicedObject == null)
            {
                continue;
            }

            RemoveChildRigidbodies(slicedObject);
            EnableColliders(slicedObject);

            Rigidbody rigidbody = EnableRootRigidbody(slicedObject);

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

    private void EnableColliders(GameObject rootObject)
    {
        Collider[] colliders = rootObject.GetComponentsInChildren<Collider>(true);

        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];

            if (collider == null)
            {
                continue;
            }

            collider.enabled = true;
            collider.isTrigger = false;
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

        rigidbody.isKinematic = false;

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
            direction = Vector3.forward;
        }

        return direction.normalized;
    }

    public void ResetSliceState()
    {
        sliced = false;
    }
}