<<<<<<< HEAD
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardTypeEnum
{
    Single,
    Pair,
    Third,
    Bomb,
    ThreePlusOne,
    Sequence,
    SequencePair,
}
public class CardManager : SingletonMonoBehaviour<CardManager>
{
    /// <summary>
    /// 被选中的卡片列表
    /// </summary>
    public List<Card> selectedCard = new List<Card>();
    /// <summary>
    /// 卡片被选中事件监听
    /// </summary>
    public Action<Card> OnClickEvent;
    /// <summary>
    /// 卡片被取消选中事件监听
    /// </summary>
    public Action<Card> OnCancelClickEvent;
    /// <summary>
    /// 出牌按钮
    /// </summary>
    [SerializeField] private Button mPlayCardButton;
    /// <summary>
    /// 提示按钮
    /// </summary>
    [SerializeField] private Button mPromptButton;
    /// <summary>
    /// pass按钮
    /// </summary>
    [SerializeField] private Button mPassButton;
    /// <summary>
    /// 出牌的区域
    /// </summary>
    [SerializeField] private Transform mTableObj;
    /// <summary>
    /// 操作提示语句
    /// </summary>
    [SerializeField] private Text mPromptText;



    private void OnEnable()
    {
        mPlayCardButton.onClick.AddListener(PlayCard);


        OnClickEvent += OnCardClicked;
        OnCancelClickEvent += OnCardCancelClicked;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    private void PlayCard()
    {
        bool isPlay = false;
        int[] numbers = new int[selectedCard.Count];
        for (int i = 0; i < selectedCard.Count; i++)
        {
            numbers[i] = int.Parse(selectedCard[i].cardName.Split('-')[1]);
        }
        //判断是否符合出牌逻辑
        switch (numbers.Length)
        {
            case 2:
                if (numbers[0] == numbers[1])
                {
                    isPlay = true;
                }
                break;
            case 3:
                if (numbers[0] == numbers[1] && numbers[1] == numbers[2] 
                                             && numbers[0] == numbers[2])
                {
                    isPlay = true;
                }
                break;
            case 4:
                if (IsThreePlusOne(numbers))
                {
                    isPlay = true;
                }
                if (numbers[0] == numbers[1] && numbers[1] == numbers[2] 
                                             && numbers[2] == numbers[3] 
                                             && numbers[0] == numbers[3])
                {
                    isPlay = true;
                }
                
                break;
            default:
                
                if (IsSequence(numbers))
                {
                    isPlay = true;
                }
                if (IsSequencePair(numbers))
                {
                    isPlay = true;
                }
                break;
        }






        if (!isPlay) return;
        ClearTable();
        Card[] sortSelected = new Card[selectedCard.Count];
        for (int i = 0; i < selectedCard.Count; i++)
        {
            sortSelected[i] = selectedCard[i];
        }
        Array.Sort(sortSelected, (a, b) =>
        {
            var numberA = int.Parse(a.cardName.Split('-')[1]);
            var numberB = int.Parse(b.cardName.Split('-')[1]);
            if (numberA > numberB)
                return -1;
            if (numberA < numberB)
                return 1;
            return 0;
        });
        for (int i = 0; i < sortSelected.Length; i++)
            Main.Instance.SpawnCard(sortSelected[i].cardName, mTableObj,false);
        for (int i = 0; i < selectedCard.Count; i++)
            Destroy(selectedCard[i].gameObject);
        selectedCard.Clear();
    }

    /// <summary>
    /// 判断是否是顺子
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsSequence(int[] numbers)
    {
        if (numbers.Length < 5 || numbers.Length > 12) return false;
        //判断是否有不符合顺子的单牌
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > 14)
            {
                return false;
            }
        }
        Array.Sort(numbers);
        for (int i = 0; i < numbers.Length - 1; i++)
        {
            if (numbers[i] + 1 != numbers[i + 1])
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 判断是否是连对
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsSequencePair(int[] numbers)
    {
        if (numbers.Length < 6 || numbers.Length > 24) return false;
        //判断是否有不符合顺子的单牌
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > 14)
            {
                return false;
            }
        }
        Array.Sort(numbers);
        for (int i = 0; i < numbers.Length - 2; i += 2)
        {
            if (numbers[i] != numbers[i + 1] || numbers[i] + 1 != numbers[i + 2])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 判断是否三带一
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsThreePlusOne(int[] numbers)
    {
        if (numbers.Length != 4) return false;
        Array.Sort(numbers);
        if (numbers[0] == numbers[3]) return false;
        //判断带一是否大于三个的
        if (numbers[3] > numbers[0] && numbers[0] == numbers[1])
        {
            for (int i = 0; i < numbers.Length - 2; i ++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        if (numbers[3] > numbers[0] && numbers[0] != numbers[1])
        {
            for (int i = 1; i < numbers.Length - 1; i++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        if (numbers[0] > numbers[3])
        {
            for (int i = 1; i < numbers.Length - 1; i ++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 清空桌面
    /// </summary>
    public void ClearTable()
    {

        for (int i = 0; i < mTableObj.childCount; i++)
        {
            Destroy(mTableObj.GetChild(i).gameObject);
        }
    }

    private void OnCardClicked(Card card)
    {
        selectedCard.Add(card);
    }

    private void OnCardCancelClicked(Card card)
    {
        Debug.Log("CancleClick" + card.cardName);
        selectedCard.Remove(card);
    }

    private void OnDisable()
    {
        OnClickEvent -= OnCardClicked;
        OnCancelClickEvent -= OnCardCancelClicked;
    }
}
=======
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardTypeEnum
{
    Single,
    Pair,
    Third,
    Bomb,
    ThreePlusOne,
    Sequence,
    SequencePair,
}
public class CardManager : SingletonMonoBehaviour<CardManager>
{
    /// <summary>
    /// 被选中的卡片列表
    /// </summary>
    public List<Card> selectedCard = new List<Card>();
    /// <summary>
    /// 卡片被选中事件监听
    /// </summary>
    public Action<Card> OnClickEvent;
    /// <summary>
    /// 卡片被取消选中事件监听
    /// </summary>
    public Action<Card> OnCancelClickEvent;
    /// <summary>
    /// 出牌按钮
    /// </summary>
    [SerializeField] private Button mPlayCardButton;
    /// <summary>
    /// 提示按钮
    /// </summary>
    [SerializeField] private Button mPromptButton;
    /// <summary>
    /// pass按钮
    /// </summary>
    [SerializeField] private Button mPassButton;
    /// <summary>
    /// 出牌的区域
    /// </summary>
    [SerializeField] private Transform mTableObj;
    /// <summary>
    /// 操作提示语句
    /// </summary>
    [SerializeField] private Text mPromptText;



    private void OnEnable()
    {
        mPlayCardButton.onClick.AddListener(PlayCard);


        OnClickEvent += OnCardClicked;
        OnCancelClickEvent += OnCardCancelClicked;
    }

    /// <summary>
    /// 出牌
    /// </summary>
    private void PlayCard()
    {
        bool isPlay = false;
        int[] numbers = new int[selectedCard.Count];
        for (int i = 0; i < selectedCard.Count; i++)
        {
            numbers[i] = int.Parse(selectedCard[i].cardName.Split('-')[1]);
        }
        //判断是否符合出牌逻辑
        switch (numbers.Length)
        {
            case 2:
                if (numbers[0] == numbers[1])
                {
                    isPlay = true;
                }
                break;
            case 3:
                if (numbers[0] == numbers[1] && numbers[1] == numbers[2] 
                                             && numbers[0] == numbers[2])
                {
                    isPlay = true;
                }
                break;
            case 4:
                if (IsThreePlusOne(numbers))
                {
                    isPlay = true;
                }
                if (numbers[0] == numbers[1] && numbers[1] == numbers[2] 
                                             && numbers[2] == numbers[3] 
                                             && numbers[0] == numbers[3])
                {
                    isPlay = true;
                }
                
                break;
            default:
                
                if (IsSequence(numbers))
                {
                    isPlay = true;
                }
                if (IsSequencePair(numbers))
                {
                    isPlay = true;
                }
                break;
        }






        if (!isPlay) return;
        ClearTable();
        Card[] sortSelected = new Card[selectedCard.Count];
        for (int i = 0; i < selectedCard.Count; i++)
        {
            sortSelected[i] = selectedCard[i];
        }
        Array.Sort(sortSelected, (a, b) =>
        {
            var numberA = int.Parse(a.cardName.Split('-')[1]);
            var numberB = int.Parse(b.cardName.Split('-')[1]);
            if (numberA > numberB)
                return -1;
            if (numberA < numberB)
                return 1;
            return 0;
        });
        for (int i = 0; i < sortSelected.Length; i++)
            Main.Instance.SpawnCard(sortSelected[i].cardName, mTableObj,false);
        for (int i = 0; i < selectedCard.Count; i++)
            Destroy(selectedCard[i].gameObject);
        selectedCard.Clear();
    }

    /// <summary>
    /// 判断是否是顺子
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsSequence(int[] numbers)
    {
        if (numbers.Length < 5 || numbers.Length > 12) return false;
        //判断是否有不符合顺子的单牌
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > 14)
            {
                return false;
            }
        }
        Array.Sort(numbers);
        for (int i = 0; i < numbers.Length - 1; i++)
        {
            if (numbers[i] + 1 != numbers[i + 1])
            {
                return false;
            }
        }
        return true;
    }
    /// <summary>
    /// 判断是否是连对
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsSequencePair(int[] numbers)
    {
        if (numbers.Length < 6 || numbers.Length > 24) return false;
        //判断是否有不符合顺子的单牌
        for (int i = 0; i < numbers.Length; i++)
        {
            if (numbers[i] > 14)
            {
                return false;
            }
        }
        Array.Sort(numbers);
        for (int i = 0; i < numbers.Length - 2; i += 2)
        {
            if (numbers[i] != numbers[i + 1] || numbers[i] + 1 != numbers[i + 2])
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 判断是否三带一
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsThreePlusOne(int[] numbers)
    {
        if (numbers.Length != 4) return false;
        Array.Sort(numbers);
        if (numbers[0] == numbers[3]) return false;
        //判断带一是否大于三个的
        if (numbers[3] > numbers[0] && numbers[0] == numbers[1])
        {
            for (int i = 0; i < numbers.Length - 2; i ++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        if (numbers[3] > numbers[0] && numbers[0] != numbers[1])
        {
            for (int i = 1; i < numbers.Length - 1; i++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        if (numbers[0] > numbers[3])
        {
            for (int i = 1; i < numbers.Length - 1; i ++)
            {
                if (numbers[i] != numbers[i + 1])
                {
                    return false;
                }
            }
        }
        return true;
    }

    /// <summary>
    /// 清空桌面
    /// </summary>
    public void ClearTable()
    {

        for (int i = 0; i < mTableObj.childCount; i++)
        {
            Destroy(mTableObj.GetChild(i).gameObject);
        }
    }

    private void OnCardClicked(Card card)
    {
        selectedCard.Add(card);
    }

    private void OnCardCancelClicked(Card card)
    {
        Debug.Log("CancleClick" + card.cardName);
        selectedCard.Remove(card);
    }

    private void OnDisable()
    {
        OnClickEvent -= OnCardClicked;
        OnCancelClickEvent -= OnCardCancelClicked;
    }
}
>>>>>>> fa1842a525d3b9d639306928e3905e7d24fbfd66
