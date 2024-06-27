<<<<<<< HEAD
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class Main : SingletonMonoBehaviour<Main>
{
    /// <summary>
    /// 发牌按钮
    /// </summary>
    [SerializeField] private Button mDealButton;
    /// <summary>
    /// 整理牌按钮
    /// </summary>
    [SerializeField] private Button mFromCardlButton;
    /// <summary>
    /// 卡牌预制体
    /// </summary>
    [SerializeField] private GameObject mCardObj;
    /// <summary>
    /// 自身卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mCardParentObj;
    /// <summary>
    /// 左边玩家卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mLeftPlayerCadParentObj;
    /// <summary>
    /// 右边玩家卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mRightPlayerCardParentObj;
    /// <summary>
    /// 最后的底牌卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mSurplusCardParentObj;
    /// <summary>
    /// 一副牌
    /// </summary>
    private List<string> mDeck = new List<string>();
    /// <summary>
    /// 每个人手上的牌 从自身顺时针旋转代号为 1,2,3
    /// </summary>
    private Dictionary<int, List<string>> mSingleDeck = new Dictionary<int, List<string>>();
    Queue<Action> mDealQueue = new Queue<Action>();
    
    void Start()
    {
        mDealButton.onClick.AddListener(DealCard);
        mFromCardlButton.onClick.AddListener(FormCard);
        DealCard();
    }

    
    void Update()
    {
        Debug.Log(mDealQueue.Count);
        if (mDealQueue.Count == 17)
        {
            StartCoroutine(ExecuteDealAction());
        }
    }

    /// <summary>
    /// 执行发牌延迟
    /// </summary>
    /// <returns></returns>
    private IEnumerator ExecuteDealAction()
    {
        while (mDealQueue.Count > 0)
        {
            
            lock (mDealQueue)
            {
                
                var t = mDealQueue.Dequeue();
                t();
                SpawnCardBack(mLeftPlayerCadParentObj);
                SpawnCardBack(mRightPlayerCardParentObj);
            }
            yield return new WaitForSeconds(0.3f);
        }
        //以下用来执行发牌结束逻辑
            FormCard();//整理牌
            SurplusCard();//产生底牌
        
    }

    /// <summary>
    /// 底牌处理
    /// </summary>
    private void SurplusCard()
    {
        
        for (int i = 0; i < mDeck.Count; i++)
        {
            SpawnCard(mDeck[i], mSurplusCardParentObj,false);
        }
        mDeck.Clear();
    }

    /// <summary>
    /// 牌堆初始化
    /// </summary>
    private void DeckInit()
    {
        mSingleDeck[1] = new List<string>();
        mSingleDeck[2] = new List<string>();
        mSingleDeck[3] = new List<string>();
        CardLogicManager.Instance.ClearTable();
        //清空自身区域
        for (int i = 0; i < mCardParentObj.childCount; i++)
        {
            Destroy(mCardParentObj.GetChild(i).gameObject);
        }
        //清空底牌
        for (int i = 0; i < mSurplusCardParentObj.childCount; i++)
        {
            Destroy(mSurplusCardParentObj.GetChild(i).gameObject);
        }
        //用来指示牌的花色 1-梅花M 2-方片F 3-红桃R 4-黑桃B
        int index = 1;
        //添加大小王
        mDeck.Add("J-17");//大王
        mDeck.Add("L-16");//小王
                             //添加其他牌
        for (int i = 1; i < 5; i++)
        {
            index = i;
            for (int j = 1; j < 14; j++)
            {
                switch (index)
                {
                    case 1:
                        if (j == 1)
                            mDeck.Add("M" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("M" + "-" + 15);
                        else
                            mDeck.Add("M" + "-" + j);
                        break;
                    case 2:
                        if (j == 1)
                            mDeck.Add("F" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("F" + "-" + 15);
                        else
                            mDeck.Add("F" + "-" + j);
                        break;
                    case 3:
                        if (j == 1)
                            mDeck.Add("R" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("R" + "-" + 15);
                        else
                            mDeck.Add("R" + "-" + j);
                        break;
                    case 4:
                        if (j == 1)
                            mDeck.Add("B" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("B" + "-" + 15);
                        else
                            mDeck.Add("B" + "-" + j);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 整理自己的牌
    /// </summary>
    public void FormCard()
    {
        //播放洗牌动画
        mCardParentObj.GetComponent<Animator>().Play("FormAction");

        //清空自身区域
        for (int i = 0; i < mCardParentObj.childCount; i++)
        {
            Destroy(mCardParentObj.GetChild(i).gameObject);
        }
        CardLogicManager.Instance.selectedCard.Clear();
        //1-15 15>14>2>1>13
        //获取当前自身的卡牌
        string[] cardNamber = new string[mCardParentObj.childCount]; 
        for (int i = 0; i < mCardParentObj.childCount; i++)
            cardNamber[i] = mCardParentObj.GetChild(i).GetComponent<Card>().cardName;
        //1 大于 0 等于 -1 小于 前大后
        Array.Sort(cardNamber, (a, b) =>
        {
            var numberA = int.Parse(a.Split('-')[1]);
            var numberB = int.Parse(b.Split('-')[1]);
            if (numberA > numberB)
                return 1;
            if (numberA < numberB)
                return -1;
            return 0;
        });
        Array.Reverse(cardNamber);
        for (int i = 0; i < cardNamber.Length; i++)
        {
            SpawnCard(cardNamber[i], mCardParentObj,true);
        }
    }

    /// <summary>
    /// 生成其他玩家牌
    /// </summary>
    /// <param name="cardParent"></param>
    public void SpawnCardBack(Transform cardParent)
    {
        var obj = Instantiate(mCardObj, cardParent);
        //同步加载数据
        ResManager.Instance.Load<SpriteAtlas>("SpriteAtlas", spriteatlas =>
        {
            obj.GetComponent<Image>().sprite = spriteatlas.GetSprite("CardBack");
        }, false);
    }

    /// <summary>
    /// 发牌
    /// </summary>
    private void DealCard()
    {
        DeckInit();
        //循环发牌
        int index = 1;
        //随机数控制
        int random;

        while (mDeck.Count > 3)
        {
            random = Random.Range(0, mDeck.Count);
            string card = mDeck[random];
            mDeck.RemoveAt(random);
            //按顺序放入
            mSingleDeck[index].Add(card);


            switch (index)
            {
                case 1:
                    mDealQueue.Enqueue(() =>
                    {
                        SpawnCard(card, mCardParentObj, true);
                    });
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            index++;
            if (index > 3)
                index = 1;
        }
    }

    /// <summary>
    /// 生成对应卡牌
    /// </summary>
    /// <param name="cardName">卡牌名字</param>
    /// <param name="cardParent">卡牌挂载位置</param>
    /// /// <param name="interaction">生成的卡牌是否可交互</param>
    public void SpawnCard(string cardName,Transform cardParent,bool interaction)
    {
        var obj = Instantiate(mCardObj, cardParent);
        obj.GetComponent<Card>().cardName = cardName;
        if (interaction)
            obj.GetComponent<Card>().interaction = true;

        //同步加载数据
        ResManager.Instance.Load<SpriteAtlas>("SpriteAtlas", spriteatlas =>
        {
            obj.GetComponent<Image>().sprite = spriteatlas.GetSprite(cardName);
        },false);
    }
}
=======
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;



public class Main : SingletonMonoBehaviour<Main>
{
    /// <summary>
    /// 发牌按钮
    /// </summary>
    [SerializeField] private Button mDealButton;
    /// <summary>
    /// 整理牌按钮
    /// </summary>
    [SerializeField] private Button mFromCardlButton;
    /// <summary>
    /// 卡牌预制体
    /// </summary>
    [SerializeField] private GameObject mCardObj;
    /// <summary>
    /// 自身卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mCardParentObj;
    /// <summary>
    /// 最后的底牌卡牌的挂载物体
    /// </summary>
    [SerializeField] private Transform mSurplusCardParentObj;
    /// <summary>
    /// 一副牌
    /// </summary>
    private List<string> mDeck = new List<string>();
    /// <summary>
    /// 每个人手上的牌 从自身顺时针旋转代号为 1,2,3
    /// </summary>
    private Dictionary<int, List<string>> mSingleDeck = new Dictionary<int, List<string>>();

    void Start()
    {
        DealCard();
        mDealButton.onClick.AddListener(DealCard);
        mFromCardlButton.onClick.AddListener(FormCard);
    }

    /// <summary>
    /// 底牌处理
    /// </summary>
    private void SurplusCard()
    {
        
        for (int i = 0; i < mDeck.Count; i++)
        {
            SpawnCard(mDeck[i], mSurplusCardParentObj,false);
        }
        mDeck.Clear();
    }

    /// <summary>
    /// 牌堆初始化
    /// </summary>
    private void DeckInit()
    {
        mSingleDeck[1] = new List<string>();
        mSingleDeck[2] = new List<string>();
        mSingleDeck[3] = new List<string>();
        CardManager.Instance.ClearTable();
        //清空自身区域
        for (int i = 0; i < mCardParentObj.childCount; i++)
        {
            Destroy(mCardParentObj.GetChild(i).gameObject);
        }
        //清空底牌
        for (int i = 0; i < mSurplusCardParentObj.childCount; i++)
        {
            Destroy(mSurplusCardParentObj.GetChild(i).gameObject);
        }
        //用来指示牌的花色 1-梅花M 2-方片F 3-红桃R 4-黑桃B
        int index = 1;
        //添加大小王
        mDeck.Add("J-17");//大王
        mDeck.Add("L-16");//小王
                             //添加其他牌
        for (int i = 1; i < 5; i++)
        {
            index = i;
            for (int j = 1; j < 14; j++)
            {
                switch (index)
                {
                    case 1:
                        if (j == 1)
                            mDeck.Add("M" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("M" + "-" + 15);
                        else
                            mDeck.Add("M" + "-" + j);
                        break;
                    case 2:
                        if (j == 1)
                            mDeck.Add("F" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("F" + "-" + 15);
                        else
                            mDeck.Add("F" + "-" + j);
                        break;
                    case 3:
                        if (j == 1)
                            mDeck.Add("R" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("R" + "-" + 15);
                        else
                            mDeck.Add("R" + "-" + j);
                        break;
                    case 4:
                        if (j == 1)
                            mDeck.Add("B" + "-" + 14);
                        else if (j == 2)
                            mDeck.Add("B" + "-" + 15);
                        else
                            mDeck.Add("B" + "-" + j);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// 整理自己的牌
    /// </summary>
    public void FormCard()
    {
        //清空自身区域
        for (int i = 0; i < mCardParentObj.childCount; i++)
        {
            Destroy(mCardParentObj.GetChild(i).gameObject);
        }
        CardManager.Instance.selectedCard.Clear();
        //1-15 15>14>2>1>13
        //获取当前自身的卡牌
        string[] cardNamber = new string[mCardParentObj.childCount]; 
        for (int i = 0; i < mCardParentObj.childCount; i++)
            cardNamber[i] = mCardParentObj.GetChild(i).GetComponent<Card>().cardName;
        //1 大于 0 等于 -1 小于 前大后
        Array.Sort(cardNamber, (a, b) =>
        {
            var numberA = int.Parse(a.Split('-')[1]);
            var numberB = int.Parse(b.Split('-')[1]);
            if (numberA > numberB)
                return 1;
            if (numberA < numberB)
                return -1;
            return 0;
        });
        Array.Reverse(cardNamber);
        for (int i = 0; i < cardNamber.Length; i++)
        {
            SpawnCard(cardNamber[i], mCardParentObj,true);
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    private void DealCard()
    {
        DeckInit();
        //循环发牌
        int index = 1;
        //随机数控制
        int random;

        while (mDeck.Count > 3)
        {
            random = Random.Range(0, mDeck.Count);
            string card = mDeck[random];
            mDeck.RemoveAt(random);
            //按顺序放入
            mSingleDeck[index].Add(card);
            switch (index)
            {
                case 1:
                    SpawnCard(card, mCardParentObj,true);
                    break;
                case 2:
                    break;
                case 3:
                    break;
            }

            index++;
            if (index > 3)
                index = 1;
        }

        SurplusCard();
    }


    /// <summary>
    /// 生成对应卡牌
    /// </summary>
    /// <param name="card">卡牌名字</param>
    /// <param name="cardParent">卡牌挂载位置</param>
    /// /// <param name="Interaction">生成的卡牌是否可交互</param>
    public void SpawnCard(string cardName,Transform cardParent,bool Interaction)
    {
        var obj = Instantiate(mCardObj, cardParent);
        obj.GetComponent<Card>().cardName = cardName;
        if (Interaction)
            obj.GetComponent<Card>().mInteraction = true;
        var cardSplit = cardName.Split("-");
        var cardEnum = cardSplit[0];
        var number = int.Parse(cardSplit[1]);
        //拼接卡片名字
        string cardPlus = "";

        switch (number)
        {
            case 14:
                cardPlus += "A";
                break;
            case 15:
                cardPlus += "2";
                break;
            case 11:
                cardPlus += "J";
                break;
            case 12:
                cardPlus += "Q";
                break;
            case 13:
                cardPlus += "K";
                break;
            default:
                cardPlus += number;
                break;
        }

        switch (cardEnum)
        {
            case "M":
                cardPlus += "\n" + "♣";
                obj.GetComponentInChildren<Text>().color = Color.black;
                break;
            case "F":
                cardPlus += "\n" + "♦";
                obj.GetComponentInChildren<Text>().color = Color.red;
                break;
            case "R":
                cardPlus += "\n" + "♥";
                obj.GetComponentInChildren<Text>().color = Color.red;
                break;
            case "B":
                cardPlus += "\n" + "♠";
                obj.GetComponentInChildren<Text>().color = Color.black;
                break;
            case "J":
                cardPlus = "J" + "\n" + "O" + "\n" + "K" + "\n" + "大";
                obj.GetComponentInChildren<Text>().color = Color.red;
                break;
            case "L":
                cardPlus = "J" + "\n" + "O" + "\n" + "K" + "\n" + "小";
                obj.GetComponentInChildren<Text>().color = Color.black;
                break;
        }

        obj.GetComponentInChildren<Text>().text = cardPlus;


    }
}
>>>>>>> fa1842a525d3b9d639306928e3905e7d24fbfd66
