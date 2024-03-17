using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public static readonly string lastPlayedStageKey = "n_lastPlayedStage";
    public static readonly string randomizeStagesKey = "n_randomizeStages";
    public static readonly string cumulativeStagePlayedKey = "n_cumulativeStages";

    [HideInInspector] public bool isLevelActive = false;
    [HideInInspector] public bool isLevelSuccessful = false;

    public event System.Action LevelStartedEvent;
    public event System.Action LevelEndedEvent; // fired regardless of fail or success
    public event System.Action LevelSuccessEvent; // fired only on success
    public event System.Action LevelFailedEvent; // fired only on fail
    public event System.Action LevelAboutToChangeEvent; // fired just before next level load

    public int BlastObjectiveAmount;
    public int MaxTargetScore;
    GridManager _gridManager => GridManager.instance;
    private void Start()
    {
        CanvasManager.instance.ScoreUpdatedEvent += OnScoreUpdated;
    }

    private void OnScoreUpdated(int score)
    {
        if (score >= MaxTargetScore)
            EndGame(success: true);
    }

    protected override void Awake()
    {
        base.Awake();

        if (!PlayerPrefs.HasKey(cumulativeStagePlayedKey)) PlayerPrefs.SetInt(cumulativeStagePlayedKey, 1);

        Application.targetFrameRate = 999;
        QualitySettings.vSyncCount = 0;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

    }

    public void StartGame()
    {
        isLevelActive = true;
        LevelStartedEvent?.Invoke();
    }

    public void EndGame(bool success)
    {
        isLevelActive = false;
        isLevelSuccessful = success;

        LevelEndedEvent?.Invoke();
        if (success)
        {
            LevelSuccessEvent?.Invoke();
        }
        else
        {
            LevelFailedEvent?.Invoke();

        }
    }

    public void NextStage()
    {
        PlayerPrefs.SetInt(cumulativeStagePlayedKey, PlayerPrefs.GetInt(cumulativeStagePlayedKey, 1) + 1);

        int targetScene;

        if (PlayerPrefs.GetInt(randomizeStagesKey, 0) == 0)
        {
            targetScene = SceneManager.GetActiveScene().buildIndex + 1;
            if (targetScene == SceneManager.sceneCountInBuildSettings)
            {
                targetScene = RandomStage();
                PlayerPrefs.SetInt(randomizeStagesKey, 1);
            }
        }

        else
        {
            targetScene = RandomStage();
        }

        PlayerPrefs.SetInt(lastPlayedStageKey, targetScene);
        LevelAboutToChangeEvent?.Invoke();
        RestartStage();
    }

    public void RestartStage()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private int RandomStage()
    {
        return Random.Range(2, SceneManager.sceneCountInBuildSettings);
    }

    public int GetTotalStagePlayed()
    {
        return PlayerPrefs.GetInt(cumulativeStagePlayedKey, 1);
    }

    public void CheckFailStatus()
    {
        int occupiedCellAmount = 0;
        for (int i = 0; i < _gridManager.transform.childCount; i++)
        {
            CellController cell = _gridManager.transform.GetChild(i).GetComponent<CellController>();

            if (cell.isOccupied && !cell.IsAction)
                occupiedCellAmount++;
        }
        if (occupiedCellAmount >= _gridManager.transform.childCount)
        {
            EndGame(success: false);
        }
    }
}
