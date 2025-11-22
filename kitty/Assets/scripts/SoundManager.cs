using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button muteButton;
    [SerializeField] private Image muteButtonImage;
    [SerializeField] private Slider volumeSlider;

    [Header("Mute Icons")]
    [SerializeField] private Sprite unmutedIcon; // Normal speaker icon
    [SerializeField] private Sprite mutedIcon;   // Speaker with slash icon

    [Header("Audio Source")]
    [SerializeField] private AudioSource backgroundMusic;

    // Private variables
    private bool isMuted = false;
    private float previousVolume = 0.7f; // Default 70%

    void Start()
    {
        // Initialize volume slider
        if (volumeSlider != null)
        {
            volumeSlider.minValue = 0f;
            volumeSlider.maxValue = 1f;
            volumeSlider.value = previousVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        // Initialize mute button
        if (muteButton != null)
        {
            muteButton.onClick.AddListener(ToggleMute);
        }

        // Set initial audio volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = previousVolume;
        }

        // Update icon
        UpdateMuteIcon();
    }

    // Called when mute button is clicked
    public void ToggleMute()
    {
        if (isMuted)
        {
            // Unmute: restore previous volume
            isMuted = false;

            if (backgroundMusic != null)
            {
                backgroundMusic.volume = previousVolume;
            }

            // Update slider without triggering listener
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            volumeSlider.value = previousVolume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        else
        {
            // Mute: save current volume and set to 0
            previousVolume = volumeSlider.value;

            // Make sure we save a non-zero value
            if (previousVolume == 0f)
            {
                previousVolume = 0.7f;
            }

            isMuted = true;

            if (backgroundMusic != null)
            {
                backgroundMusic.volume = 0f;
            }

            // Update slider without triggering listener
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            volumeSlider.value = 0f;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        UpdateMuteIcon();
    }

    // Called when volume slider value changes
    public void OnVolumeChanged(float value)
    {
        // Update audio volume
        if (backgroundMusic != null)
        {
            backgroundMusic.volume = value;
        }

        // If volume is 0, set to muted state
        if (value == 0f)
        {
            if (!isMuted)
            {
                // Save previous volume before muting via slider
                if (volumeSlider.value > 0f)
                {
                    previousVolume = volumeSlider.value;
                }
            }
            isMuted = true;
        }
        else
        {
            // If volume is above 0, unmute
            isMuted = false;
            previousVolume = value; // Update saved volume
        }

        UpdateMuteIcon();
    }

    // Update the mute button icon
    private void UpdateMuteIcon()
    {
        if (muteButtonImage == null) return;

        if (isMuted || volumeSlider.value == 0f)
        {
            muteButtonImage.sprite = mutedIcon;
        }
        else
        {
            muteButtonImage.sprite = unmutedIcon;
        }
    }

    void OnDestroy()
    {
        // Clean up listeners
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
        }

        if (muteButton != null)
        {
            muteButton.onClick.RemoveListener(ToggleMute);
        }
    }
}