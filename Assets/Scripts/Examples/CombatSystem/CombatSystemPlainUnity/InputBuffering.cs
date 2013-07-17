using System;
using System.Collections.Generic;
using System.Linq;
using Boxer;
using UnityEngine;

public class InputBuffering : MonoBehaviour
{
    private static readonly TimeSpan BufferTimeOut = TimeSpan.FromMilliseconds(400);

    private static readonly TimeSpan MergeInputTime = TimeSpan.FromMilliseconds(20);

    private IEnumerable<KeyCode> _keys;

    private TimeSpan _lastBufferUpdateTime;
    private TimeSpan _mergeInputTime;

    private IList<IList<BoxerAction>> _buffer;
    private List<BoxerAction> _mergeBuffer; 

    void Awake()
    {
        _lastBufferUpdateTime = TimeSpan.Zero;
        _keys = Enum.GetValues(typeof(KeyCode)) as KeyCode[];
        _buffer = new List<IList<BoxerAction>>();
        _mergeBuffer = new List<BoxerAction>();
        _mergeInputTime = TimeSpan.Zero;
    }

    // TODO Use LateUpdate to poll the buffer.
    // Problems:
    //  - LateUpdate is not a good ordering mechanism because it has only one level of nesting
    //  - Polling the buffer means we do not know when input actually changes, i.g. we might poll the same buffer again.

    void Update()
    {
        // Expire old input.
        TimeSpan time = TimeSpan.FromSeconds(Time.realtimeSinceStartup);
        TimeSpan timeSinceLastUpdate = time - _lastBufferUpdateTime;
        if (timeSinceLastUpdate > BufferTimeOut)
        {
            _buffer.Clear();
        }

        // Get all of the keys pressed this frame.
        var keysPressed = new List<BoxerAction>();
        foreach (var key in _keys)
        {
            if (Input.GetKeyDown(key) && BoxerData.ActionMapping.ContainsKey(key))
            {
                keysPressed.Add(BoxerData.ActionMapping[key]);
            }
        }

        TimeSpan timeSinceMergeWindowOpen = time - _mergeInputTime;
        // It is very hard to press two buttons on exactly the same frame.
        // If they are close enough, consider them pressed at the same time.
        bool mergeInput = timeSinceMergeWindowOpen < MergeInputTime;

        if (mergeInput)
        {
            _mergeBuffer.AddRange(keysPressed);
        }
        else
        {
            if (_mergeBuffer.Count > 0)
            {
                _buffer.Add(_mergeBuffer);
                _lastBufferUpdateTime = time;

                // Clear the merge buffer
                _mergeBuffer = new List<BoxerAction>();

                Debug.Log(new InputSequence(_buffer).ToString());
            }

            // Start a new merge buffer
            if (keysPressed.Count > 0)
            {
                _mergeBuffer = keysPressed;
                _mergeInputTime = time;
            }
        }
        
    }
}