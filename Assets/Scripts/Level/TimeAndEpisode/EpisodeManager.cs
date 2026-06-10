using System;
using UnityEngine;

public class EpisodeManager : MonoBehaviour
{

    EpisodeTitleCardUI _episodeTitleCardUI;
    
    public Action onStartEpisode;


    private void Awake()
    {
        _episodeTitleCardUI = FindAnyObjectByType<EpisodeTitleCardUI>();
    }

    private void Start()
    {
        onStartEpisode += _episodeTitleCardUI.PlayTitleCardAnim;
    }
    private void OnDestroy()
    {
        onStartEpisode -= _episodeTitleCardUI.PlayTitleCardAnim;
    }
}
