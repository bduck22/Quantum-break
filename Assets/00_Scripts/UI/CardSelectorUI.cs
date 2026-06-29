using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardSelectorUI : MonoBehaviour
{
    public PlayerCardBase[] Cards;

    public Transform[] CardPanels;

    UIWindow window;

    private void Awake()
    {
        window = transform.parent.GetComponent<UIWindow>(); 
    }

    public void InitOpen()
    {
        Cards = new PlayerCardBase[3];

        for(int i=0;i<Cards.Length;i++)
        {
            Cards[i] = GameDataManager.Instance.GetRandomCardCollection();
            CardPanels[i].gameObject.SetActive(false);
        }
        gameObject.SetActive(true);
    }

    public void Show()
    {
        for(int i = 0; i < 3; i++)
        {
            CardPanels[i].GetChild(0).GetChild(0).GetComponent<Image>().sprite = Cards[i].Data.Icon;
            CardPanels[i].GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = Cards[i].Data.Name;

            string Description = Cards[i].Data.Description;
            Description = Description.Replace("{value}", Cards[i].Data.Value.ToString());

            CardPanels[i].GetChild(2).GetChild(0).GetComponent<TextMeshProUGUI>().text = Description;

            PlayerCardData data = GameManager.Instance.Inventory.PlayerCardInInventory[Cards[i].Type];

            CardPanels[i].GetChild(3).GetChild(0).GetComponent<TextMeshProUGUI>().text = $"보유 갯수 : {data.HavingCount}" + (data.MaxCount != 0 ? " / " + data.MaxCount :"");

            CardPanels[i].gameObject.SetActive(true);
        }
    }

    public void SelectCard(int num)
    {
        if (GameManager.Instance.Inventory.IsCanGetCard(Cards[num].Type))
        {
            GameManager.Instance.Inventory.GetCard(Cards[num].Type);
            Cards[num].Apply();
        }
        GameManager.Instance.Cardget = true;
        transform.gameObject.SetActive(false);

        window.Refresh();
    }
}
