using SevenStrikeModules.XHud;
using UnityEngine;

public class sss : MonoBehaviour
{
    public XHud_Module_Primitive_Tween tween;
    public int id;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            //TweenNode node = tween.TweenNode_GetByID(id);
            tween.Tween_PlayAll_Forced();
            //Debug.Log(JsonUtility.ToJson(node));

            //tween.Tweener_Play(node,);
        }
    }
}
