using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleGameObjectLink : MonoBehaviour {

    [SerializeField]
    private ShittyKeyboard m_shittyRef = null;
    [SerializeField]
    private int m_keyToLook = 0;

    private void Start()
    {
        m_shittyRef.RegisterStateListener(m_keyToLook, SetState);
    }

    private void SetState(bool newState)
    {
        gameObject.SetActive(newState);
    }
}
