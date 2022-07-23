using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipSystem : MonoBehaviour
{
    public static ToolTipSystem current;
    public ToolTip toolTip;
    public ToolTip toolTipTwo;
    private void Start()
    {
        Hide();
    }
    private void Awake()
    {
        current = this;
       
    }
    public static void Show(string content, string header = "")
    {
        current.toolTip.SetText(content, header);
        current.toolTip.gameObject.SetActive(true);

        current.toolTipTwo.SetText(content, header);
        current.toolTipTwo.gameObject.SetActive(true);
    }
    public static void Hide()
    {
        current.toolTip.gameObject.SetActive(false);

        current.toolTipTwo.gameObject.SetActive(false);
    }
}
