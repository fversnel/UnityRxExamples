using System;
using System.Reactive.Linq;
using UnityEngine;
using UnityRxExample.Keyboard;

[RequireComponent(typeof(UnityRxKeyboard))]
class RxKeyboardExampleScript : MonoBehaviour
{
    void Start()
    {
        var keyboard = GetComponent<UnityRxKeyboard>().Events();

        keyboard.Where(e => e.Type == KeyEventType.KeyDown)
                .Subscribe(e => Debug.Log("key down " + e.KeyCode));
        keyboard.Where(e => e.Equals(KeyboardEvent.KeyDown(KeyCode.W)))
                .Subscribe(e => Debug.Log("key event " + e));
    }
}
