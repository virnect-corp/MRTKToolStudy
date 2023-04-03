using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace CustomInputSystem.Keyboard
{
    [RequireComponent(typeof(TMP_InputField)), RequireComponent(typeof(Interactable))]
    public class SystemKeyboardCustom : MonoBehaviour
    {
#region TestCursor
#if UNITY_EDITOR
    [SerializeField]
    bool testCursor;
    [SerializeField]
    string testText;
    [SerializeField]
    int testCursorIdx;
    int testPrevLength;
    bool prevTestCursorValue;
    public bool PrevTestCursorValue
    {
        get 
        {
            return prevTestCursorValue;
        }
        set
        {
            if(prevTestCursorValue!=value)
            {
                prevTestCursorValue = value;
                CursorChecking(value);
            }
        }
    }
    void CursorChecking(bool value)
    {
        Debug.Log($"CursorChecking : {value}");
        if(value == true && blinkCaretAnim==null)
        {
            InputField.interactable = false;
            RectTransform rect = InputField.textComponent.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0,0);
            rect.anchorMax = new Vector2(1,1);
            rect.pivot = new Vector2(0.5f ,0.5f);
            var interactable = GetComponent<Interactable>();
            if(interactable.Profiles[0].Target == null)
            {
                interactable.Profiles[0].Target = gameObject;
            }
            if(interactable.Profiles[0].Themes == null)
            {
                List<Theme> themeList = new List<Theme>();
                themeList.Add(Resources.Load<Microsoft.MixedReality.Toolkit.UI.Theme>("InteratableThema"));
                interactable.Profiles[0].Themes = themeList;
            }
            testUpdateCoroutine = StartCoroutine(TextUpdateCheck());
            blinkCaretAnim = StartCoroutine(BlinkCaret());
        }
        if(value== false && blinkCaretAnim!=null)
        {
            StopCoroutine(blinkCaretAnim);
            StopCoroutine(testUpdateCoroutine);
        }
    }

#endif
#endregion
#region Attribute
        [SerializeField]
        private UnityEvent onShowKeyboard;
        [SerializeField]
        private UnityEvent<string> onHideKeyboard;
        private MixedRealityKeyboard mixedRealityKeyboard;
        GameObject caretPrefabs;
        Coroutine blinkCaretAnim;
        Coroutine testUpdateCoroutine;
        float inputFieldHeight;
        string prevText;
        float cursorDepth;
#endregion Attribute
#region Property 
    private TMP_InputField inputField = null;
    public TMP_InputField InputField
    {
        get 
        { 
            if(inputField==null)
            {
                inputField = GetComponent<TMP_InputField>();
            }
            return inputField; 
        }
        set
        {
            if(value == null) return;
            inputField = value;
            inputField.text = Text;
            inputField.ForceLabelUpdate();
            UpdateCaret();
        }
    }
    private Transform caret = null;
    public Transform Caret
    {
        get 
        { 
            if(caret == null)
            {
                if(caretPrefabs ==null)
                {
                    caretPrefabs = Resources.Load<GameObject>("CaretRootForCanvas");
                }
                caret = Instantiate(caretPrefabs, InputField.textComponent.transform).transform;
                RectTransform rect = caret.GetComponent<RectTransform>();
                rect.anchoredPosition3D = new Vector3(0,0,cursorDepth);
                rect.localRotation = Quaternion.identity;
                rect.localScale = Vector3.one;
                rect.sizeDelta = new Vector2(rect.sizeDelta.x, inputFieldHeight * 0.9f);
            }
            return caret; 
        }
        set
        {
            if (caret != value)
            {
                caret = value;
                UpdateCaret();
            }
        }
    }
    private string text = string.Empty;
    public string Text
    {
        get { return text; }
        set
        {
            if (value != text)
            {
                text = value;
                if (InputField != null)
                {
                    if(string.IsNullOrEmpty(text) == false)
                    {
                        InputField.text = text;
                        InputField.ForceLabelUpdate();
                        Canvas.ForceUpdateCanvases();
                    }
                }
                UpdateCaret();
            }
        }
    }
    private int caretIndex = 0;
    public int CaretIndex
    {
        get { return caretIndex; }
        set
        {
            if (value != caretIndex)
            {
                caretIndex = value;
                UpdateCaret();
            }
        }
    }
#endregion Property 
#region MonoBehaviour Implementation
        void Start()
        {
            ComponentInit();
            EventInit();
        }
#if UNITY_EDITOR
        void Update()
        {
            PrevTestCursorValue = testCursor;
        }
#endif
#endregion MonoBehaviour Implementation
#region Initialize
        void ComponentInit()
        {
            var parentCanvas = GetComponentInParent<Canvas>();
            if( parentCanvas &&
                parentCanvas.rootCanvas && 
                parentCanvas.rootCanvas.gameObject.GetComponent<NearInteractionTouchableUnityUI>() == false)
            {
                parentCanvas.rootCanvas.gameObject.AddComponent<NearInteractionTouchableUnityUI>();
            }

            inputFieldHeight = InputField.textComponent.GetComponent<RectTransform>().rect.height;
#if UNITY_EDITOR
            inputField.interactable = true;
#elif WINDOWS_UWP
            inputField.interactable = false;

            RectTransform rect = InputField.textComponent.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0,0);
            rect.anchorMax = new Vector2(1,1);
            // rect.pivot = new Vector2(0.0f ,0.5f); 이건 mrtk에서 pivot 조금씩 달라서 픽스하는건 우선 포기 개선사항.
            cursorDepth = rect.GetComponent<RectTransform>().anchoredPosition3D.z;
            var interactable = GetComponent<Interactable>();
            var profile = interactable.Profiles;
            if(profile.Count > 0)
            {
                if(profile[0].Target == null)
                {
                    profile[0].Target = gameObject;
                }
                if(profile[0].Themes == null)
                {
                    List<Theme> themeList = new List<Theme>();
                    themeList.Add(Resources.Load<Microsoft.MixedReality.Toolkit.UI.Theme>("InteratableThema"));
                    profile[0].Themes = themeList;
                }
            }
            if(gameObject.GetComponent<MixedRealityKeyboard>()==null)
            {
                mixedRealityKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();     
            }
            mixedRealityKeyboard.DisableUIInteractionWhenTyping = false;
#endif
        }
        void EventInit()
        {
#if WINDOWS_UWP
            if (mixedRealityKeyboard.OnShowKeyboard != null)
            {
                mixedRealityKeyboard.OnShowKeyboard.AddListener(() => 
                {
                    if(blinkCaretAnim==null)
                    {
                        testUpdateCoroutine = StartCoroutine(TextUpdateCheck());
                        blinkCaretAnim = StartCoroutine(BlinkCaret());
                        onShowKeyboard?.Invoke();
                        UpdateCaret();
                    }
                });
            }
            if (mixedRealityKeyboard.OnHideKeyboard != null)
            {
                mixedRealityKeyboard.OnHideKeyboard.AddListener(() => 
                {
                    onHideKeyboard?.Invoke(Text);
                });
            }
            var interactable = GetComponent<Interactable>();
            interactable.OnClick.AddListener(() => OpenSystemKeyboard());
            var interactableOnTouchReceiver = interactable.AddReceiver<InteractableOnTouchReceiver>();
            interactableOnTouchReceiver.OnTouchEnd.AddListener(() => OpenSystemKeyboard());
#endif
        }
#endregion Initialize
        private IEnumerator TextUpdateCheck()
        {
            while(true)
            {
#if UNITY_EDITOR
                Text = testText;
                // Debug.Log(testPrevLength != testText.Length);
                if(testPrevLength != testText.Length)
                {
                    testPrevLength = testText.Length;
                    CaretIndex = testText.Length;
                    testCursorIdx = CaretIndex;
                }
                else
                {
                    CaretIndex = testCursorIdx;
                }

                yield return null;
#elif WINDOWS_UWP
                if (mixedRealityKeyboard.Visible)
                {
                    prevText = Text;
                    Text = mixedRealityKeyboard.Text;
                    CaretIndex = mixedRealityKeyboard.CaretIndex;
                    if(string.IsNullOrEmpty(mixedRealityKeyboard.Text) && CaretIndex==0)
                    {
                        InputField.text = string.Empty;
                    }
                }
                else
                {
                    if(blinkCaretAnim != null)
                    {
                        StopCoroutine(blinkCaretAnim);
                        blinkCaretAnim = null;
                        Caret.gameObject.SetActive(false);
                        mixedRealityKeyboard.HideKeyboard();
                        Text = prevText;
                    }
                    StopCoroutine(testUpdateCoroutine);
                }
                yield return new WaitForSeconds(0.1f);
#endif
            }
        }
#region mixedRealityKeyboard
        public void OpenSystemKeyboard()
        {
    #if UNITY_EDITOR
            Debug.Log("OpenSystemKeyboard");
            InputField.OnSelect(null);
    #elif WINDOWS_UWP
            mixedRealityKeyboard.HideKeyboard();
            Text = InputField.text;
            mixedRealityKeyboard.ShowKeyboard(Text, false);
    #endif
        }

        public void ClearText()
        {
#if WINDOWS_UWP
            Text = string.Empty;
            if(mixedRealityKeyboard.Visible)
            {
                mixedRealityKeyboard.ShowKeyboard(string.Empty, false);
            }
#endif
        }
#endregion
#region Caret관련
        private void UpdateCaret()
        {
            if (Caret != null)
            {
                TMP_CharacterInfo[] textinfo = InputField.textComponent.textInfo.characterInfo;
                CaretIndex = Mathf.Clamp(CaretIndex, 0, string.IsNullOrEmpty(Text) ? 0 : Text.Length);
                Vector3 caretPosition;
                if(Text.Length==0)
                {
                    Caret.localPosition = Vector3.zero;
                    InputField.text = string.Empty;
                }
                if (CaretIndex == Text.Length && Text.Length !=0)
                {
                    caretPosition = textinfo[CaretIndex - 1].topRight;
                }
                else if(CaretIndex == 0 && (textinfo.Length < CaretIndex || textinfo.Length > CaretIndex))
                {
                    caretPosition = textinfo[0].topLeft;
                }
                else
                {
                    caretPosition = textinfo[CaretIndex].topLeft;
                }
                RectTransform captionRect = InputField.textComponent.GetComponent<RectTransform>();
                RectTransform viewPort = InputField.textViewport.GetComponent<RectTransform>();
                caretPosition.x -= (captionRect.rect.width * 0.5f);
                caretPosition.y = -inputFieldHeight * .1f;
                caretPosition.z = cursorDepth;
                var worldPosition = InputField.transform.TransformPoint(caretPosition);
                Caret.transform.position = worldPosition;
            }
        }
        private IEnumerator BlinkCaret()
        {
            while (Caret != null)
            {
                Caret.gameObject.SetActive(!Caret.gameObject.activeSelf);
                const float blinkTime = 0.53f;
                yield return new WaitForSeconds(blinkTime);
            }
        }
        // void DebugLog(string position)
        // {
        //     $"{position} | visible : {mixedRealityKeyboard.Visible}, idx : {CaretIndex}, pos : {Caret.transform.position} text : {mixedRealityKeyboard.Text}, InputField : {InputField.text}, inputCaption : {InputField.textComponent.text}".Log();
        // }
#endregion
    }

}
