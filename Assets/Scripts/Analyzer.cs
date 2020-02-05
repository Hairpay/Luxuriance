using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Analyzer : MonoBehaviour
{
    [Min(0.0f)] public float _rayDistance;
    [Min(0.0f)] public float _analysisDuration;

    private float _analysisDeltaTime;
    private Avatar _avatar;

    private void Start()
    {
        _avatar = GetComponentInParent<Avatar>();
    }

    private void Update()
    {
        if( Input.GetButtonDown( "Analyze" ) )
        {
            var raycast = CastRay();
            var analyzable = raycast.collider.GetComponent<Analyzable>();
            if (analyzable != null)
            {
                if (GetAnalysis())
                {
                    analyzable.DisplayMessage();
                    ResetAnalysisTimer();
                }
            }
            else
            {
                ResetAnalysisTimer();
            }
        }
        else
        {
            ResetAnalysisTimer();
        }
    }

    private RaycastHit2D CastRay()
    {
        var origin = gameObject.transform.position;
        var direction = new Vector2
        {
            x = Input.GetAxisRaw("HorizontalDirection") * _rayDistance,
            y = Input.GetAxisRaw("VerticalDirection") * _rayDistance
        };

        var raycast = Physics2D.Raycast(origin, direction, _rayDistance);
        Debug.DrawRay(origin, direction, Color.yellow);

        return raycast;
    }

    private bool GetAnalysis()
    {
        _analysisDeltaTime += Time.deltaTime;
        return _analysisDeltaTime > _analysisDuration;
    }

    private void ResetAnalysisTimer()
    {
        _analysisDeltaTime = 0.0f;
    }
}