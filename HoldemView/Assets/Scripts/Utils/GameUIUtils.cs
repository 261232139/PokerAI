using UnityEngine;
using UnityEngine.UI;

public static class GameUIUtils
{
    public static void AddGuideCanvas(GameObject obj, bool raycaster = true)
    {
        Canvas c = obj.AddComponent<Canvas>();
        c.overrideSorting = true;
        c.sortingOrder = 4;

        obj.SetLayer(LayerConst.Default);

        if (raycaster)
        {
            obj.AddComponent<GraphicRaycaster>();
        }
    }

    public static void RemoveGuideCanvas(GameObject obj)
    {
        GraphicRaycaster raycaster = obj.GetComponent<GraphicRaycaster>();
        if (raycaster != null)
        {
            UnityEngine.Object.Destroy(raycaster);
        }

        obj.SetLayer(LayerConst.UI);

        Canvas c = obj.GetComponent<Canvas>();
        if (c != null)
        {
            UnityEngine.Object.Destroy(c);
        }
    }
}
