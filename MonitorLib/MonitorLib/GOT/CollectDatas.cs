using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace MonitorLib.GOT
{
    public class CollectResFrameDatas<T> where T : UnityEngine.Object
    {
        public static KeyValuePair<long, int> TakeSample()
        {
            long size = 0L;
            int count = 0;
            T[] samples = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < samples.Length; i++)
            {
                long sampleSize = Profiler.GetRuntimeMemorySizeLong(samples[i]);
                string name = samples[i].GetType().Name;
                count++;
                size += sampleSize;
            }
            return new KeyValuePair<long, int>(size, count);
        }
    }

    public class CollectDatas<T> where T : UnityEngine.Object
    {
        private readonly List<RecoreInfo> m_Records = new List<RecoreInfo>();
        private readonly Comparison<RecoreInfo> m_RecordComparer = RecordComparer;
        private DateTime m_SampleTime = DateTime.MinValue;
        private int m_SampleCount = 0;
        private long m_SampleSize = 0L;

        public void TakeSample()
        {
            m_Records.Clear();
            m_SampleTime = DateTime.UtcNow;
            m_SampleCount = 0;
            m_SampleSize = 0L;
            //获取所有类型的资源
            T[] samples = Resources.FindObjectsOfTypeAll<T>();
            for (int i = 0; i < samples.Length; i++)
            {
                long sampleSize = Profiler.GetRuntimeMemorySizeLong(samples[i]);
                string name = samples[i].GetType().Name;
                m_SampleCount++;
                m_SampleSize += sampleSize;

                RecoreInfo record = null;
                foreach (RecoreInfo r in m_Records)
                {
                    if (r.Name == name)
                    {
                        record = r;
                        break;
                    }
                }

                if (record == null)
                {
                    record = new RecoreInfo(name);
                    m_Records.Add(record);
                }

                record.Count++;
                record.Size += sampleSize;
            }

            m_Records.Sort(m_RecordComparer);
        }

        private static int RecordComparer(RecoreInfo a, RecoreInfo b)
        {
            int result = b.Size.CompareTo(a.Size);
            if (result != 0)
            {
                return result;
            }

            result = a.Count.CompareTo(b.Count);
            if (result != 0)
            {
                return result;
            }

            return a.Name.CompareTo(b.Name);
        }
    }
}
