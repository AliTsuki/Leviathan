using UnityEngine;

// Modifies the behaviour of Audio Sources
public static class AudioController
{
    // Fade in Audio
    public static void FadeIn(AudioSource _source, float _step, float _maxVolume)
    {
        // Get volume of audio source
        float volume = _source.volume;
        // If source is playing...
        if(_source.isPlaying)
        {
            // If volume is less than max...
            if(volume < _maxVolume)
            {
                // If volume plus step is less than max, add step to volume
                if(volume + _step <= _maxVolume)
                {
                    _source.volume += _step;
                }
                // If volume plus step is more than max, set to max volume
                else if(volume + _step > _maxVolume)
                {
                    _source.volume = _maxVolume;
                }
            }
        }
        // If source is not playing, set to 0 and start playing audio
        else
        {
            _source.volume = 0f;
            _source.Play();
        }
    }

    // Fade out Audio
    public static void FadeOut(AudioSource _source, float _step, float _minVolume)
    {
        // Get volume of audio source
        float volume = _source.volume;
        // If volume is greater than zero...
        if(volume > 0)
        {
            // If volume is greater than min...
            if(volume > _minVolume)
            {
                // If volume minus step is greater or equal to min, minus step from volume
                if(volume - _step >= _minVolume)
                {
                    _source.volume -= _step;
                }
                // If volume minus step is less than min, set volume to min volume
                else if(volume - _step < _minVolume)
                {
                    _source.volume = _minVolume;
                }
            }
        }
        // If volume is 0, stop playing audio
        else
        {
            _source.Stop();
        }
    }
}
