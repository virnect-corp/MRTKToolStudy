// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using Microsoft.MixedReality.Toolkit.Experimental.UI;
// using Microsoft.MixedReality.Toolkit.Input;
// using Microsoft.MixedReality.Toolkit.UI;
// using TMPro;
// using UnityEngine;

// [RequireComponent(typeof(Interactable))]
// public class KeyboardCanvasInputFieldClick2 : KeyboardBaseCustom
// {
        

// #region  테스트코드임
// #if UNITY_EDITOR
//     public bool isTest;
//     public bool isKeyboardVisibleTest;
//     public string testString;
// #endif
// #endregion

//     MixedRealityKeyboard wmrKeyboard;

//     GameObject caretPrefab;
//     Coroutine caretCoroutine;
//     TMP_InputField inputField = null;

//     public TMP_InputField InputField
//     {
//         get { return inputField; } 
//         set
//         {
//             if (inputField != value)
//             {
//                 $"InputField : {InputField.text}".Log();
//                 inputField = value;
//                 if (inputField != null)
//                 {
//                     UpdateCaret();
//                 }
//             }
//         }
//     }
//     Transform previewCaret = null;
//     public Transform PreviewCaret
//     {
//         get 
//         { 
//             return previewCaret; 
//         }
//         set
//         {
//             if (previewCaret != value)
//             {
//                 $"PreviewCaret : {PreviewCaret.position}".Log();
//                 previewCaret = value;
//                 UpdateCaret();
//             }
//         }
//     }
//     private int caretIndex = 0;
//     public int CaretIndex
//     {
//         get { return caretIndex; }
//         set
//         {
            
//             if (value != caretIndex)
//             {
//                 $"caretIndex : {caretIndex}".Log();
//                 caretIndex = value;
//                 UpdateCaret();
//             }
//         }
//     }

//     private string text = string.Empty;
//     public override string Text
//     {
//         get { return text; }
//         protected set
//         {
//             if (value != text)
//             {
//                 text = value;
//                 if (InputField != null)
//                 {
//                     InputField.text = text;
//                     //InputField.textComponent.ForceMeshUpdate();
//                 }
//             }
//         }
//     }
//     bool disableUIInteractionWhenTypingCheck = false;
//     bool isKeyboardVisible = false;
//     float caretMargin=0;
// #region 초기화하기.
//     private void Start() {
// #if UNITY_EDITOR
//         if(isTest == false)
//         {
//             gameObject. GetComponent<KeyboardCanvasInputFieldClick2>().enabled = false;
//             return;
//         }
// #endif
//         MixedRealityKeyboardInit();
//         TextComponentInit();
//         CaretInit();
//         EventInit();
//     }
//     void MixedRealityKeyboardInit()
//     {
//         wmrKeyboard = gameObject.AddComponent<MixedRealityKeyboard>();
//         wmrKeyboard.DisableUIInteractionWhenTyping = disableUIInteractionWhenTypingCheck;
        
//     }
//     void TextComponentInit()
//     {
//         if(GetComponentInParent<Canvas>().rootCanvas)
//         {
//             GetComponentInParent<Canvas>().rootCanvas.gameObject.AddComponent<NearInteractionTouchableUnityUI>();
//         }
//         inputField = GetComponent<TMP_InputField>();
//         if(inputField==null)
//         {
//             inputField = gameObject.AddComponent<TMP_InputField>();
//         }
//         RectTransform rect = inputField.textComponent.GetComponent<RectTransform>();
//         rect.anchorMin = new Vector2(0,0.5f);
//         rect.anchorMax = new Vector2(0,0.5f);
//         rect.pivot = new Vector2(0,0.5f);
//         rect.anchoredPosition = Vector2.zero;
//     }
//     void CaretInit()
//     {
//         if(caretPrefab==null) 
//         {
//             // caretPrefab = Resources.Load<GameObject>("CaretRootForCanvas");
//         }
//         if(inputField)
//         {
//             caretMargin = InputField.textViewport.GetComponent<RectTransform>().offsetMin.x;
//         }

//         GameObject caret = Instantiate(caretPrefab,InputField.textComponent.transform,false);
//         PreviewCaret = caret.transform;
//         caret.SetActive(false);

//         RectTransform rect = PreviewCaret.GetComponent<RectTransform>();
//         rect.anchoredPosition3D = Vector3.zero;
//         rect.localRotation = Quaternion.identity;
//         rect.localScale = Vector3.one;
//         rect.sizeDelta = new Vector2(rect.sizeDelta.x, InputField.textComponent.GetComponent<RectTransform>().rect.height * 0.85f);
//     }
//     private void EventInit()
//     {
//         var interactable = inputField.GetComponent<Interactable>();
//         interactable.IsTargeted = gameObject;
//         if(interactable.Profiles[0].Themes ==null)
//         {
//             List<Theme> themeList = new List<Theme>();
//             themeList.Add(Resources.Load<Microsoft.MixedReality.Toolkit.UI.Theme>(""));
//             interactable.Profiles[0].Themes = themeList;
//         }
//         interactable.OnClick.AddListener(() => OpenSystemKeyboard());
//         var interactableOnTouchReceiver =interactable.AddReceiver<InteractableOnTouchReceiver>();
//         // interactableOnTouchReceiver.OnTouchStart.AddListener(() => OpenSystemKeyboard());
//         interactableOnTouchReceiver.OnTouchEnd.AddListener(() => OpenSystemKeyboard());
//     }

// #endregion    
//     public void OpenSystemKeyboard()
//     {
// #if UNITY_EDITOR
//         if(isTest == false) return;
// #elif WINDOWS_UWP
//         if(wmrKeyboard.Visible == false)
//         {
//             wmrKeyboard.ShowKeyboard(Text, false);
//         }
// #endif
//         if(previewCaret!=null)
//         {
//             isKeyboardVisible = true;
//             previewCaret.gameObject.SetActive(true);
//             if(caretCoroutine==null)
//             {
//                 caretCoroutine = StartCoroutine(BlinkCaret());
//             }
//         }
//     }
//     private void Update()
//     {
//         if (inputField != null)
//         {
//             if(isKeyboardVisible)
//             {
// #if UNITY_EDITOR
//                 if(SetText(testString))return;
// #endif
//                 if(wmrKeyboard.Visible == false)
//                 {
//                     isKeyboardVisible = false;
//                     StopCaretAnim();
//                     return;
//                 }
//                 $"wmrKeyboard.Text : {wmrKeyboard.Text}".Log();
//                 SetText(wmrKeyboard.Text);
//                 CaretIndex = Text.Length;
//             }
//         }
//     }
//     bool SetText(string text){
//             Text = text;
//             inputField.text = text;
//             inputField.ForceLabelUpdate();
// //         if(CheckContentType(text))
// //         {
// //         }
// // #if WINDOWS_UWP
// //         else
// //         {
// //         keyboard.Text = text.Remove(text.Length-1);
// //         }
// // #endif
//         return true;
//     }
//     private void UpdateCaret()
//     {
//         caretIndex = Mathf.Clamp(caretIndex, 0, string.IsNullOrEmpty(text) ? 0 : text.Length);
//         if (previewCaret != null)
//         {
//             if (caretIndex == 0)
//             {
//                 previewCaret.transform.localPosition = Vector3.zero;
//             }
//             else
//             {
//                 var textComponent = InputField.textComponent;
//                 Vector3 localPosition;
//                 if (caretIndex == text.Length)
//                 {
//                     localPosition = textComponent.textInfo.characterInfo[caretIndex - 1].topRight;
//                 }
//                 else
//                 {
//                     localPosition = textComponent.textInfo.characterInfo[caretIndex].topLeft;
//                 }
//                 localPosition.x += caretMargin;
//                 localPosition.y = 0;
//                 localPosition.z = 0.0f;
//                 $"localPosition : {localPosition}".Log();
//                 var position = InputField.transform.TransformPoint(localPosition);
//                 previewCaret.position = position;
//             }
//         }
//     }
//     private IEnumerator BlinkCaret()
//     {
//         while (previewCaret != null)
//         {
//             if(previewCaret != null)
//             {
//                 previewCaret.gameObject.SetActive(!previewCaret.gameObject.activeSelf);
//             }
//             yield return new WaitForSeconds(0.53f);
//         }
//     }
//     void StopCaretAnim()
//     {
//         if(caretCoroutine != null)
//         {
//             StopCoroutine(caretCoroutine);
//             caretCoroutine = null;
//         }
//         if(previewCaret != null)
//         {
//             previewCaret.gameObject.SetActive(false);
//         }
//     }
//     // public bool CheckContentType(string text)
//     // {
//     //     switch(inputField.contentType)
//     //     {
//     //         case TMP_InputField.ContentType.Standard     : return true;
//     //         case TMP_InputField.ContentType.Autocorrected: return true;
//     //         case TMP_InputField.ContentType.IntegerNumber: return CheckingCharType(text, "N");
//     //         case TMP_InputField.ContentType.DecimalNumber: return CheckingCharType(text, "N");
//     //         case TMP_InputField.ContentType.Alphanumeric : return CheckingCharType(text, "KEN");
//     //         case TMP_InputField.ContentType.Name         : return true;
//     //         case TMP_InputField.ContentType.EmailAddress : return CheckingCharType(text, "ESNO");
//     //         case TMP_InputField.ContentType.Password     : return true;
//     //         case TMP_InputField.ContentType.Pin          : return true;
//     //         case TMP_InputField.ContentType.Custom       : return true;
//     //     }
//     //     return false;
//     // }
//     // public bool CheckingCharType(string str, string checkPoint) {
//     //     string strResult= null;
//     //     string strAcChar = "!@#$%^&*()?><-_=+[]|`'";
//     //     char[] chArr = str.ToCharArray();
//     //     if(chArr.Length<=0) return false;
//     //     char ch = str.ToCharArray()[str.Length-1];
//     //     //한글
//     //     if ((0xAC00 <= ch && ch <= 0xD7A3) || (0x3131 <= ch && ch <= 0x318E))
//     //     {
//     //         strResult = "K";
//     //     }
//     //     //영어
//     //     else if ((0x61 <= ch && ch <= 0x7A) || (0x41 <= ch && ch <= 0x5A))
//     //     {
//     //         strResult = "E";
//     //     }
//     //     //숫자
//     //     else if (0x30 <= ch && ch <= 0x39)
//     //     {
//     //         strResult = "N";
//     //     }
//     //     else if (0x0080 <= ch && ch <= 0x00FF)
//     //     {
//     //         strResult = "S";
//     //     }
//     //     //특수문자외 지정된 문자
//     //     else if (strAcChar.Contains<char>(ch))
//     //     {
//     //         strResult = "O";
//     //     }

//     //     var compareChar = checkPoint.ToCharArray();
//     //     for (int i=0; i < compareChar.Length; i++)
//     //     {
//     //         if(strResult.Contains(compareChar[i]) == false)
//     //         {
//     //             return true;
//     //         }
//     //     }
//     //     return false;
//     // }

// }