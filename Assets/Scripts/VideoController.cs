using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image blackScreen; // UI Image preta em cima de tudo

    void Start()
    {
        blackScreen.gameObject.SetActive(true);
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
        blackScreen.gameObject.SetActive(false);
    }
}