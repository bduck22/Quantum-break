using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapSelectorUI : MonoBehaviour
{
    UIWindow window;

    public TextMeshProUGUI Title;

    public TextMeshProUGUI MapName1;
    public TextMeshProUGUI MapName2;

    public Transform StarParent1;
    public Image[] Stars1 = new Image[5];

    public Transform StarParent2;
    public Image[] Stars2 = new Image[5];

    public Sprite StarTrue;
    public Sprite StarFalse;

    public MapData CurrentMap;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Opened += ReFresh;
    }

    public void ReFresh()
    {
        
    }
}
