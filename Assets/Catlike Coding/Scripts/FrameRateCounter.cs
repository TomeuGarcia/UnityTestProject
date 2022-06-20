using UnityEngine;
using TMPro;

public class FrameRateCounter : MonoBehaviour
{
    public enum DisplayMode { FPS, MS }
    [SerializeField] DisplayMode displayMode = DisplayMode.FPS;


    [SerializeField] TextMeshProUGUI display;

    [SerializeField, Range(0.1f, 2.0f)] float sampleDuration = 1.0f;

    int frames = 0;
    float duration = 0.0f;
    float bestDuration = float.MaxValue;
    float worstDuration = 0.0f;

    private void Update()
    {
        float frameDuration = Time.unscaledDeltaTime;

        frames += 1;
        duration += frameDuration;


        if (frameDuration < bestDuration)
        {
            bestDuration = frameDuration;
        }

        if (frameDuration > worstDuration)
        {
            worstDuration = frameDuration;
        }


        if (duration >= sampleDuration)
        {
            if (displayMode == DisplayMode.FPS)
            {
                display.SetText(
                    "FPS\n{0:0}\n{1:0}\n{2:0}",
                    1.0f / bestDuration,
                    frames / duration,
                    1.0f / worstDuration
                );
            }
            else
            {
                display.SetText(
                    "MS\n{0:1}\n{1:1}\n{2:1}",
                    1000f * bestDuration,
                    1000f * duration / frames,
                    1000f * worstDuration
                );
            }



            frames = 0;
            duration = 0.0f;
            bestDuration = float.MaxValue;
            worstDuration = 0.0f;
        }

    }


}
