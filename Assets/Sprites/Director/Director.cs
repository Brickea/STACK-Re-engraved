using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{
    public GameObject prefabBox;
    public Camera mainCamera;

    public GameObject XMoveReference;
    public GameObject ZMoveReference;

    private BoxController boxController;

    private static bool direction = true;

    //0游戏结束
    //1游戏开始
    //2重新开始
    private static int status = 0;

    //奖励计数，7个的时候增长
    public static int reward = 0;

    //场景重载初始化
    public static void init()
    {
        direction = true;
        status = 1;
        Box.index = 1;
    }



    // Use this for initialization
    void Start()
    {
        boxController = new BoxController(prefabBox, GameObject.Find("Box0"));
        status = 0;

        //之后可以不用返回这个newBox，因为并不需要在这一层次直接控制newBox，而是通过BoxController来控制
        //在生成新盒子的时候也要指定盒子的移动方式
        boxController.GenerateNewBox(this.XMoveReference);
    }
    //分数UI更新
    void scoreUpdate()
    {
        GameObject scoreText = GameObject.Find("Canvas/Score");
        Text text = (Text)scoreText.GetComponent<Text>();
        text.text = "Score : " + (Box.index - 2);
    }
    //开始UI更新
    void startUpdate()
    {
        GameObject startText = GameObject.Find("Canvas/Start");
        Text text = (Text)startText.GetComponent<Text>();
        if (status == 1)
        {
            text.text = "";
        }
        else if (status == 0)
        {
            text.text = "Start";
        }
    }
    //失败UI更新
    void failUpdate()
    {
        GameObject failText = GameObject.Find("Canvas/Fail");
        Text text = (Text)failText.GetComponent<Text>();
        if (status == 1 || status ==0)
        {
            text.text = "";
        }
        else if (status == 2)
        {
            text.text = "Game Over!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        //菜单逻辑
        //*********************************************************************************
        startUpdate();
        failUpdate();
        //*********************************************************************************
        //菜单逻辑


        //选择移动模式让新生成的方块动起来
        boxController.MoveOn();
        

        //添加鼠标监听
        if (Input.GetMouseButtonDown(0))
        {
            if (status == 1)
            {
                //游戏逻辑
                GameLogic();
            }
            else if (status == 0)
            {
                status = 1;
            }
            else if (status == 2)
            {
                Application.LoadLevel("gameScene");
                init();
                status = 1;
            }
        }
        else
        {

        }
        //摄像头移动
        GameObject newBox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        if (mainCamera.transform.position.y < (23.4 + newBox.transform.position.y))
        {
            mainCamera.transform.position = Vector3.MoveTowards(mainCamera.transform.position, new Vector3(27, (float)16.11 + newBox.transform.position.y, (float)24.5), 5 * Time.deltaTime);
        }
       
    }

    private void GameLogic()
    {
        
        boxController.StopAndFall();
        GameObject newBox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        GameObject oldBox = GameObject.Find("Box" + (Box.index - 2)) as GameObject;
        print(oldBox);
        print(newBox);
        if (direction)
        {
            if (Mathf.Abs(newBox.transform.position.x - oldBox.transform.position.x) < (newBox.transform.localScale.x + oldBox.transform.localScale.x) / 2)
            {
                //完美下落判断，如果用户点击之后生成盒子位置在参考系方向上与旧盒子的位置差值小于旧盒子边长的10%，则判定为完美下落
                if (Mathf.Abs(newBox.transform.position.x - oldBox.transform.position.x) < oldBox.transform.localScale.x * 0.1)
                {
                    Vector3 newScale = boxController.PerfectFall();
                    print(oldBox);
                    print(newBox);
                    boxController.GenerateNewBox(newBox.transform.localScale, this.ZMoveReference);
                    boxController.Freeze();
                }
                else
                {
                    //进行切割操作并且传入下落旋转方向以及力量方向
                    Vector3 newScale = boxController.SplitBox(new Vector3(newBox.transform.position.x - oldBox.transform.position.x, 0, 0));
                    boxController.GenerateNewBox(newScale, this.ZMoveReference);
                    boxController.Freeze();
                    reward = 0;
                }
                boxController.SpeedIncrease();
            }
            else
            {
                print("You die!");
                status = 2;
            }
           
        }
        else
        {
            if (Mathf.Abs(newBox.transform.position.z - oldBox.transform.position.z) < (newBox.transform.localScale.z + oldBox.transform.localScale.z) / 2)
            {
                //完美下落判断，如果用户点击之后生成盒子位置在参考系方向上与旧盒子的位置差值小于旧盒子边长的10%，则判定为完美下落
                if (Mathf.Abs(newBox.transform.position.z - oldBox.transform.position.z) < oldBox.transform.localScale.z * 0.1)
                {
                    Vector3 newScale = boxController.PerfectFall();
                    print(oldBox);
                    print(newBox);

                    boxController.GenerateNewBox(newBox.transform.localScale, this.XMoveReference);
                    boxController.Freeze();
                }
                else
                {
                    //进行切割操作并且传入下落旋转方向以及力量方向
                    Vector3 newScale = boxController.SplitBox(new Vector3(0, 0, newBox.transform.position.z - oldBox.transform.position.z));
                    boxController.GenerateNewBox(newScale, this.XMoveReference);
                    boxController.Freeze();
                    reward = 0;
                }
                boxController.SpeedIncrease();

            }
            else
            {
                print("You die!");
                status = 2;
            }
        }
        direction = !direction;
        scoreUpdate();
    }
}
