using UnityEngine;

// Modifies the behaviour of Audio Sources
public static class AudioController
{
    // Fade in Audio
    public static void FadeIn(AudioSource source, float step, float maxVolume)
    {
        // Get volume of audio source
        float volume = source.volume;
        // If source is playing...
        if(source.isPlaying)
        {
            // If volume is less than max...
            if(volume < maxVolume)
            {
                // If volume plus step is less than max, add step to volume
                if(volume + step <= maxVolume)
                {
                    source.volume += step;
                }
                // If volume plus step is more than max, set to max volume
                else if(volume + step > maxVolume)
                {
                    source.volume = maxVolume;
                }
            }
        }
        // If source is not playing, set to 0 and start playing audio
        else
        {
            source.volume = 0f;
            source.Play();
        }
    }

    // Fade out Audio
    public static void FadeOut(AudioSource source, float step, float minVolume)
    {
        // Get volume of audio source
        float volume = source.volume;
        // If volume is greater than zero...
        if(volume > 0)
        {
            // If volume is greater than min...
            if(volume > minVolume)
            {
                // If volume minus step is greater or equal to min, minus step from volume
                if(volume - step >= minVolume)
                {
                    source.volume -= step;
                }
                // If volume minus step is less than min, set volume to min volume
                else if(volume - step < minVolume)
                {
                    source.volume = minVolume;
                }
            }
        }
        // If volume is 0, stop playing audio
        else
        {
            source.Stop();
        }
    }
}
