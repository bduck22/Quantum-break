using UnityEngine;

public class TurretInstaller : MonoBehaviour
{
    public GameObject TurretPreview;

    public bool IsActived;

    Transform camera;

    public bool IsInstall;

    public TurretController LookTurret;

    public bool CanInstall;

    public Turret_Type type;

    LayerMask TurretLayer;
    LayerMask MapLayer;

    public float InteractionDistance;

    private void Awake()
    {
        camera = Camera.main.transform;

        TurretLayer = LayerMask.GetMask("TurretBody");
        MapLayer = LayerMask.GetMask("Map");
    }

    public void PreViewActive()
    {
        IsActived = true;
        for (int i = 0; i < TurretPreview.transform.childCount; i++)
        {
            TurretPreview.transform.GetChild(i).gameObject.SetActive(false);
        }

        TurretPreview.transform.GetChild((int)type).gameObject.SetActive(true);
    }

    public void PreViewDeActive()
    {
        IsActived = false;
        for (int i = 0; i < TurretPreview.transform.childCount; i++)
        {
            TurretPreview.transform.GetChild(i).gameObject.SetActive(false);
        }
    }

    private void Update()
    {

        RaycastHit hit;
        if (Physics.Raycast(camera.position, camera.forward, out hit, InteractionDistance, TurretLayer))
        {
            LookTurret = hit.transform.GetComponent<TurretController>();
            IsInstall = true;
            if (IsActived)
            {
                PreViewDeActive();
            }
        }
        else if(CanInstall)
        {
            LookTurret = null;
            if (Physics.Raycast(camera.position, camera.forward, out hit, InteractionDistance, MapLayer))
            {
                if(hit.normal.y >= 0.65)
                {
                    IsInstall = true;

                    if (!IsActived)
                    {
                        PreViewActive();
                    }
                }
            }
            else
            {
                if (IsActived)
                {
                    PreViewDeActive();
                }
                IsInstall = false;
            }
        }
        else
        {
            LookTurret = null;
            if (IsActived)
            {
                IsInstall = false;
                PreViewDeActive();
            }
        }

        if (IsActived)
        {
            TurretPreview.transform.position = hit.point;
        }
    }

    public bool Interaction()
    {
        if (LookTurret)
        {
            LookTurret.UnInstall();
            return false;
        }
        else
        {
            SpawnManagers.Instance.Turret.SetTurret(type, TurretPreview.transform.GetChild((int)type).position);
            return true;
        }
    }
}