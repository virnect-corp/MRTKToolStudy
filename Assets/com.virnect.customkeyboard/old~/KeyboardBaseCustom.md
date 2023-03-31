
[메인으로](../../StudyHololens2.md)

###  사용방법
1. TextMeshLabel Or Text에 keyBoardInputField 부착하기
2. Interactable 컴포넌트의 OnClick이벤트에 keyBoardInputField.OpenSystemKeyboard()연결
3. Interactable과 연관된 이벤트 실행 후 테스트!

//UNITY_2019_3_OR_NEWER 에서만 동작한다고 함.



### 다른 방법으로는 아래와 같이 사용 가능.
TouchScreenKeyboard.Open("");
keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);

ShowKeyboard(string text = "", bool multiLine = false)HideKeyboard() 하여 키보드를 표시 및 숨기고, 키보드가 OnShowKeyboard표시되고OnCommitText, 숨겨지고, Enter 키를 누를 때 처리할 , OnHideKeyboard 및 이벤트를 처리합니다.

### 테스트단계
1. DebugLog체크
2. 키보드 On
3. 키보드 Off
4. 입력확인
5. Caret생성확인.
6. Caret생성위치확인
7. Caret애님확인.
8. Caret글자에 위치 반영확인




### 예외상황
손을 내리면 키보드 꺼져버림.


### SystemFlow
```plantuml
    card UpdateText{
        :키보드 CaretIdx확인;
        
    }
    split
        :키보드 오픈시;
        :커서애니메이션 켜기;
    split again
        :키보드 닫을 때;
        :커서애니메이션 끄기;
    end split

```



|함수|설명
|-|-
|InitKeyboard |키보드 초기화
|ShowKeyboard |키보드 보이기
|HideKeyboard |키보드 숨기기



### SystemFlowOld
```plantuml
    card Start{
        if(테스트 모드면)
            :테스트 함수 실행후 리턴;
            end
        endif
        :키보드 초기화하기
        텍스트 컴포넌트 초기화
        Caret 초기화;
    }
    
    card OpenSystemKeyboard{
    
    if (홀로렌즈 키보드를 눌렀을 떄)
        if (테스트 모드면)
            :리턴;
            end
        endif
        :Caret이 켜지고
        깜박이기 애니메이션 시작;
    endif    
    }

    
    card Update{
        if(테스트 모드면)
            :테스트 함수 실행후 리턴;
            end
        endif

        if (키보드 안보이면)
            :Caret Anim중지;
        endif
        if(컨텐츠 타입에 맞는 글자라면)
            split 
                :텍스트 업데이트;
            split again
                :커서인덱스 수정;
            end split
        endif
    }
```

|함수|설명
|-|-
|TestCaretTest           |CaretTest코드
|Start                   |초기화시작
|MixedRealityKeyboardInit|키보드초기화
|TextComponentInit       |텍스트 컴포넌트 초기화
|CaretInit               |커서초기화
|OnDisable               |커서 깜박이는 애님 중지
|OpenSystemKeyboard      |홀로렌즈 Keyboard열기
|Update                  |KeyboardData확인.
|UpdateCaret             |커서 위치 업데이트
|BlinkCaret              |커서 깜박이는 애님 시작
|StopCaretAnim           |커서 깜박이는 애님 중지
|CheckContentType        |컨텐츠 타입비교
|CheckingCharType        |문자열별 언어비교

### 메모
텍스트 TMP 우선 한글로 만들고.
엔터 누르면 꺼지고, 엔터 누르면 나타내는거 확인함. 요거...
키보드 사라질때 키보드 값이 없어지는 듯 하다..?



### 참조문서

[Mixedreality키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/develop/unity/keyboard-input-in-unity)
[MRTK2-기능-UX구성-시스템키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/mrtk-unity/mrtk2/features/ux-building-blocks/system-keyboard?view=mrtkunity-2022-05)
[MRTK2-기능-실험적-MixedReality키보드](https://learn.microsoft.com/ko-kr/windows/mixed-reality/mrtk-unity/mrtk2/features/experimental/mixed-reality-keyboard?view=mrtkunity-2022-05)
[MixedRealityKeyboard](https://learn.microsoft.com/ko-kr/dotnet/api/microsoft.mixedreality.toolkit.experimental.ui.mixedrealitykeyboard?preserve-view=true&view=mixed-reality-toolkit-unity-2020-dotnet-2.8.0)
[UnityTouchKeyBoard](https://docs.unity3d.com/ScriptReference/TouchScreenKeyboard.Open.html)
[InputPane](https://learn.microsoft.com/en-us/uwp/api/windows.ui.viewmanagement?view=winrt-22621)


### 버리고 새로운 키보드를 만든다면 참조할 자료..
campfireunion/VRKeys: A drum-style keyboard for VR in Unity
https://github.com/NormalVR/CutieKeys
https://github.com/campfireunion/VRKeys
https://github.com/vuplex/unity-keyboard