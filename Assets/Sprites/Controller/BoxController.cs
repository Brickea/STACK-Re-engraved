using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxController : MonoBehaviour
{
    //控制最新的box
    private Box newestBox;
    //预置资源
    private GameObject prefab;
    //旧盒子
    private GameObject oldBox;
    //速度
    private float speed = 8;
    //private static bool direction = false;

    //盒子控制器构造函数
    public BoxController(GameObject prefab)
    {
        this.prefab = prefab;
        this.oldBox = this.prefab;
    }

    //盒子控制器构造函数
    public BoxController(GameObject prefab, GameObject baseBox)
    {
        this.prefab = prefab;
        this.oldBox = baseBox;
    }

    //生成盒子
    public Box GenerateNewBox(GameObject moveReference)
    {
        //传入预置资源和移动模式
        this.newestBox = new Box(this.prefab, GetBox(), moveReference);
        oldBox = newestBox.GetBox();
        return this.newestBox;
    }
    //生成盒子
    public Box GenerateNewBox(Vector3 newScale, GameObject moveReference)
    {
        //传入预置资源和移动模式
        this.newestBox = new Box(this.prefab, newScale, GetBox(), moveReference);
        oldBox = newestBox.GetBox();
        return this.newestBox;
    }

    //得到旧盒子
    public GameObject GetBox()
    {
        return oldBox;
    }

    //使盒子动起来
    public void MoveOn()
    {
        this.newestBox.MoveBackForth();
    }

    //停止盒子移动
    public void StopAndFall()
    {
        this.newestBox.SetSpeed(0);
        this.newestBox.SetGravity();
    }
    //速度自增
    public void SpeedIncrease()
    {
        if (speed <= 16)
        {
            speed = 8 + 8 * Box.index / 100f;
        }
            this.newestBox.SetSpeed(speed);
    }

    //冻住盒子
    public void Freeze()
    {
        Box.SetFrezze(GameObject.Find("Box" + (Box.index - 2)) as GameObject);
    }

    //切割盒子
    public Vector3 SplitBox()
    {
        //获取新旧盒子对象
        GameObject newestBox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        GameObject oldBox = GameObject.Find("Box" + (Box.index - 2)) as GameObject;
        //传入函数计算切割盒子的大小和位置
        float[] boxPosition = calcPosition(newestBox, oldBox, this.newestBox.GetDirection());
        float fallBoxCenter = boxPosition[0], fallBoxScale = boxPosition[1], stayBoxCenter = boxPosition[2], stayBoxScale = boxPosition[3];
        Vector3 newScale;
        //根据移动方向调整旧盒子大小，并且移动到指定位置上
        if (this.newestBox.GetDirection())
        {
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(stayBoxCenter, newestBox.transform.position.y, oldBox.transform.position.z), Quaternion.identity);
            //调整大小
            newScale = new Vector3(stayBoxScale, newestBox.transform.localScale.y, newestBox.transform.localScale.z);
            newestBox.transform.localScale = newScale;
            this.newestBox.GenerateFallBox(
                new Vector3(fallBoxCenter, newestBox.transform.position.y, oldBox.transform.position.z),
                new Vector3(fallBoxScale, newestBox.transform.localScale.y, newestBox.transform.localScale.z),
                prefab);
        }
        else
        {
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(oldBox.transform.position.x, newestBox.transform.position.y, stayBoxCenter), Quaternion.identity);
            //调整大小
            newScale = new Vector3(newestBox.transform.localScale.x, newestBox.transform.localScale.y, stayBoxScale);
            newestBox.transform.localScale = newScale;
            this.newestBox.GenerateFallBox(
                new Vector3(oldBox.transform.position.x, newestBox.transform.position.y, fallBoxCenter),
                new Vector3(newestBox.transform.localScale.x, newestBox.transform.localScale.y, fallBoxScale),
                prefab);
        }

        //新盒子和旧盒子中心距离判断，决定是否切割
        //切割

        return newScale;
    }

    //切割盒子
    public Vector3 SplitBox(Vector3 force)
    {
        //获取新旧盒子对象
        GameObject newestBox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        GameObject oldBox = GameObject.Find("Box" + (Box.index - 2)) as GameObject;
        //传入函数计算切割盒子的大小和位置
        float[] boxPosition = calcPosition(newestBox, oldBox, this.newestBox.GetDirection());
        float fallBoxCenter = boxPosition[0], fallBoxScale = boxPosition[1], stayBoxCenter = boxPosition[2], stayBoxScale = boxPosition[3];
        Vector3 newScale;
        //根据移动方向调整旧盒子大小，并且移动到指定位置上
        if (this.newestBox.GetDirection())
        {
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(stayBoxCenter, newestBox.transform.position.y, oldBox.transform.position.z), Quaternion.identity);
            //调整大小
            newScale = new Vector3(stayBoxScale, newestBox.transform.localScale.y, newestBox.transform.localScale.z);
            newestBox.transform.localScale = newScale;
            GameObject temp = this.newestBox.GenerateFallBox(
                new Vector3(fallBoxCenter, newestBox.transform.position.y, oldBox.transform.position.z),
                new Vector3(fallBoxScale, newestBox.transform.localScale.y, newestBox.transform.localScale.z),
                prefab);

            //使下落物体更有感觉
            //    **********************************************
            //增加横向力
            temp.GetComponent<Rigidbody>().AddForce(force * 100);
            //增加旋转
            temp.transform.rotation = new Quaternion(force.x * 0.005f, 0, force.z * 0.005f, 1);
            //通过给下落物体一个666的力量来增加下落速度
            temp.GetComponent<Rigidbody>().AddForce(new Vector3(0, -666 + 333, 0));

            //    **********************************************
            //使下落物体更有感觉
        }
        else
        {
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(oldBox.transform.position.x, newestBox.transform.position.y, stayBoxCenter), Quaternion.identity);
            //调整大小
            newScale = new Vector3(newestBox.transform.localScale.x, newestBox.transform.localScale.y, stayBoxScale);
            newestBox.transform.localScale = newScale;
            GameObject temp = this.newestBox.GenerateFallBox(
                new Vector3(oldBox.transform.position.x, newestBox.transform.position.y, fallBoxCenter),
                new Vector3(newestBox.transform.localScale.x, newestBox.transform.localScale.y, fallBoxScale),
                prefab);

            //使下落物体更有感觉
            //    **********************************************
            //增加横向力
            temp.GetComponent<Rigidbody>().AddForce(force * 100);
            //增加旋转
            temp.transform.rotation = new Quaternion(force.x * 0.005f, 0, force.z * 0.005f, 1);
            //通过给下落物体一个666的力量来增加下落速度
            temp.GetComponent<Rigidbody>().AddForce(new Vector3(0, -666 + 333, 0));

            //    **********************************************
            //使下落物体更有感觉
        }

        //新盒子和旧盒子中心距离判断，决定是否切割
        //切割

        return newScale;
    }

    //完美下落
    public Vector3 PerfectFall()
    {
        //获取新旧盒子对象
        GameObject newestBox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        GameObject oldBox = GameObject.Find("Box" + (Box.index - 2)) as GameObject;
        //传入函数计算切割盒子的大小和位置
        float[] boxPosition = calcPosition(newestBox, oldBox, this.newestBox.GetDirection());
        float stayBoxCenter = 0, stayBoxScale = 0;
        Vector3 newScale;
        //根据移动方向调整旧盒子大小，并且移动到指定位置上
        if (this.newestBox.GetDirection())
        {
            //完美下落获取旧盒子的位置信息和大小
            stayBoxCenter = oldBox.transform.position.x;
            stayBoxScale = oldBox.transform.localScale.x;
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(stayBoxCenter, newestBox.transform.position.y, oldBox.transform.position.z), Quaternion.identity);
            //调整大小
            newScale = new Vector3(stayBoxScale, newestBox.transform.localScale.y, newestBox.transform.localScale.z);
            newestBox.transform.localScale = newScale;

            Director.reward++;
            if (Director.reward >= 7)
            {
                Vector3 scale = newestBox.transform.localScale;
                float scale_x = scale.x * 1.2f;
                if (scale_x > 6)
                {
                    scale_x = 6;
                }
                newestBox.transform.localScale = new Vector3(scale_x, scale.y, scale.z);
            }
        }
        else
        {
            //完美下落获取旧盒子的位置信息和大小
            stayBoxCenter = oldBox.transform.position.z;
            stayBoxScale = oldBox.transform.localScale.z;
            //调整位置
            newestBox.transform.SetPositionAndRotation(new Vector3(oldBox.transform.position.x, newestBox.transform.position.y, stayBoxCenter), Quaternion.identity);
            //调整大小
            newScale = new Vector3(newestBox.transform.localScale.x, newestBox.transform.localScale.y, stayBoxScale);
            newestBox.transform.localScale = newScale;

            Director.reward++;
            if (Director.reward >= 7)
            {
                Vector3 scale = newestBox.transform.localScale;
                float scale_z = scale.z * 1.2f;
                if (scale_z > 6)
                {
                    scale_z = 6;
                }
                newestBox.transform.localScale = new Vector3(scale.x, scale.y, scale_z);
            }
        }
        return newScale;
    }



    //传入direction确定是x轴还是z轴
    /*
     require:
        newBox 
        oldBox 
        direction:判断x轴z轴移动方向
     return: 
        0: fallBoxCenter
        1: fallBoxScale
        2: stayBoxCenter
        3: stayBoxScale
    */
    public static float[] calcPosition(GameObject newestBox, GameObject oldBox, bool direction)
    {
        float[] boxPosition = new float[4];
        float stayBoxCenter = 0, stayBoxXScale = 0, fallBoxCenter = 0, fallBoxScale = 0;
        if (direction)
        {
            float bias = newestBox.transform.position.x - oldBox.transform.position.x;
            if (bias > 0)
            {
                //x轴正方向切割
                //stay盒子
                //new的左边加上old的右边 除 2 得到中心
                stayBoxCenter = ((newestBox.transform.position.x - (newestBox.transform.localScale.x) / 2)
                    + (oldBox.transform.position.x + (oldBox.transform.localScale.x) / 2))
                    / 2;
                //old的右边减去new的左边 得到边长
                stayBoxXScale = (oldBox.transform.position.x + (oldBox.transform.localScale.x) / 2)
                    - (newestBox.transform.position.x - (newestBox.transform.localScale.x) / 2);

                //fall盒子
                //new的右边加上old的右边 除 2 得到中心
                fallBoxCenter = ((newestBox.transform.position.x + (newestBox.transform.localScale.x) / 2)
                     + (oldBox.transform.position.x + (oldBox.transform.localScale.x) / 2))
                     / 2;
                //new的右边减去old的右边得到边长
                fallBoxScale = (newestBox.transform.position.x + (newestBox.transform.localScale.x) / 2)
                    - (oldBox.transform.position.x + (oldBox.transform.localScale.x) / 2);
            }
            else
            {
                //x轴负方向切割
                //stay盒子
                //new的右边加上old的左边 除 2 得到中心
                stayBoxCenter = ((newestBox.transform.position.x + (newestBox.transform.localScale.x) / 2)
                    + (oldBox.transform.position.x - (oldBox.transform.localScale.x) / 2))
                    / 2;
                //new的右边减去old的左边 得到边长
                stayBoxXScale = (newestBox.transform.position.x + (newestBox.transform.localScale.x) / 2)
                    - (oldBox.transform.position.x - (oldBox.transform.localScale.x) / 2);

                //fall盒子
                //new的左边加上old的左边 除 2 得到中心
                fallBoxCenter = ((newestBox.transform.position.x - (newestBox.transform.localScale.x) / 2)
                     + (oldBox.transform.position.x - (oldBox.transform.localScale.x) / 2))
                     / 2;
                //old的左边减去new的左边得到边长
                fallBoxScale = (oldBox.transform.position.x - (oldBox.transform.localScale.x) / 2)
                    - (newestBox.transform.position.x - (newestBox.transform.localScale.x) / 2);
            }
        }
        else
        {
            float bias = newestBox.transform.position.z - oldBox.transform.position.z;
            if (bias > 0)
            {
                //x轴正方向切割
                //stay盒子
                //new的左边加上old的右边 除 2 得到中心
                stayBoxCenter = ((newestBox.transform.position.z - (newestBox.transform.localScale.z) / 2)
                    + (oldBox.transform.position.z + (oldBox.transform.localScale.z) / 2))
                    / 2;
                //old的右边减去new的左边 得到边长
                stayBoxXScale = (oldBox.transform.position.z + (oldBox.transform.localScale.z) / 2)
                    - (newestBox.transform.position.z - (newestBox.transform.localScale.z) / 2);

                //fall盒子
                //new的右边加上old的右边 除 2 得到中心
                fallBoxCenter = ((newestBox.transform.position.z + (newestBox.transform.localScale.z) / 2)
                     + (oldBox.transform.position.z + (oldBox.transform.localScale.z) / 2))
                     / 2;
                //new的右边减去old的右边得到边长
                fallBoxScale = (newestBox.transform.position.z + (newestBox.transform.localScale.z) / 2)
                    - (oldBox.transform.position.z + (oldBox.transform.localScale.z) / 2);
            }
            else
            {
                //x轴负方向切割
                //stay盒子
                //new的右边加上old的左边 除 2 得到中心
                stayBoxCenter = ((newestBox.transform.position.z + (newestBox.transform.localScale.z) / 2)
                    + (oldBox.transform.position.z - (oldBox.transform.localScale.z) / 2))
                    / 2;
                //new的右边减去old的左边 得到边长
                stayBoxXScale = (newestBox.transform.position.z + (newestBox.transform.localScale.z) / 2)
                    - (oldBox.transform.position.z - (oldBox.transform.localScale.z) / 2);

                //fall盒子
                //new的左边加上old的左边 除 2 得到中心
                fallBoxCenter = ((newestBox.transform.position.z - (newestBox.transform.localScale.z) / 2)
                     + (oldBox.transform.position.z - (oldBox.transform.localScale.z) / 2))
                     / 2;
                //old的左边减去new的左边得到边长
                fallBoxScale = (oldBox.transform.position.z - (oldBox.transform.localScale.z) / 2)
                    - (newestBox.transform.position.z - (newestBox.transform.localScale.z) / 2);
            }
        }
        boxPosition[0] = fallBoxCenter;
        boxPosition[1] = fallBoxScale;
        boxPosition[2] = stayBoxCenter;
        boxPosition[3] = stayBoxXScale;
        return boxPosition;
    }

}
