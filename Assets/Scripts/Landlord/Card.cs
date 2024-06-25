using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour,IPointerClickHandler
{
    /// <summary>
    /// 卡片名字
    /// </summary>
    public string cardName;
    /// <summary>
    /// 判断是否被选中
    /// </summary>
    private bool mIsSelect;
    /// <summary>
    /// 判断是否可交互
    /// </summary>
    public bool mInteraction;



    void Start()
    {
        mIsSelect = false;
    }

    

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!mInteraction)return;

        if (!mIsSelect)
        {
            transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
            mIsSelect = !mIsSelect;
            CardManager.Instance.OnClickEvent?.Invoke(this);
        }
        else
        {
            transform.localScale = Vector3.one;
            mIsSelect = !mIsSelect;
            CardManager.Instance.OnCancelClickEvent?.Invoke(this);
        }
            
    }
}
