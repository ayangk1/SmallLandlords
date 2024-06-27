using System;
using System.Collections;
using UnityEngine;

public class ResManager : SingletonMonoBehaviour<ResManager>
{
    /// <summary>
    /// 加载资源
    /// </summary>
    /// <typeparam name="T">加载资源的类型</typeparam>
    /// <param name="path">资源路径</param>
    /// <param name="isover">回调函数</param>
    /// /// <param name="asyn">是否异步加载</param>
    /// <returns></returns>
    public void Load<T>(string path, Action<T> isover, bool asyn) where T : UnityEngine.Object
    {
        if (asyn)
        {
            ResourceRequest request = Resources.LoadAsync<T>(path);

            request.completed += over =>
            {
                if (request.asset != null)
                {
                    T obj = request.asset as T;
                    isover?.Invoke(obj);
                    
                }
                else
                {
                    Debug.LogError($"加载失败，{path}");
                }
            };
        }
        else
        {
            T obj = Resources.Load<T>(path);
            isover?.Invoke(obj);
        }
    }

    /// <summary>
    /// 卸载资源
    /// </summary>
    /// <param name="obj"></param>
    public void UnloadSource<T>(T obj)
    {
        
        if (obj is GameObject)
        {
            Destroy(obj as GameObject);
        }
        else
        {
            Resources.UnloadAsset(obj as UnityEngine.Object);
        }
    }
}