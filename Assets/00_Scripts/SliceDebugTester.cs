using BzKovSoft.CharacterSlicer;
using BzKovSoft.ObjectSlicer;
using UnityEngine;
using UnityEngine.InputSystem;

public class SliceDebugTester : MonoBehaviour
{
    [SerializeField] private BzSliceableCharacter target;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float hitPointHeightOffset = 1.2f;

    private async void Update()
    {
        if (!Keyboard.current.f1Key.wasPressedThisFrame)
        {
            return;
        }

        if (target == null)
        {
            Debug.LogWarning("Target이 없습니다. Robot2의 BzSliceableCharacter를 넣어주세요.");
            return;
        }

        playerCamera = Camera.main;
        if (playerCamera == null)
        {
            Debug.LogWarning("Player Camera가 없습니다.");
            return;
        }

        Vector3 hitPoint = target.transform.position + Vector3.up * hitPointHeightOffset;
        Vector3 planeNormal = playerCamera.transform.right;

        Plane plane = new Plane(planeNormal.normalized, hitPoint);

        Debug.Log("F1 슬라이스 테스트 실행");

        BzSliceTryResult result = await target.SliceAsync(plane, null);

        if (result == null)
        {
            Debug.LogWarning("슬라이스 결과가 null입니다.");
            return;
        }

        if (!result.sliced)
        {
            Debug.LogWarning("슬라이스 실패");
            return;
        }

        Debug.Log("슬라이스 성공");
    }
}