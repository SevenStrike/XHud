using SevenStrikeModules.XHud;
using UnityEngine;

public class add : MonoBehaviour
{
    public XHud_Module_Element element;
    public int LastID;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            // 实例化预制体
            XHud_Module_Element ele = Instantiate(element);
            LastID = XHud_Manager.Instance.hm_ScreenElement_Create(ele, "Fox", XHud_Manager.Instance.CreateArgs_Default, true, Vector3.zero, Vector3.one, Vector2.one * 100).ID;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            XHud_Manager.Instance.hm_HudElement_RecycleAt(LastID, XHud_Manager.Instance.RecycleArgs_Default);
        }
    }
}
