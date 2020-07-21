using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ItemRemove : MonoBehaviour
{ 
    Vector2 v0;
    Vector2 v1;

    public Text itemText;
    public InputField targetField;
    public Text debugTxt;
    public static int pos;
    public void PDown()
    {
#if UNITY_IOS || UNITY_ANDROID&&!UNITY_EDITOR
        Debug.Log("down");
        Touch touch = Input.touches[0];
        v0 = touch.position;
#endif
#if UNITY_STANDALONE_WIN||UNITY_EDITOR
        v0 = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
        print(v0);
    }
    public void PUp()
    {
#if UNITY_IOS || UNITY_ANDROID&&!UNITY_EDITOR
        Debug.Log("up");
        Touch touch = Input.touches[0];
        v1 = touch.position;
#endif
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        v1 = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
#endif
#if UNITY_IOS || UNITY_ANDROID && !UNITY_EDITOR
        print(v1);
        if (((v1 - v0).x > 300) && ((v1 - v0).y < 150))
        {
            GameObject.Destroy(this.gameObject);
        }
#endif
        //data = (v1 - v0).x.ToString();
        //textDisplay.text = data;
        //textDisplay.gameObject.SetActive(true);
    }
    public void deleteItem() 
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        if (Input.GetMouseButton(1))
        {
            GameObject.Destroy(this.gameObject);
        }
#endif
    }
    //    public void InsertDown()
    //    {
    //        //GameObject target = GameObject.FindGameObjectWithTag("TargetText");
    //#if UNITY_IOS || UNITY_ANDROID
    //        GUIUtility.systemCopyBuffer = itemText.text;
    //#endif
    //#if UNITY_STANDALONE_WIN
    //        string oldText = targetField.text;

    //        print(pos+".......");
    //            if (pos == 0)
    //            {
    //            print(pos + "-->zero"+":[oldText]:"+oldText);   
    //            targetField.text = itemText.text + oldText;
    //            }
    //            else if(pos>0)
    //            {
    //                targetField.text = oldText.Substring(0, pos) + itemText.text + oldText.Substring(pos, oldText.Length - pos);
    //            }
    //#endif
    //}
    //public void getCaretPos()
    //{
    //#if UNITY_STANDALONE_WIN
    //        pos = targetField.caretPosition;
    //        print(pos+"get");
    //#endif
    //}
    public void copyToField()
    {
        GUIUtility.systemCopyBuffer = itemText.text;
        targetField.text = itemText.text;
        debugTxt.text = "Account copied";
    }
}
