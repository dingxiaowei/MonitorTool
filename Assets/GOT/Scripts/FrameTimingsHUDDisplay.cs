using System;
using UnityEngine;
using Unity.Profiling;

public class FrameTimingsHUDDisplay : MonoBehaviour
{
    GUIStyle m_Style;
    // Profiling state.
    private int frameCount = 0;
    private float accumulatedFrameTimeCPU = 0.0f;
    private float accumulatedFrameTimeGPU = 0.0f;
    private FrameTiming[] m_FrameTimings = new FrameTiming[1];

    void Awake()
    {
        m_Style = new GUIStyle();
        m_Style.fontSize = 15;
        m_Style.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        CaptureTimings();

        var reportMsg =
            $"\nCPU: {m_FrameTimings[0].cpuFrameTime:00.00}" +
            $"\nMain Thread: {m_FrameTimings[0].cpuFrameTime:00.00}" +
            $"\ncpuTimeFrameComplete: {m_FrameTimings[0].cpuTimeFrameComplete:00.00}" +
            $"\ncpuTimePresentCalled: {m_FrameTimings[0].cpuTimePresentCalled:00.00}" +
            $"\nGPU: {m_FrameTimings[0].gpuFrameTime:00.00}" +
            $"\nCPUTotal: {accumulatedFrameTimeCPU:00.00}" +
            $"\nCPUAverage: {(accumulatedFrameTimeCPU / frameCount):00.00}" +
            $"\nGPUTotal: {accumulatedFrameTimeGPU:00.00}" +
            $"\nGPUAverage: {(accumulatedFrameTimeGPU / frameCount):00.00}";

        var oldColor = GUI.color;
        GUI.color = new Color(1, 1, 1, 1);
        float w = 300, h = 210;

        GUILayout.BeginArea(new Rect(32, 100, w, h), "Frame Stats", GUI.skin.window);
        GUILayout.Label(reportMsg, m_Style);
        GUILayout.EndArea();

        GUI.color = oldColor;
    }

    private void CaptureTimings()
    {
        FrameTimingManager.CaptureFrameTimings();
        uint frameTimingsCount = FrameTimingManager.GetLatestTimings(1, m_FrameTimings);

        if (frameTimingsCount != 0)
        {
            accumulatedFrameTimeCPU += (float)m_FrameTimings[0].cpuFrameTime;
            accumulatedFrameTimeGPU += (float)m_FrameTimings[0].gpuFrameTime;
        }
        else
        {
            accumulatedFrameTimeCPU += Time.unscaledDeltaTime * 1000.0f;
            // No GPU time to query.
        }

        ++frameCount;
    }
}