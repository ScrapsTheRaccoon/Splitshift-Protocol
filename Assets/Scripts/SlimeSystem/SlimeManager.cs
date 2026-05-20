using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeManager : MonoBehaviour
{
    //[SerializeField] private GameObject playerContainer;
    [SerializeField] private GameObject _largeSlimePrefab;
    [SerializeField] private GameObject _mediumSlimePrefab;
    [SerializeField] private GameObject _smallSlimePrefab;

    [SerializeField] private GameObject splitEffectPrefab;
    [SerializeField] private GameObject MergeEffectPrefab;

    [SerializeField] private AudioClip invalidActionClip;
    [SerializeField] private AudioClip CameraMoveClip;

    private StatusBarController statusBarController;
    private PortraitController portraitController;

    private List<Slime> slimes = new List<Slime>();
    private int _activeSlimeIndex = 0;

    private PlayerEmotionBubble _activeEmotionBubble;
    private ScreenShake screenShake;

    private void Start()
    {
        screenShake = GameObject.Find("Main Camera").GetComponent<ScreenShake>();

        OnUIReady();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            SwitchPlayer();
        }
    }

    public void OnUIReady()
    {
        statusBarController = FindAnyObjectByType<StatusBarController>();
        portraitController = FindAnyObjectByType<PortraitController>();

        SetActiveSlime();
    }

    public void RegisterSlime(Slime slime)
    {
        if (slime == null || slimes.Contains(slime))
        {
            return;
        }

        slimes.Add(slime);

        if (slimes.Count == 1)
        {
            _activeSlimeIndex = 0;
            SetActiveSlime();
        }

    }

    public void RemoveSlime(Slime slime)
    {
        int index = slimes.IndexOf(slime);
        if (index == -1) return;

        slimes.RemoveAt(index);

        if (index <= _activeSlimeIndex && _activeSlimeIndex > 0)
        {
            _activeSlimeIndex--;
        }

        SetActiveSlime();
    }

    #region Split Mechanics
    public void Split(Slime slime)
    {
        if (!slime.CanSplit())
        {
            AudioManager.Instance.PlaySFX(invalidActionClip);
            InvalidAction();
            return;
        }

        Vector3 pos = slime.transform.position;

        //AudioManager.Instance.PlaySFX(splitClip);
        Instantiate(splitEffectPrefab, pos, Quaternion.identity);
        StartCoroutine(HandleSplitWithDelay(slime, 0.3f));
    }

    private IEnumerator HandleSplitWithDelay(Slime slime, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 pos = slime.transform.position;
        List<Slime> newSlimes = new List<Slime>();

        var (firstStats, secondStats) = slime.GetSplitValues();

        GameObject firstObj, secondObj;

        switch (slime.size)
        {
            case Slime.SlimeSize.Large:
                firstObj = Instantiate(_mediumSlimePrefab, pos + Vector3.left * 0.5f, Quaternion.identity); //, playerContainer.transform);
                secondObj = Instantiate(_smallSlimePrefab, pos + Vector3.right * 0.5f, Quaternion.identity); //, playerContainer.transform);
                break;

            case Slime.SlimeSize.Medium:
                firstObj = Instantiate(_smallSlimePrefab, pos + Vector3.left * 0.5f, Quaternion.identity); //, playerContainer.transform);
                secondObj = Instantiate(_smallSlimePrefab, pos + Vector3.right * 0.5f, Quaternion.identity); //, playerContainer.transform);
                break;

            default:
                AudioManager.Instance.PlaySFX(invalidActionClip);
                yield break;
        }

        screenShake.ShakeCamera(1f, 1f);

        Slime firstSlime = firstObj.GetComponent<Slime>();
        Slime secondSlime = secondObj.GetComponent<Slime>();

        firstSlime.Init(firstStats.health, firstStats.moisture);
        secondSlime.Init(secondStats.health, secondStats.moisture);

        firstSlime.GetComponent<PlayerMoisture>()?.Init(this);
        secondSlime.GetComponent<PlayerMoisture>()?.Init(this);

        RegisterSlime(firstSlime);
        RegisterSlime(secondSlime);

        _activeSlimeIndex = slimes.IndexOf(firstSlime);
        SetActiveSlime();

        RemoveSlime(slime);
        Destroy(slime.gameObject);
    }
    #endregion

    #region Merge Mechanics
    public void Merge(Slime slimeA, Slime slimeB)
    {
        Vector3 mergePosition = (slimeA.transform.position + slimeB.transform.position) / 2f;
        Instantiate(MergeEffectPrefab, mergePosition, Quaternion.identity);

        StartCoroutine(HandleMergeWithDelay(slimeA, slimeB, mergePosition, 0.5f));
    }

    private IEnumerator HandleMergeWithDelay(Slime slimeA, Slime slimeB, Vector3 mergePosition, float delay)
    {
        yield return new WaitForSeconds(delay);

        Slime.SlimeSize resultSize;

        if (slimeA.size == Slime.SlimeSize.Small && slimeB.size == Slime.SlimeSize.Small)
        {
            resultSize = Slime.SlimeSize.Medium;
        }
        else if ((slimeA.size == Slime.SlimeSize.Medium && slimeB.size == Slime.SlimeSize.Small) ||
                 (slimeA.size == Slime.SlimeSize.Small && slimeB.size == Slime.SlimeSize.Medium))
        {
            resultSize = Slime.SlimeSize.Large;
        }
        else
        {
            AudioManager.Instance.PlaySFX(invalidActionClip);
            yield break;
        }

        float totalHealth = slimeA.GetCombinedHealth(slimeB);
        float totalMoisture = slimeA.GetCombinedMoisture(slimeB);

        RemoveSlime(slimeA);
        RemoveSlime(slimeB);
        Destroy(slimeA.gameObject);
        Destroy(slimeB.gameObject);

        GameObject prefabToSpawn = resultSize switch
        {
            Slime.SlimeSize.Small => _smallSlimePrefab,
            Slime.SlimeSize.Medium => _mediumSlimePrefab,
            Slime.SlimeSize.Large => _largeSlimePrefab,
            _ => null
        };

        if (prefabToSpawn == null)
        {
            Debug.LogError("Merge prefab missing for result size.");
            yield break;
        }

        GameObject newSlimeGO = Instantiate(prefabToSpawn, mergePosition, Quaternion.identity); //, playerContainer.transform);
        Slime newSlime = newSlimeGO.GetComponent<Slime>();
        RegisterSlime(newSlime);

        float maxHealth = newSlime.GetComponent<PlayerHealth>().maxHealth;
        float maxMoisture = newSlime.GetComponent<PlayerMoisture>().MaxMoisture;

        newSlime.Init(Mathf.Min(totalHealth, maxHealth), Mathf.Min(totalMoisture, maxMoisture));
        newSlime.GetComponent<PlayerMoisture>()?.Init(this);

        _activeSlimeIndex = slimes.IndexOf(newSlime);
        SetActiveSlime();
    }
    #endregion

    #region Active Player Handling
    public void SetActiveSlime()
    {
        if (slimes.Count == 0)
            return;

        _activeSlimeIndex = Mathf.Clamp(_activeSlimeIndex, 0, slimes.Count - 1);

        for (int i = 0; i < slimes.Count; i++)
        {
            slimes[i].isActivePlayer = (i == _activeSlimeIndex);
        }

        if (statusBarController != null && portraitController != null && slimes.Count > 0)
        {
            statusBarController.UpdateUIWithActiveSlime(slimes[_activeSlimeIndex], slimes.Count);
            portraitController.UpdatePortrait(slimes[_activeSlimeIndex]);
        }

        // Cache active slime's emotion bubble
        var activeSlime = slimes[_activeSlimeIndex];
        var activePlayer = activeSlime.GetComponent<Player>();
        if (activePlayer != null)
        {
            _activeEmotionBubble = activePlayer.GetComponentInChildren<PlayerEmotionBubble>();
        }
    }


    public void SwitchPlayer()
    {
        CleanupDeadSlimes();
        _activeSlimeIndex = Mathf.Clamp(_activeSlimeIndex, 0, slimes.Count - 1);

        if (slimes.Count <= 1) return;

        // Deactivate current
        slimes[_activeSlimeIndex].isActivePlayer = false;

        // Move to the next one (wrap around if needed)
        _activeSlimeIndex = (_activeSlimeIndex + 1) % slimes.Count;

        // Activate new one
        slimes[_activeSlimeIndex].isActivePlayer = true;

        // PLay camera whooshing sound
        AudioManager.Instance.PlaySFX(CameraMoveClip);

        if (statusBarController != null && portraitController != null && slimes.Count > 0)
        {
            statusBarController.UpdateUIWithActiveSlime(slimes[_activeSlimeIndex], slimes.Count);
            portraitController.UpdatePortrait(slimes[_activeSlimeIndex]);
        }

    }
    #endregion

    public void TriggerWarningOnActive()
    {
        if (_activeEmotionBubble != null)
        {
            _activeEmotionBubble.ShowWarning();
        }
    }

    public void InvalidAction()
    {
        if (_activeEmotionBubble != null)
        {
            _activeEmotionBubble.TriggerInvalidWarning();
        }
    }

    public void OnPlayerDeath(Slime slime)
    {
        // Check if dying slime is the active player
        bool isActive = (slime == slimes[_activeSlimeIndex]);

        // if the dying slime is an inactive slime, switch the camera
        if (!isActive)
        {
            CameraController cam = FindAnyObjectByType<CameraController>();
            cam?.FocusOnTemporaryTarget(slime.transform);

            if (_activeEmotionBubble != null)
            {
                _activeEmotionBubble.ShowIdle();
            }
            AudioManager.Instance.PlaySFX(CameraMoveClip);
        }

        RemoveSlime(slime);
        SwitchPlayer();
        CheckForGameOver();
    }

    private void CheckForGameOver()
    {
        if (slimes.Count == 0 && GameManager.Instance != null)
        {
            GameManager.Instance.TriggerGameOver();
        }
    }


    private void CleanupDeadSlimes()
    {
        slimes.RemoveAll(s => s == null);
    }

    public Transform GetActiveSlimeTransform()
    {
        if (slimes.Count == 0) return null;
        return slimes[_activeSlimeIndex].transform;
    }
}
