using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using UnityEngine;
using UnityRxExample.Keyboard;

public class UnityRxKeyboard : MonoBehaviour
{
    private ISubject<KeyboardEvent> _keyEvents;
    private IEnumerable<KeyCode> _keys;

    private void Awake()
    {
        _keys = Enum.GetValues(typeof (KeyCode)) as KeyCode[];
        _keyEvents = new Subject<KeyboardEvent>();
    }

    public IObservable<KeyboardEvent> Events()
    {
        return _keyEvents;
    }

    // Update is called once per frame
    private void Update()
    {
        foreach (KeyCode key in _keys)
        {
            if (Input.GetKeyDown(key))
            {
                _keyEvents.OnNext(new KeyboardEvent(KeyEventType.KeyDown, key));
            }
            else if (Input.GetKeyUp(key))
            {
                _keyEvents.OnNext(new KeyboardEvent(KeyEventType.KeyUp, key));
            }
        }
    }

    private void OnDestroy()
    {
        _keyEvents.OnCompleted();
    }
}