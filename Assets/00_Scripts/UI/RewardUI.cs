using IWantGoHome.ScreenEffects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RewardUI : MonoBehaviour
{
    UIWindow window;

    public TextMeshProUGUI Title;

    public Image MapPanel1;
    public Image MapPanel2;

    Outline PanelOutLine1;
    Outline PanelOutLine2;

    public Sprite FillStar;
    public Sprite UnFillStar;

    public Transform Star1;
    Image[] Stars1;

    public Transform Star2;
    Image[] Stars2;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;

    public Button RewardButton;

    public Transform ContinueButton;

    public bool FirstSelect;

    public bool NowSelect;

    private void Awake()
    {
        window = GetComponent<UIWindow>();
        window.Opened += Open;
        window.Closed += Close;
        window.Refreshed += Refresh;

        PanelOutLine1 = MapPanel1.GetComponent<Outline>();
        PanelOutLine2 = MapPanel2.GetComponent<Outline>();
        Stars1 = new Image[5];
        Stars2 = new Image[5];

        for (int i = 0; i < 5; i++)
        {
            Stars1[i] = Star1.GetChild(i).GetComponent<Image>();
            Stars2[i] = Star2.GetChild(i).GetComponent<Image>();
        }
    }

    void Open()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
    }

    void Close()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Refresh()
    {
        FirstSelect = false;

        Color setColor;
        MapPanel1.color = Color.black;

        MapPanel2.color = Color.black;

        setColor = PanelOutLine1.effectColor;
        setColor.a = 0;
        PanelOutLine1.effectColor = setColor;

        setColor = PanelOutLine2.effectColor;
        setColor.a = 0;
        PanelOutLine2.effectColor = setColor;

        MapData map1 = GameManager.Instance.Map1;
        MapData map2 = GameManager.Instance.Map2;

        text1.text = map1.Name + "\n\nWave " + map1.Wave;
        text2.text = map2.Name + "\n\nWave " + map2.Wave;

        text2.color = Color.black;
        text1.color = Color.black;

        for (int i = 1; i <= 5; i++)
        {
            Stars1[i - 1].color = Color.black;
            if (map1.Difficult >= i)
            {
                Stars1[i - 1].sprite = FillStar;
            }
            else
            {
                Stars1[i - 1].sprite = UnFillStar;
            }

            Stars2[i - 1].color = Color.black;
            if (map2.Difficult >= i)
            {
                Stars2[i - 1].sprite = FillStar;
            }
            else
            {
                Stars2[i - 1].sprite = UnFillStar;
            }
        }

        RewardButton.enabled = !GameManager.Instance.Cardget;

        RewardButton.GetComponent<CanvasGroup>().alpha = !GameManager.Instance.Cardget ? 1 : 0.2f;

        Title.text = GameManager.Instance.Cleared ?"코어 수복됨" :"코어 파괴됨";

        ContinueButton.gameObject.SetActive(false);
    }

    public void Seleted(bool One)
    {
        if (!FirstSelect)
        {
            ContinueButton.gameObject.SetActive(true);
            FirstSelect = true;
        }

        NowSelect = One;

        if (One)
        {
            Color setColor;
            ColorUtility.TryParseHtmlString("#7D00FF", out setColor);
            MapPanel1.color = setColor;

            MapPanel2.color = Color.black;

            setColor = PanelOutLine1.effectColor;
            setColor.a = 1;
            PanelOutLine1.effectColor = setColor;

            setColor = PanelOutLine2.effectColor;
            setColor.a = 0;
            PanelOutLine2.effectColor = setColor;

            text1.color = Color.white;
            text2.color = Color.black;

            for (int i = 1; i <= 5; i++)
            {
                Stars1[i - 1].color = Color.white;

                Stars2[i - 1].color = Color.black;
            }
        }
        else
        {
            Color setColor;
            ColorUtility.TryParseHtmlString("#7D00FF", out setColor);
            MapPanel2.color = setColor;

            MapPanel1.color = Color.black;

            setColor = PanelOutLine2.effectColor;
            setColor.a = 1;
            PanelOutLine2.effectColor = setColor;

            setColor = PanelOutLine1.effectColor;
            setColor.a = 0;
            PanelOutLine1.effectColor = setColor;

            text2.color = Color.white;
            text1.color = Color.black;

            for (int i = 1; i <= 5; i++)
            {
                Stars2[i - 1].color = Color.white;

                Stars1[i - 1].color = Color.black;
            }
        }
    }

    public void StartMap()
    {
        GameManager.Instance.SelectedMap = NowSelect ? GameManager.Instance.Map1 : GameManager.Instance.Map2;
        window.Close();
        GameManager.Instance.MapInit();
    }
}
