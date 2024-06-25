using UnityEngine;
using XLua;


public class LuaTest : MonoBehaviour
{
    private LuaEnv mLuaEnv = new LuaEnv();
    // Start is called before the first frame update
    void Start()
    {
        mLuaEnv.DoString("require'main'");
        
    }


    // Update is called once per frame
    void Update()
    {

    }
}
