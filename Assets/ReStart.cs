using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReStart : MonoBehaviour
{

    void Start()
      {
         //获取按钮游戏对象
         GameObject btnObj = GameObject.Find("Canvas/Button");
         //获取按钮脚本组件
         Button btn = (Button)btnObj.GetComponent<Button>();
         //添加点击侦听
         btn.onClick.AddListener(onClick);
     }
 
    
     void onClick()
     {

        Application.LoadLevel("gameScene");
        Director.init();
     }
}
