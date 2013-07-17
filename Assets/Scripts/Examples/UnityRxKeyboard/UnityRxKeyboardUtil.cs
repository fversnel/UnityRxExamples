using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using UnityEngine;
using UnityRxExample.Keyboard;

namespace UnityRxExample
{
    public class UnityRxKeyboardUtil
    {
        public static IObservable<KeyboardEvent> KeyDown(IObservable<KeyboardEvent> keyboard, KeyCode keyCode)
        {
            return keyboard.Where(e => e.Equals(KeyboardEvent.KeyDown(keyCode)));
        }

        public static IObservable<KeyboardEvent> KeyUp(IObservable<KeyboardEvent> keyboard, KeyCode keyCode)
        {
            return keyboard.Where(e => e.Equals(KeyboardEvent.KeyUp(keyCode)));
        }

        /// <summary>
        /// Returns the keys that are currently being held.</summary>
        public static IObservable<HashSet<KeyCode>> KeysHeld(IObservable<KeyboardEvent> keyboard)
        {
            return keyboard
                .Scan(new HashSet<KeyCode>(), (keysHeld, keyEvent) =>
                {
                    switch (keyEvent.Type)
                    {
                        case KeyEventType.KeyDown:
                            keysHeld.Add(keyEvent.KeyCode);
                            break;
                        case KeyEventType.KeyUp:
                            keysHeld.Remove(keyEvent.KeyCode);
                            break;
                    }
                    return keysHeld;
                });
        }
    }
}
