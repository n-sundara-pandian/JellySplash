﻿using UnityEngine;
using System.Collections;
using thelab.mvc;

/// <summary>
/// Root class for all views.
/// </summary>
public class BounceView : View<BounceApplication>
{

    /// <summary>
    /// Reference to the Ball view.
    /// </summary>
    public BallView ball { get { return m_ball = Assert<BallView>(m_ball); } }
    private BallView m_ball;

    /// <summary>
    /// Reference to the Ball view.
    /// </summary>
    public TimerView timer { get { return m_timer = Assert<TimerView>(m_timer); } }
    private TimerView m_timer;
}
