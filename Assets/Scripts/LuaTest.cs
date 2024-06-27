<<<<<<< HEAD
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
=======
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
>>>>>>> fa1842a525d3b9d639306928e3905e7d24fbfd66
