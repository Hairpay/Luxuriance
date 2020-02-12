﻿using System;
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
        if( Input.GetAxis( "Trigger" ) > 0 )
        {
            var raycast = CastRay();
            if( raycast.collider != null && raycast.collider.GetComponents<Analyzable>().Length > 0 )
            {
                var analyzable = raycast.collider.GetComponent<Analyzable>();
                if( GetAnalysis() )
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
        Debug.Log( "Casting a ray" );
        var origin = new Vector2(transform.position.x, transform.position.y);
        var direction = new Vector2
        {
            x = Input.GetAxis( "HorizontalDirection"),
            y = Input.GetAxis( "VerticalDirection")
        };

        direction = direction.normalized * _rayDistance;

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