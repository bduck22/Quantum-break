using UnityEngine;

public class UIUpdateManager : MonoBehaviour
{
    public static UIUpdateManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public UIController UIController;
}
