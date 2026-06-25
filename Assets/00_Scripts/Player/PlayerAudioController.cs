using AYellowpaper.SerializedCollections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudioController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private PlayerMovement movement;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource footstepSource;
    [SerializeField] private AudioSource actionSource;

    [Header("Surface Check")]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private SurfaceType defaultSurface = SurfaceType.Concrete;
    [SerializeField] private float surfaceCheckDistance = 2f;
    [SerializeField] private float surfaceRayStartHeight = 0.2f;

    [Header("Surface Sounds")]
    [SerializedDictionary("Surface", "Clips")]
    [SerializeField] private SerializedDictionary<SurfaceType, AudioClip[]> footstepSounds;

    [SerializedDictionary("Surface", "Clips")]
    [SerializeField] private SerializedDictionary<SurfaceType, AudioClip[]> landSounds;

    [SerializedDictionary("Surface", "Clips")]
    [SerializeField] private SerializedDictionary<SurfaceType, AudioClip[]> hardLandSounds;

    [Header("Common Action Sounds")]
    [SerializeField] private AudioClip[] jumpSounds;
    [SerializeField] private AudioClip[] wallJumpSounds;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip[] hitSounds;

    [Header("Wall Move Sounds")]
    [SerializeField] private AudioClip[] wallStepSounds;

    [Header("Footstep Setting")]
    [SerializeField] private float stepDistance = 2f;
    [SerializeField] private float minMoveSpeed = 0.1f;

    [Header("Wall Step Setting")]
    [SerializeField] private float wallStepDistance = 1.5f;
    [SerializeField] private float minWallMoveSpeed = 0.1f;

    private bool wasFootstepActive;
    private bool wasWallStepActive;

    private bool canFootstep;
    private bool canWallStep;

    private float movedDistance;
    private float wallMovedDistance;

    private int lastFootstepIndex = -1;
    private int lastJumpIndex = -1;
    private int lastWallJumpIndex = -1;
    private int lastAttackIndex = -1;
    private int lastHitIndex = -1;
    private int lastLandIndex = -1;
    private int lastHardLandIndex = -1;
    private int lastWallStepIndex = -1;

    private void Awake()
    {
        if (!player)
        {
            player = GetComponent<PlayerController>();
        }

        if (!movement)
        {
            movement = GetComponent<PlayerMovement>();
        }

        if (!footstepSource)
        {
            footstepSource = GetComponent<AudioSource>();
        }

        if (!actionSource)
        {
            actionSource = footstepSource;
        }
    }

    private void OnEnable()
    {
        if (player)
        {
            player.OnWalk += EnableFootstep;
            player.OnWalk += DisableWallStep;

            player.OnAir += DisableFootstep;
            player.OnAir += DisableWallStep;

            player.OnRightWall += EnableWallStep;
            player.OnLeftWall += EnableWallStep;

            player.OnDashing += DisableFootstep;
            player.OnDashing += DisableWallStep;

            player.OnGroundJump += PlayJumpSound;
            player.OnWallJump += PlayWallJumpSound;

            player.OnAttack += PlayAttackSound;
            player.OnHit += PlayHitSound;
        }

        if (movement)
        {
            movement.OnStepped += PlayLandSound;
            movement.OnBigStepped += PlayHardLandSound;
        }
    }

    private void OnDisable()
    {
        if (player)
        {
            player.OnWalk -= EnableFootstep;
            player.OnWalk -= DisableWallStep;

            player.OnAir -= DisableFootstep;
            player.OnAir -= DisableWallStep;

            player.OnRightWall -= EnableWallStep;
            player.OnLeftWall -= EnableWallStep;

            player.OnDashing -= DisableFootstep;
            player.OnDashing -= DisableWallStep;

            player.OnGroundJump -= PlayJumpSound;
            player.OnWallJump -= PlayWallJumpSound;

            player.OnAttack -= PlayAttackSound;
            player.OnHit -= PlayHitSound;
        }

        if (movement)
        {
            movement.OnStepped -= PlayLandSound;
            movement.OnBigStepped -= PlayHardLandSound;
        }
    }

    private void Update()
    {
        Debug.Log(player.cc.velocity);
        UpdateFootstep();
        UpdateWallStep();
    }

    private void EnableFootstep()
    {
        canFootstep = true;
    }

    private void DisableFootstep()
    {
        canFootstep = false;
        movedDistance = 0f;
        wasFootstepActive = false;
    }

    private void EnableWallStep()
    {
        canWallStep = true;

        canFootstep = false;
        movedDistance = 0f;
    }

    private void DisableWallStep()
    {
        canWallStep = false;
        wallMovedDistance = 0f;
        wasWallStepActive = false;
    }

    private void UpdateFootstep()
    {
        bool canPlay = CanPlayFootstep();

        if (!canPlay)
        {
            wasFootstepActive = false;
            movedDistance = 0f;
            return;
        }

        // 걷기 시작 순간: 즉시 1회 재생
        if (!wasFootstepActive)
        {
            wasFootstepActive = true;
            movedDistance = 0f;

            PlayFootstepSound();
            return;
        }

        Vector3 horizontalVelocity = player.cc.velocity;
        horizontalVelocity.y = 0f;

        movedDistance += horizontalVelocity.magnitude * Time.deltaTime;

        if (movedDistance >= stepDistance)
        {
            movedDistance = 0f;
            PlayFootstepSound();
        }
    }

    private void UpdateWallStep()
    {
        bool canPlay = CanPlayWallStep();

        if (!canPlay)
        {
            wasWallStepActive = false;
            wallMovedDistance = 0f;
            return;
        }

        // 벽타기 시작 순간: 즉시 1회 재생
        if (!wasWallStepActive)
        {
            wasWallStepActive = true;
            wallMovedDistance = 0f;

            PlayWallStepSound();
            return;
        }

        Vector3 velocity = player.cc.velocity;

        // 벽타기가 수평 이동 위주면 아래 두 줄 유지
        velocity.y = 0f;

        wallMovedDistance += velocity.magnitude * Time.deltaTime;

        if (wallMovedDistance >= wallStepDistance)
        {
            wallMovedDistance = 0f;
            PlayWallStepSound();
        }
    }

    private bool CanPlayFootstep()
    {
        if (!canFootstep)
        {
            return false;
        }

        if (!player)
        {
            return false;
        }

        if (player.IsDead)
        {
            return false;
        }

        if (!player.cc.isGrounded)
        {
            return false;
        }

        if (player.InputHandler.Move.sqrMagnitude <= 0.01f)
        {
            return false;
        }

        Vector3 horizontalVelocity = player.cc.velocity;
        horizontalVelocity.y = 0f;

        return horizontalVelocity.sqrMagnitude > minMoveSpeed * minMoveSpeed;
    }

    private bool CanPlayWallStep()
    {
        if (!canWallStep)
        {
            return false;
        }

        if (!player)
        {
            return false;
        }

        if (player.IsDead)
        {
            return false;
        }

        if (player.CurrentState != PlayerState.Wall)
        {
            return false;
        }

        if (!player.Walling)
        {
            return false;
        }

        Vector3 horizontalVelocity = player.cc.velocity;
        horizontalVelocity.y = 0f;

        return horizontalVelocity.sqrMagnitude > minWallMoveSpeed * minWallMoveSpeed;
    }

    private void PlayFootstepSound()
    {
        SurfaceType surfaceType = GetCurrentSurfaceType();

        AudioClip[] clips = GetClipsFromDictionary(footstepSounds, surfaceType);

        PlayRandom(clips, footstepSource, ref lastFootstepIndex);
    }

    private void PlayWallStepSound()
    {
        PlayRandom(wallStepSounds, footstepSource, ref lastWallStepIndex);
    }

    private void PlayJumpSound()
    {
        PlayRandom(jumpSounds, actionSource, ref lastJumpIndex);
    }

    private void PlayWallJumpSound()
    {
        PlayRandom(wallJumpSounds, actionSource, ref lastWallJumpIndex);
    }

    private void PlayLandSound()
    {
        SurfaceType surfaceType = GetCurrentSurfaceType();

        AudioClip[] clips = GetClipsFromDictionary(landSounds, surfaceType);

        PlayRandom(clips, actionSource, ref lastLandIndex);

        movedDistance = 0f;
    }

    private void PlayAttackSound()
    {
        PlayRandom(attackSounds, actionSource, ref lastAttackIndex);
    }

    private void PlayHitSound()
    {
        PlayRandom(hitSounds, actionSource, ref lastHitIndex);
    }

    private void PlayHardLandSound()
    {
        SurfaceType surfaceType = GetCurrentSurfaceType();

        AudioClip[] clips = GetClipsFromDictionary(hardLandSounds, surfaceType);

        PlayRandom(clips, actionSource, ref lastHardLandIndex);

        movedDistance = 0f;
    }

    private SurfaceType GetCurrentSurfaceType()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * surfaceRayStartHeight;

        if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, surfaceCheckDistance, groundMask))
        {
            if (hit.collider.TryGetComponent(out FootstepSurface surface))
            {
                return surface.SurfaceType;
            }

            FootstepSurface parentSurface = hit.collider.GetComponentInParent<FootstepSurface>();

            if (parentSurface)
            {
                return parentSurface.SurfaceType;
            }
        }

        return defaultSurface;
    }

    private AudioClip[] GetClipsFromDictionary(
        SerializedDictionary<SurfaceType, AudioClip[]> dictionary,
        SurfaceType surfaceType)
    {
        if (dictionary == null)
        {
            return null;
        }

        if (dictionary.TryGetValue(surfaceType, out AudioClip[] clips))
        {
            return clips;
        }

        if (dictionary.TryGetValue(defaultSurface, out AudioClip[] defaultClips))
        {
            return defaultClips;
        }

        return null;
    }

    private void PlayRandom(AudioClip[] clips, AudioSource source, ref int lastIndex)
    {
        if (!source)
        {
            return;
        }

        if (clips == null || clips.Length == 0)
        {
            return;
        }

        int index;

        if (clips.Length == 1)
        {
            index = 0;
        }
        else
        {
            do
            {
                index = Random.Range(0, clips.Length);
            }
            while (index == lastIndex);
        }

        lastIndex = index;

        source.PlayOneShot(clips[index]);
    }
}