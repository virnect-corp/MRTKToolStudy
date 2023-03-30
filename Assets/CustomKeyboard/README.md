[위로](../../README.md)
###  사용방법
1. Canvas안쪽의 TMP_InputField 컴포넌트에 SystemKeyboardCustom 추가한다.
2. TMP_InputField 컴포넌트에 위치한 Recttransofrm Pivot.x 의 위치를 변경하여 커서 위치를 조절한다.
3. 
※ 현재는 UWP환경에서 
Canvas 안의 TMP_InputField만 테스트 해봤습니다. 다른 Text컴포넌트는 요청 및 필요시 추가할 생각 입니다.

### 스크립트 플로우 요약

```plantuml
    :키보드 오픈;
    card Update{
        if(키보드 입력) then (true)
        :text Component Update;
        :커서 위치 업데이트;
        endif
        :키보드 상태 체크;
    }
```
### 속성 및 함수 명세

|Attribute|설명
|-|-
|OpenTextInit    | 터치시에 텍스트 초기화 할지 체크하기.
|wmrKeyboard     | mrtk에서 제공해주는 키보드 스크립트.
|caretPrefabs    | 커서 Prefabs
|blinkCaretAnim  | 커서 애님을 관리하려고 만듬
|inputFieldHeight| inputFieldHeight 반복되는 계산을 줄이고자 빼놓음.
|prevText        | 키보드가 닫힐 때 초기화 되는 것을 방지하는 변수

|Property|설명
|-|-
|InputField| 컴포넌트 널체크 및 반환
|Caret     | 커서 게임오브젝트 널 체크 및 반환
|Text      | 텍스트 체크 후 텍스트 컴포넌트, 커서위치 업데이트
|CaretIndex| 커서 위치 확인후 적용

|Function|설명
|-|-
|Awake             | blinkCaretAnim 초기화
|Start             | 컴포넌트, 이벤트 초기화 함수 호출
|Update            | 키보드 상태체크, 키보드 컴포넌트에 입력
|ComponentInit     | 컴포넌트 초기화
|EventInit         | 이벤트 초기화
|OpenSystemKeyboard| 키보드 열기.
|UpdateCaret       | 커서 위치 조정함수.
|BlinkCaret        | 깜박이는 애니메이션

### 개선 예정
1. 키보드를 용도에 맞게 분류할 예정.
|용도|요약
|-|-
|2D|Canvas Screen -> Overlay    : UI Local 포인트 그대로 활용.
|3D|Canvas Screen -> WorldSpace : UI Local 공간변환
|3DMesh|

2. keyInputEvent 게산 수정예정.
    현재 코루틴에서 글자변경 + CusorIndex까지 같이 돌고 있음.
    두 기능을 분리하고 변경시에 계산하도록 수정 할 에정.



### 참조문서
[Mixedreality키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/develop/unity/keyboard-input-in-unity)
[MRTK2-기능-UX구성-시스템키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/mrtk-unity/mrtk2/features/ux-building-blocks/system-keyboard?view=mrtkunity-2022-05)
[MRTK2-기능-실험적-MixedReality키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/mrtk-unity/mrtk2/features/experimental/mixed-reality-keyboard?view=mrtkunity-2022-05)
[MixedRealityKeyboard](https://learn.microsoft.com/ko-kr/dotnet/api/microsoft.mixedreality.toolkit.experimental.ui.mixedrealitykeyboard?preserve-view=true&view=mixed-reality-toolkit-unity-2020-dotnet-2.8.0)
[UnityTouchKeyBoard](https://docs.unity3d.com/ScriptReference/TouchScreenKeyboard.Open.html)
[InputPane](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement?view=winrt-22621)