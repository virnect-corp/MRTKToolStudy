using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
//#if WINDOWS_UWP
using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections;
//#endif

public class MRTKKeyboardCustom : MonoBehaviour, IDebugtoolAction, IMixedRealityPointerHandler
{
#region EditorTest
    public Action[] RegistAction()
    {
        return new Action[]{InitKeyboard,
        ShowKeyboard,
        HideKeyboard,
        };
    }
#endregion
#region Properties
    public int CaretIndex
    {
        get;
        private set;
    } = 0;
    private string text = string.Empty;
    public string Text
    {
        get { return text; }
        protected set
        {
            if (value != text)
            {
                text = value;
                if (inputField != null)
                {
                    inputField.text = text;
                    //InputField.textComponent.ForceMeshUpdate();
                }
            }
        }
    }
#endregion properties
    TMP_InputField inputField;
    float inputFieldHeight;

    [SerializeField] GameObject caretPrefab;
    GameObject caretObject;
    IEnumerator blinkCaretCoroutine;

    MixedRealityInputModule inputModule = null;
    public TouchScreenKeyboard keyboard = null;

    protected virtual void Awake()
    {
        inputModule = CameraCache.Main.GetComponent<MixedRealityInputModule>();
    }
    void FixedUpdate() 
    {
        if(keyboard==null) return;
        UpdateText();
        UpdateCaret();
    }
    void InitKeyboard()
    {
        inputFieldHeight = inputField.textComponent.GetComponent<RectTransform>().rect.height;
    #if WINDOWS_UWP
        inputModule.ProcessPaused = false;
        if(keyboard==null)
        {
            keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default, false, false, false, false);
        }
    #endif
        CaretInit();
    }

    void CaretInit()
    {
        if(caretObject==null)
        {
            caretObject = Instantiate(caretPrefab,inputField.textComponent.transform,false);
            caretObject.SetActive(false);
        RectTransform rect = caretObject.GetComponent<RectTransform>();
        rect.anchoredPosition3D = Vector3.zero;
        rect.localRotation = Quaternion.identity;
        rect.localScale = Vector3.one;
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, inputFieldHeight * 0.9f);
        }
    }
    
    #region IMixedRealityPointerHandler implementation

    public void OnPointerDown(MixedRealityPointerEventData eventData) { }
    public void OnPointerDragged(MixedRealityPointerEventData eventData) { }
    public void OnPointerUp(MixedRealityPointerEventData eventData) { }
    public void OnPointerClicked(MixedRealityPointerEventData eventData) => ShowKeyboard();

    #endregion

    public void ShowKeyboard() => SetActiveKeyboardObject(true );
    public void HideKeyboard() => SetActiveKeyboardObject(false);
    void SetActiveKeyboardObject(bool value)
    {
        InitKeyboard();
        if(keyboard    != null) keyboard.active = value;
        if(caretObject != null) caretObject.SetActive(value);
        if(value == true){
            keyboard.text = Text;
            keyboard.selection = new RangeInt(Text.Length,1);
        }
    }

    void UpdateText()
    {
        if (keyboard != null && keyboard.active)
        {
            CaretIndex = keyboard.selection.end;
            Text = keyboard.text;
            // if (!multiLine)
            // {
            //     if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            //     {
            //         HideKeyboard();
            //     }
            // }
        }
    }
    private void UpdateCaret()
    {
        if (caretObject == null) 
            return;
        if(keyboard.active)
        {
            $"caretObject : {caretObject}, keyboard.active : {keyboard.active}, blinkCaretCoroutine : {blinkCaretCoroutine}".Log();
            if(blinkCaretCoroutine==null)
            {
                blinkCaretCoroutine = BlinkCaretAnim();
                StartCoroutine(blinkCaretCoroutine);
            }
        }
        else
        {
            if(blinkCaretCoroutine!=null)
            {
                StopCoroutine(blinkCaretCoroutine);
                blinkCaretCoroutine = null;
                caretObject.SetActive(false);
            }
        }
            CaretIndex = Mathf.Clamp(CaretIndex, 0, string.IsNullOrEmpty(text) ? 0 : text.Length);
            var textComponent = inputField.textComponent;
            Vector3 caretPosition;
            if (CaretIndex == text.Length)
            {
                caretPosition = textComponent.textInfo.characterInfo[CaretIndex - 1].topRight;
            }
            else if(CaretIndex == 0)
            {
                caretPosition = Vector3.zero;
            }
            else
            {
                caretPosition = textComponent.textInfo.characterInfo[CaretIndex].topLeft;
            }
            caretPosition.y = -inputFieldHeight * .1f;
            caretPosition.z = 0.0f;
            var resultPosition = inputField.transform.TransformPoint(caretPosition);
            caretObject.transform.position = resultPosition;

    }
    private IEnumerator BlinkCaretAnim()
    {
        while (caretObject != null)
        {
            if(caretObject != null)
            {
                caretObject.gameObject.SetActive(!caretObject.gameObject.activeSelf);
            }
            yield return new WaitForSeconds(0.53f);
        }
    }
    private TouchScreenKeyboardType GetConvertMRTKTextType(TMP_InputField.ContentType contentType)
    {
        return contentType switch
        {
            TMP_InputField.ContentType.Standard      => TouchScreenKeyboardType.Default,
            TMP_InputField.ContentType.Autocorrected => TouchScreenKeyboardType.Default,
            TMP_InputField.ContentType.IntegerNumber => TouchScreenKeyboardType.NumberPad,
            TMP_InputField.ContentType.DecimalNumber => TouchScreenKeyboardType.NumberPad,
            TMP_InputField.ContentType.Alphanumeric  => TouchScreenKeyboardType.Default,
            TMP_InputField.ContentType.Name          => TouchScreenKeyboardType.Default,
            TMP_InputField.ContentType.EmailAddress  => TouchScreenKeyboardType.EmailAddress,
            TMP_InputField.ContentType.Password      => TouchScreenKeyboardType.Default,
            TMP_InputField.ContentType.Pin           => TouchScreenKeyboardType.NumberPad,
            _ => TouchScreenKeyboardType.Default
        };
    }
}
