using System.Collections.Generic;
using UnityEngine;
using System;
using System.Reactive.Subjects;
using UnityRxExample.Keyboard;

public class UnityRxKeyboard : MonoBehaviour
{
    private IEnumerable<KeyCode> _keys;
    private ISubject<KeyboardEvent> _keyEvents;

    void Awake()
    {
        _keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
        _keyEvents = new Subject<KeyboardEvent>();
    }

    public IObservable<KeyboardEvent> Events()
    {
        return _keyEvents;
    }

	// Update is called once per frame
	void Update() {
        foreach (var key in _keys)
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

    void OnDestroy()
    {
        _keyEvents.OnCompleted();
    }
}
