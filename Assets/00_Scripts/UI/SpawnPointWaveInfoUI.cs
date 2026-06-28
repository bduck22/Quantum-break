using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpawnPointWaveInfoUI : MonoBehaviour
{
    [Header("UI Root")]
    [SerializeField] private GameObject uiRoot;

    [Header("UI Components")]
    [SerializeField] private Image enemyIconImage;
    [SerializeField] private TextMeshProUGUI enemyNameText;
    [SerializeField] private TextMeshProUGUI spawnCountText;

    [Header("Follow")]
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 2f, 0f);
    [SerializeField] private bool followTarget = true;

    [Header("Billboard")]
    [SerializeField] private bool lookAtCamera = true;
    [SerializeField] private Camera targetCamera;

    private Transform currentTarget;
    private WaveSpawnData currentWaveSpawnData;

    private void Awake()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }

        Close();
    }

    private void LateUpdate()
    {
        if (uiRoot == null || !uiRoot.activeSelf)
        {
            return;
        }

        if (followTarget && currentTarget != null)
        {
            transform.position = currentTarget.position + worldOffset;
        }

        if (lookAtCamera && targetCamera != null)
        {
            LookAtCamera();
        }
    }

    public void Open(Transform target, WaveSpawnData waveSpawnData)
    {
        if (target == null)
        {
            Close();
            return;
        }

        bool isSameTarget = currentTarget == target;
        bool isSameData = currentWaveSpawnData == waveSpawnData;

        currentTarget = target;
        currentWaveSpawnData = waveSpawnData;

        transform.position = currentTarget.position + worldOffset;

        if (uiRoot != null)
        {
            uiRoot.SetActive(true);
        }

        if (isSameTarget && isSameData)
        {
            return;
        }

        Refresh();
    }

    public void Close()
    {
        currentTarget = null;
        currentWaveSpawnData = null;

        if (uiRoot != null)
        {
            uiRoot.SetActive(false);
        }
    }

    private void Refresh()
    {
        if (currentWaveSpawnData == null)
        {
            SetEmptyUI();
            return;
        }

        EnemyDataCollection enemyData = GetEnemyData(currentWaveSpawnData.Type);

        if (enemyData == null)
        {
            SetUnknownEnemyUI();
            return;
        }

        if (enemyIconImage != null)
        {
            enemyIconImage.sprite = enemyData.Icon;
            enemyIconImage.enabled = enemyData.Icon != null;
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = enemyData.Name;
        }

        if (spawnCountText != null)
        {
            spawnCountText.text = $"x{currentWaveSpawnData.SpawnCount}";
        }
    }

    private EnemyDataCollection GetEnemyData(Enemy_Type enemyType)
    {
        if (GameDataManager.Instance == null)
        {
            return null;
        }

        if (!GameDataManager.Instance.IsEnemyData(enemyType))
        {
            return null;
        }

        return GameDataManager.Instance.GetEnemyData(enemyType);
    }

    private void SetEmptyUI()
    {
        if (enemyIconImage != null)
        {
            enemyIconImage.enabled = false;
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = "No Data";
        }

        if (spawnCountText != null)
        {
            spawnCountText.text = "x0";
        }
    }

    private void SetUnknownEnemyUI()
    {
        if (enemyIconImage != null)
        {
            enemyIconImage.enabled = false;
        }

        if (enemyNameText != null)
        {
            enemyNameText.text = currentWaveSpawnData.Type.ToString();
        }

        if (spawnCountText != null)
        {
            spawnCountText.text = $"x{currentWaveSpawnData.SpawnCount}";
        }
    }

    private void LookAtCamera()
    {
        Transform cameraTransform = targetCamera.transform;

        transform.rotation = Quaternion.LookRotation(
            transform.position - cameraTransform.position,
            Vector3.up
        );
    }
}