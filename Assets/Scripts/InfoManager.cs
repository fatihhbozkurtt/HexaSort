using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoSingleton<InfoManager>
{
    public List<GridInfoAssigner> currentGridInfo;

    public GridInfoAssigner GetCurrentInfo()
    {
        int completedSceneCount = GameManager.instance.GetTotalStagePlayed() - 1;

        completedSceneCount = (completedSceneCount > currentGridInfo.Count - 1) ? currentGridInfo.Count - 1 : completedSceneCount;

        if (completedSceneCount < 0) completedSceneCount = 0;

        return currentGridInfo[completedSceneCount];

    }

}
