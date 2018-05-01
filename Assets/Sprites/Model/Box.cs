using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{

    private GameObject obj;
    private GameObject moveReference;   //参考系

    private GameObject oldBox;      //旧盒子
    private Vector3 newBoxPosition;     //新盒子位置
    private Transform[] reference;      //参考坐标数组，第二个第三个为需要的位置
    private bool direction = true;      //判断移动方向
    public float speed = 8;             //移动速度
    public static int index = 1;


    //构造函数，不包含旧方块
    public Box(GameObject prefab)
    {

        this.obj = GameObject.Instantiate(prefab, new Vector3(0, 9, 0), Quaternion.identity) as GameObject;
        obj.GetComponent<MeshRenderer>().material.color = Color.red;
        this.oldBox = this.obj;
    }

    //构造函数，包含旧方块，通过旧方块位置确定新方块的生成位置
    public Box(GameObject prefab, GameObject oldBox, GameObject moveReference)
    {
        //在旧盒子位置基础上向上增加一个厚度的高度生成新盒子
        this.oldBox = oldBox;
        newBoxPosition = oldBox.transform.position;
        newBoxPosition.y += (float)1.2; //1.2是一个盒子的厚度


        //生成当前移动参考的坐标
        this.moveReference = GameObject.Instantiate(moveReference, newBoxPosition, Quaternion.identity) as GameObject;

        //按照新位置生成新盒子
        reference = this.moveReference.GetComponentsInChildren<Transform>();    //获取新生成的移动参考位置

        //参考位置分别为第二个和第三个数组元素
        this.obj = GameObject.Instantiate(prefab, newBoxPosition, Quaternion.identity) as GameObject;
        this.obj.transform.name = "Box" + Box.index++;

    }

    public Box(GameObject prefab, Vector3 newScale, GameObject oldBox, GameObject moveReference)
    {

        //在旧盒子位置基础上向上增加一个厚度的高度生成新盒子
        this.oldBox = oldBox;
        newBoxPosition = oldBox.transform.position;
        newBoxPosition.y += (float)1.2; //1.2是一个盒子的厚度
        print(moveReference);

        //生成当前移动参考的坐标

        this.moveReference = GameObject.Instantiate(moveReference, newBoxPosition, Quaternion.identity) as GameObject;




        //按照新位置生成新盒子
        reference = this.moveReference.GetComponentsInChildren<Transform>();    //获取新生成的移动参考位置
        newBoxPosition = reference[1].transform.position;
        //参考位置分别为第二个和第三个数组元素
        this.obj = GameObject.Instantiate(prefab, newBoxPosition, Quaternion.identity) as GameObject;
        obj.transform.localScale = newScale;
        this.obj.transform.name = "Box" + Box.index++;

    }


    //方块来回移动函数
    public void MoveBackForth()
    {

        GameObject newbox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        if (direction)
        {
            newbox.transform.position = Vector3.MoveTowards(newbox.transform.position, reference[1].position, speed * Time.deltaTime);
            if (newbox.transform.position == reference[1].position)
            {
                direction = false;
            }
        }
        else
        {
            newbox.transform.position = Vector3.MoveTowards(newbox.transform.position, reference[2].position, speed * Time.deltaTime);
            if (newbox.transform.position == reference[2].position)
            {
                direction = true;
            }
        }

    }
    //设值方块移动速度
    public void SetSpeed(float tempSpeed)
    {
        speed = tempSpeed;
    }
    //设值方块重力
    public void SetGravity()
    {
        GameObject newbox = GameObject.Find("Box" + (Box.index - 1)) as GameObject;
        newbox.GetComponent<Rigidbody>().useGravity = true;
    }
    //获取当前盒子GameObject对象
    public GameObject GetBox()
    {
        return this.obj;
    }
    //使盒子固定
    public static void SetFrezze(GameObject box)
    {
        box.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY;
    }
    //创建下落盒子
    public GameObject GenerateFallBox(GameObject prefab)
    {
        GameObject temp = GameObject.Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        temp.SetActive(false);
        return temp;
    }
    //创建下落盒子
    public GameObject GenerateFallBox(Vector3 fallPosition, Vector3 fallScale, GameObject prefab)
    {
        GameObject temp = GameObject.Instantiate(prefab, fallPosition, Quaternion.identity) as GameObject;
        //temp.GetComponent<Rigidbody>().AddForce(new Vector3(0, 0, 50));
        //temp.transform.rotation = new Quaternion(0, 0, (float)-0.008, 1);
        temp.transform.localScale = fallScale;
        temp.SetActive(true);
        temp.GetComponent<Rigidbody>().useGravity = true;
        return temp;
    }
    //获得移动Z X方向
    public bool GetDirection()
    {
        if (this.moveReference.name.Equals("XMoveReference(Clone)"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
