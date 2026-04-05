using SevenStrikeModules.XHud;
using UnityEngine;

public class add : MonoBehaviour
{
    public XHud_Module_Element element;
    public bool InitiateMode;
    public bool WorldMode;
    public int id;
    public Vector2 size;
    public Vector3 scale;
    public Vector3 offset;
    public Vector3 pos;
    public float alpha;
    public Transform[] targets;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            Motion_Creator arg = XHud_Manager.Instance.CreateArgs_Default;

            XHudElementNode node = null;
            XHud_Module_Element ele = Instantiate(element);

            if (WorldMode)
            {
                // 实例化模式
                if (InitiateMode)
                {
                    node = XHud_Manager.Instance.hm_WorldElement_Create(ele)
                    .SetAnchored_World("Fox")
                    .SetAlpha(alpha)
                    .SetSize(size)
                    .SetPosition_World(GetRandomTarget())
                    .SetOffset(offset)
                    .SetScale(scale)
                    .Element_In(arg);
                }
                // 元素库模式
                else
                {
                    node = XHud_Manager.Instance.hm_WorldElement_Create("NewElementLibrary", "Image")
                   .SetAnchored_World("Fox")
                   .SetAlpha(alpha)
                   .SetSize(size)
                   .SetPosition_World(GetRandomTarget())
                   .SetOffset(offset)
                   .SetScale(scale)
                   .Element_In(arg);
                }
            }
            else
            {
                // 实例化模式
                if (InitiateMode)
                {
                    node = XHud_Manager.Instance.hm_ScreenElement_Create(ele)
                  .SetAnchored_Screen(arg.anchor, "Fox")
                  .SetAlpha(alpha)
                  .SetSize(size)
                  .SetOffset(offset)
                  .SetScale(scale)
                  .Element_In(arg);
                }
                // 元素库模式
                else
                {
                    node = XHud_Manager.Instance.hm_ScreenElement_Create("NewElementLibrary", "Image")
                   .SetAnchored_Screen(arg.anchor, "Fox")
                   .SetAlpha(alpha)
                   .SetSize(size)
                   .SetOffset(offset)
                   .SetScale(scale)
                   .Element_In(arg);
                }
            }


            id = node.ID;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            XHud_Manager.Instance.hm_HudElement_RecycleAt(id, XHud_Manager.Instance.RecycleArgs_Default);
        }
    }

    /// <summary>
    /// 获取随机位置目标
    /// </summary>
    /// <returns></returns>
    Transform GetRandomTarget()
    {
        int ran = Random.Range(0, targets.Length);
        return targets[ran];
    }
}
