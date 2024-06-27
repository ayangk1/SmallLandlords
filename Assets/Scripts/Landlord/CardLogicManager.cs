using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardTypeEnum
{
    Single,
    Pair,
    Triplet,
    Bomb,
    TripletPlusOne,
    TripletPlusPair,//三带一对
    Sequence,
    SequencePair,
    SequenceTriplet,//三连对
    SequenceTripletPlusOne,//飞机
    QuadplexSet,//四带二
    Rocket
}
public class CardLogicManager : SingletonMonoBehaviour<CardLogicManager>
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
            numbers[i] = int.Parse(selectedCard[i].cardName.Split('-')[1]);
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
                if (IsTripletPlusOne(numbers))
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
                
                if (IsSequenceTripletPlusOne(numbers))
                {
                    isPlay = true;
                }
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
    /// 判断是否是飞机
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsSequenceTripletPlusOne(int[] numbers)
    {
        if (numbers.Length < 8) return false;

        int number = JudgeSameNumber(numbers, 3);
        if (number == 0)
        {
            return false;
        }
        return true;
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
    /// 判断数组中有几个输入个数的相同的数，并且连着
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <param name="index">相同个数</param>
    /// <returns>返回相同数的个数</returns>
    public int JudgeSameNumber(int[] numbers,int index)
    {
        if (index < 1 || index > 4)
        {
            Debug.LogError("输入超出范围");
            return 0;
        }
        if (numbers.Length == 1 || index == 1) return 1;
        //排序
        Array.Sort(numbers);
        int number = 0;
        //暂时存储列表数据
        List<int> tempSameNumber = new List<int>();
        int temp = index - 1;
        for (int i = 0; i < numbers.Length - 1; i++)
        {
            if (numbers[i] == numbers[i + 1])
            {
                temp--;
            }
            else
            {
                temp = index - 1;
            }

            if (temp == 0)
            {
                tempSameNumber.Add(numbers[i]);
                temp = index - 1;
                number++;
            }
        }

        tempSameNumber.Sort((a, b) =>
        {
            if (a > b)
                return 1;
            return -1;
        });
        //如果不是连着的数
        for (int i = 0; i < tempSameNumber.Count - 1; i++)
        {
            if (tempSameNumber[i] + 1 != tempSameNumber[i + 1])
            {
                number = 0;
            }
        }

        return number;
    }

    /// <summary>
    /// 判断是否三带一
    /// </summary>
    /// <param name="numbers">输入数组</param>
    /// <returns></returns>
    private bool IsTripletPlusOne(int[] numbers)
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
