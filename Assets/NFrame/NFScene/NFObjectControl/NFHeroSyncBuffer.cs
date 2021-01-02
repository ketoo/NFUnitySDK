using System;
using System.Collections.Generic;
using UnityEngine;

public class NFHeroSyncBuffer : MonoBehaviour
{
    protected List<Keyframe> _keyframes = new List<Keyframe>();
    //pool manager
    public class Keyframe
    {
        public int InterpolationTime;
        public Vector3 Position;
        public Vector3 Director;
        public int status;
    }

    public Keyframe NextKeyframe()
    {
        if (_keyframes.Count > 0)
        {
            Keyframe keyframe = _keyframes[0];
            _keyframes.RemoveAt(0);
            return keyframe;
        }

        return null;
    }

    public Keyframe LastKeyframe()
    {
        if (_keyframes.Count > 0)
        {
            Keyframe keyframe = _keyframes[_keyframes.Count - 1];
            _keyframes.Clear();

            return keyframe;
        }

        return null;
    }

    public virtual void AddKeyframe(Keyframe keyframe)
    {
        _keyframes.Add(keyframe);
    }

    public virtual void AddKeyframe(int interpolationTime, Vector3 position, Vector3 director)
    {
        // prevent long first frame if some keyframes was skipped before the first frame

        var keyframe = new Keyframe
        {
            InterpolationTime = interpolationTime,
            Position = position,
            Director = director
        };

        _keyframes.Add(keyframe);
    }


    public virtual void Clear()
    {
        _keyframes.Clear();
    }

    public int Size()
    {
        return _keyframes.Count;
    }
}