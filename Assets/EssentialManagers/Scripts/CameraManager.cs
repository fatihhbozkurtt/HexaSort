using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoSingleton<CameraManager>
{
    public enum CamType
    {
        Menu, Game, Win, Fail
    }

    public Camera mainCam;
    public CinemachineVirtualCamera menuCam;
    public CinemachineVirtualCamera gameCam;
    public CinemachineVirtualCamera winCam;
    public CinemachineVirtualCamera failCam;
    public ParticleSystem confetti;

    CinemachineVirtualCamera[] vcamArr;

    protected override void Awake()
    {
        base.Awake();

        vcamArr = new CinemachineVirtualCamera[System.Enum.GetNames(typeof(CamType)).Length];

        vcamArr[(int)CamType.Menu] = menuCam;
        vcamArr[(int)CamType.Game] = gameCam;
        vcamArr[(int)CamType.Win] = winCam;
        vcamArr[(int)CamType.Fail] = failCam;
    }

    private void Start()
    {
        GameManager.instance.LevelStartedEvent += (() => { SetCam(CamType.Game); });
        GameManager.instance.LevelFailedEvent += (() => { SetCam(CamType.Fail); });
        GameManager.instance.LevelSuccessEvent += (() =>
        {
            SetCam(CamType.Win);
            if (confetti != null) confetti.Play(true);
        });
    }

    public void SetCam(CamType camType)
    {
        for (int i = 0; i < vcamArr.Length; i++)
        {
            if (i == (int)camType)
            {
                vcamArr[i].Priority = 50;
            }

            else
            {
                vcamArr[i].Priority = 0;
            }
        }
    }

    public CinemachineVirtualCamera GetCam(CamType camType)
    {
        return vcamArr[(int)camType];
    }

}
