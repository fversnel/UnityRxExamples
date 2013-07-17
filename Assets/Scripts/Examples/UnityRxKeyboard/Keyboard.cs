using UnityEngine;
using System;

namespace UnityRxExample.Keyboard
{
    public enum KeyEventType
    {
        KeyDown,
        KeyUp
    }

    public struct KeyboardEvent : IEquatable<KeyboardEvent>
    {
        private readonly KeyEventType _type;
        private readonly KeyCode _keyCode;

        public static KeyboardEvent KeyUp(KeyCode keyCode)
        {
            return new KeyboardEvent(KeyEventType.KeyUp, keyCode);
        }

        public static KeyboardEvent KeyDown(KeyCode keyCode)
        {
            return new KeyboardEvent(KeyEventType.KeyDown, keyCode);
        }

        public KeyboardEvent(KeyEventType type, KeyCode keyCode)
        {
            this._type = type;
            this._keyCode = keyCode;
        }

        public bool Equals(KeyboardEvent other)
        {
            return string.Equals(_keyCode, other._keyCode) && _type == other._type;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is KeyboardEvent && Equals((KeyboardEvent) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (_keyCode.GetHashCode()*397) ^ (int) _type;
            }
        }

        public override string ToString()
        {
            return string.Format("{0}[\"{1}\"]", _type, _keyCode);
        }

        public KeyEventType Type
        {
            get { return _type; }
        }

        public KeyCode KeyCode
        {
            get { return _keyCode; }
        }
    }
}
