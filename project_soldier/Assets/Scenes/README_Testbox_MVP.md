# Testbox_MVP 씬 생성 안내

이 파일은 `Milestone 1`까지 테스트 가능한 씬 생성 안내서입니다.

## 생성 절차
1. Unity 프로젝트를 연다.
2. 메뉴에서 `Project Soldier/Build Milestone0 Test Scene`를 실행한다.
3. 자동으로 `Assets/Scenes/Testbox_MVP.unity`가 생성된다.
4. Play를 눌러 조작/사격 반응을 테스트한다.

## 자동 생성되는 요소
- Ground
- 외곽 Wall 4개
- CoverBox 10개
- TargetBox 8개(`Tag: Target`, 피격 반응 스크립트 포함)
- Player + CharacterController + Main Camera + `PlayerM1Controller`
- UI_Root(Canvas) + EventSystem + 조작 UI

## 테스트 조작(에디터)
- 이동: `W/A/S/D` 또는 좌하단 Joystick
- 시점: 마우스 이동 또는 우측 LookArea 드래그
- 사격: 마우스 좌클릭 홀드 또는 FireButton 홀드
- 확인: `Target` 박스 피격 시 빨간색으로 잠깐 변환

## 참고 문서
- `문서/06_M0_테스트씬_구성안.md`
- 생성 스크립트: `Assets/Scripts/Editor/Milestone0SceneBuilder.cs`
