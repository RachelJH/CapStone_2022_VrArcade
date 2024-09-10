using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Timer : MonoBehaviour
{
    public string m_Timer = @"00:00";
    private bool m_IsPlaying;
    public float m_TotalSeconds = 5 * 60; 
    public TMP_Text m_Text;

    private void Start()
    {
        m_Timer = CountdownTimer(false);
    }

    private void Update()
    {
            m_IsPlaying = !m_IsPlaying;

        if (m_IsPlaying)
        {
            m_Timer = CountdownTimer();
        }

        if (m_TotalSeconds <= 0)
        {
            SetZero();
            //카운트0초 되면 실행할 이벤트 여기에 추가
        }

        if (m_Text)
            m_Text.text = m_Timer;
    }

    private string CountdownTimer(bool IsUpdate = true)
    {
        if (IsUpdate)
            m_TotalSeconds -= Time.fixedDeltaTime;

        TimeSpan timespan = TimeSpan.FromSeconds(m_TotalSeconds);
        string timer = string.Format("{1:00}:{2:00}",
            timespan.Hours, timespan.Minutes, timespan.Seconds, timespan.Milliseconds);

        return timer;
    }

    private void SetZero()
    {
        m_Timer = @"00:00";
        m_TotalSeconds = 0;
        m_IsPlaying = false;
    }
}
