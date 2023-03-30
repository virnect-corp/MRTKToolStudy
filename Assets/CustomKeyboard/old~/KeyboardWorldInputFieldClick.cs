using System.Collections;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using TMPro;
using UnityEngine;

public class KeyboardWorldInputFieldClick : MonoBehaviour
{
#region  테스트코드임
#if UNITY_EDITOR
    public bool isTest;
    public string testString;
    bool TestCaretTest()
    {
        Text = testString;
        previewText.text = testString;
        CaretIndex       = testString.Length;
        return isTest;
    }
#endif
#endregion

    MixedRealityKeyboard wmrKeyboard;
    [SerializeField]
    GameObject caretPrefab;
    Coroutine caretCoroutine;
    TextMeshPro previewText = null;
    public TextMeshPro PreviewText
    {
        get { return previewText; } 
        set
        {
            if (previewText != value)
            {
                previewText = value;
                if (previewText != null)
                {
                    previewText.text = Text;
                    UpdateCaret();
                }
            }
        }
    }
    TextMeshPro inputField;
    public TextMeshPro PreviewInputField
    {
        get { return previewText; } 
        set
        {
            if (previewText != value)
            {
                previewText = value;
                if (previewText != null)
                {
                    previewText.text = Text;
                    UpdateCaret();
                }
            }
        }
    }

    Transform previewCaret = null;
    public Transform PreviewCaret
    {
        get 
        { 
            return previewCaret; 
        }
        set
        {
            if (previewCaret != value)
            {
                previewCaret = value;
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
                if (PreviewText != null)
                {
                    PreviewText.text = text;
                    PreviewText.ForceMeshUpdate();
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
#if UNITY_EDITOR
                Debug.Log(testString.Length);
#endif
                caretIndex = value;
                UpdateCaret();
            }
        }
    }
    bool disableUIInteractionWhenTyping = false;

    void OnEnable()
    {
        if(caretCoroutine!=null)
            caretCoroutine = StartCoroutine(BlinkCaret());
    }
    void OnDisable()
    {
        StopCoroutine(caretCoroutine);
        caretCoroutine=null;
    }
    private void Start() => KeyBoardInputFieldInit();
    private void Update()
    {
        if (previewText != null)
        {
#if UNITY_EDITOR
            if(TestCaretTest())return;
#endif
            bool isVisible = wmrKeyboard.Visible;
            Text = wmrKeyboard.Text;
            CaretIndex       = wmrKeyboard.CaretIndex;
        }
    }
    public void OpenSystemKeyboard()
    {
        wmrKeyboard.ShowKeyboard(wmrKeyboard.Text, false);
    }
    void KeyBoardInputFieldInit()
    {
        MixedRealityKeyboardInit();
        PreviewTextInit();
        CaretInit();
    }
    void MixedRealityKeyboardInit()
    {
        wmrKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
        wmrKeyboard.DisableUIInteractionWhenTyping = disableUIInteractionWhenTyping;
    }
    void PreviewTextInit()
    {
        previewText = GetComponent<TextMeshPro>();
        if(previewText==null)
        {
            previewText = gameObject.AddComponent<TextMeshPro>();
        }
    }
    void CaretInit()
    {
        var caret = Instantiate(caretPrefab,transform,false);
        caret.transform.localPosition = new Vector3(-0.057f,0,0);
        caret.transform.localRotation = Quaternion.identity;
        caret.transform.localScale = Vector3.one;
        PreviewCaret = caret.transform;
        caretCoroutine = StartCoroutine(BlinkCaret());
    }
    private void UpdateCaret()
    {
        caretIndex = Mathf.Clamp(caretIndex, 0, string.IsNullOrEmpty(text) ? 0 : text.Length);
        if (previewCaret != null)
        {
            if (caretIndex == 0)
            {
                previewCaret.transform.localPosition = Vector3.zero;
            }
            else
            {
                Vector3 localPosition;
                if (caretIndex == text.Length)
                {
                    localPosition = PreviewText.textInfo.characterInfo[caretIndex - 1].topRight;
                }
                else
                {
                    localPosition = PreviewText.textInfo.characterInfo[caretIndex].topLeft;
                }
                localPosition.y = 0.0f;
                localPosition.z = 0.0f;
                var position = PreviewText.transform.TransformPoint(localPosition);
                previewCaret.transform.position = position;
            }
        }
    }
    private IEnumerator BlinkCaret()
    {
#if WINDOWS_UWP
        while (previewCaret != null)
#elif UNITY_EDITOR
        while (true)
#endif
        {
            if(previewCaret!=null)
            {
                previewCaret.gameObject.SetActive(!previewCaret.gameObject.activeSelf);
            }
            yield return new WaitForSeconds(0.53f);
        }
    }
}