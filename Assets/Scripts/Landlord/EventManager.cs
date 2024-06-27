<<<<<<< HEAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonMonoBehaviour<EventManager>
{
    public delegate void OnClickCardHandler(string cardName);

    private event OnClickCardHandler OnClickEvent;

    public void AddListener(OnClickCardHandler clickHandler)
    {
        OnClickEvent += clickHandler;
    }
    public void RemoveListener(OnClickCardHandler clickHandler)
    {
        OnClickEvent -= clickHandler;
    }
    public void InvokeListener(string cardName)
    {
        OnClickEvent?.Invoke(cardName);
    }
}
=======
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : SingletonMonoBehaviour<EventManager>
{
    public delegate void OnClickCardHandler(string cardName);

    private event OnClickCardHandler OnClickEvent;

    public void AddListener(OnClickCardHandler clickHandler)
    {
        OnClickEvent += clickHandler;
    }
    public void RemoveListener(OnClickCardHandler clickHandler)
    {
        OnClickEvent -= clickHandler;
    }
    public void InvokeListener(string cardName)
    {
        OnClickEvent?.Invoke(cardName);
    }
}
>>>>>>> fa1842a525d3b9d639306928e3905e7d24fbfd66
