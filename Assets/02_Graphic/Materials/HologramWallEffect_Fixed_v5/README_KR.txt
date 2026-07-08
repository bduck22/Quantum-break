HologramWallEffect_Fixed_v5
===========================

v4가 화면에서 거의 흰색 판처럼 보이는 문제를 줄인 버전입니다.
원인은 v4의 기본값이 너무 강했기 때문입니다.
- Base Alpha가 높음
- Emission Strength가 높음
- Bloom/Color Grading이 있으면 흰색으로 날아가기 쉬움

v5 기본 방향
------------
- 벽 면은 낮은 알파의 반투명 청록색
- 가로 라인은 듬성듬성
- 외곽선과 Dissolve 가장자리만 약하게 발광
- 색 출력값을 셰이더 안에서 clamp해서 흰색으로 날아가는 현상을 줄임

적용 전 삭제 권장
----------------
기존 버전이 남아 있으면 이전 shader/material이 계속 적용될 수 있습니다.
아래 폴더가 있으면 삭제한 뒤 v5를 넣는 것을 추천합니다.

Assets/HologramWallEffect
Assets/HologramWallEffect_BrightSparse
Assets/HologramWallEffect_Fixed_v3
Assets/HologramWallEffect_Fixed_v4

사용 순서
---------
1. 압축을 풀고 Assets 폴더를 Unity 프로젝트에 복사합니다.
2. Unity가 임포트와 컴파일을 끝낼 때까지 기다립니다.
3. 상단 메뉴 실행:
   Tools > Hologram Wall V5 > Fix And Create Material
4. 벽 오브젝트를 선택한 뒤 실행:
   Tools > Hologram Wall V5 > Apply Material To Selected
5. 테스트용 벽을 만들려면 실행:
   Tools > Hologram Wall V5 > Create Test Wall

스크립트 호출 예시
-----------------
public class TestWallTrigger : MonoBehaviour
{
    [SerializeField] private HologramWallDisappearV5 wall;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            wall.PlayDisappear();
        }
    }
}

너무 밝을 때 조절할 값
--------------------
Material에서 아래 값을 낮추면 됩니다.

1. _OverallBrightness
   기본값 0.72
   더 어둡게: 0.45 ~ 0.60

2. _FaceAlpha
   기본값 0.24
   더 투명하게: 0.12 ~ 0.18

3. _MaxOutputBrightness
   기본값 1.08
   Bloom 때문에 하얗게 날아가면: 0.75 ~ 0.95

아직 홀로그램 느낌이 약할 때
--------------------------
면이 너무 약하면 _FaceAlpha를 올리지 말고 _LineBrightness 또는 _RimBrightness를 조금 올리는 쪽이 낫습니다.
추천 범위:
- _LineBrightness: 0.75 -> 1.0
- _RimBrightness: 0.85 -> 1.1

라인이 촘촘할 때
---------------
_LineSpacing을 키우면 라인 간격이 넓어집니다.
추천 범위:
- 0.82 -> 1.1 또는 1.3

소리/파티클
----------
HologramWallDisappearV5 컴포넌트에 ParticleSystem, AudioSource, AudioClip을 선택적으로 넣을 수 있습니다.
없어도 shader dissolve는 정상 동작합니다.
