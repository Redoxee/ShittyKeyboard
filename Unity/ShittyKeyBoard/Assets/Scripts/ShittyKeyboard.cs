using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using System.Threading;
using System.Text;


public class ShittyKeyboard : MonoBehaviour {

    private SerialPort m_serialPort;

    private string m_buffer = "";
    private string m_lastFullState = "";
    private string m_prevState = "";

    Thread thread;
    bool m_isThreadRunning = true;
    
    public const int c_nbField = 8;

    void Awake()
    {
        m_prevState = "";
        for (int i = 0; i < c_nbField; ++i)
        {
            m_stateChageListener[i] = new List<Action<bool>>();
            m_prevState += '3';
        }

        thread = new Thread(ReadData);
        thread.Start();
    }

    void ReadData()
    {
        m_serialPort = new SerialPort("COM3", 9600);
        m_serialPort.Open();
        m_serialPort.BaseStream.ReadTimeout = 1;

        while (m_isThreadRunning)
        {
            try
            {
                m_buffer += (char)m_serialPort.BaseStream.ReadByte();
                if (m_buffer.Length > c_nbField + 1)
                {
                    Debug.Log(m_buffer);
                    var arr = m_buffer.Split(';');
                    m_buffer = arr[arr.Length - 1];

                    var state = arr[arr.Length - 2];
                    if(state.Length == c_nbField)
                        m_lastFullState = state;
                }

            }
            catch (Exception e)
            {
                e.ToString();
                //This is expected if the controller send nothing
            }
        }
    }

    private void OnDestroy()
    {
        m_isThreadRunning = false;
    }

    void Update()
    {
        if (m_buffer.Length > 0)
        {
            string copy = m_lastFullState;
            if (copy == m_prevState)
                return;

            //for (int i = 0; i < c_nbField; ++i)
            //{
            //    Debug.LogFormat("prevState {0} - {1}", copy, m_prevState);
            //    if (copy[i] != m_prevState[i])
            //        NotifyKeyChanged(i,copy[i] == '1');
            //}

            m_prevState = copy;

            Debug.Log(copy);
        }
    }

    private Dictionary<int, List<Action<bool>>> m_stateChageListener = new Dictionary<int, List<Action<bool>>>(c_nbField);

    public void RegisterStateListener(int index, Action<bool> act)
    {
        List<Action<bool>> actList;
        if (!m_stateChageListener.ContainsKey(index))
        {
            actList = new List<Action<bool>>();
            m_stateChageListener[index] = actList;
        }

        actList = m_stateChageListener[index];
        if(!actList.Contains(act))
            actList.Add(act);
    }

    private void NotifyKeyChanged(int key, bool newState)
    {
        var actList = m_stateChageListener[key];
        foreach (var act in actList)
        {
            act(newState);
        }
    }
}
