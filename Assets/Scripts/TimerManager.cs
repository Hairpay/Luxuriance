using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TimerManager
{
    private IDictionary<string, float> _timers;

    public TimerManager()
    {
        _timers = new Dictionary<string, float>();
    }

    public void UpdateTimers()
    {
        IList<string> keys = new List<string>( _timers.Keys );

        foreach( string key in keys )
        {
            _timers[key] -= Time.deltaTime;
            if( _timers[key] <= 0.0f )
            {
                _timers.Remove( key );
            }
        }
    }

    public void CreateTimer(string key, float duration )
    {
        if( !this.Exists(key) )
        {
            _timers.Add( key, duration );
        }
        else
        {
            Debug.LogWarning( @"Key '{key}' already exist in Timers dictionary" );
        }
    }

    public void RemoveTimer(string key )
    {
        if( this.Exists( key ) )
        {
            _timers.Remove( key );
        }
        else
        {
            Debug.LogWarning( @"Key '{key}' does not exist in Timers dictionary" );
        }
    }

    public float GetTimerRemainingTime(string key )
    {
        if( !Exists( key ) )
        {
            return 0.0f;
        }
        return _timers[key];
    }

    public void AddTimeToTimer(string key, float duration)
    {
        _timers[key] += duration;
    }

    public bool Exists(string key )
    {
        return _timers.ContainsKey( key );
    }

    public bool IsOverOrNull(string key )
    {
        return !Exists( key );
    }
}
