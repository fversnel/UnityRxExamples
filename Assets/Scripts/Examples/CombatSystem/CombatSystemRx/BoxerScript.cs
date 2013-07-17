using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Boxer;
using UnityEngine;
using UnityRxExample.Keyboard;

[RequireComponent(typeof(UnityRxScheduler))]
[RequireComponent(typeof(UnityRxKeyboard))]
class BoxerScript : MonoBehaviour
{

    /// <summary>Whenever the source produces a value, buffer that source (including the initial value) for a specified duration.
    /// Only one buffer may be active at a time.
    /// When a buffer closes, allow for a new buffer to start.</summary>
    public static IObservable<IList<T>> InputBuffer<T>(IObservable<T> source, TimeSpan bufferTime, IScheduler scheduler)
    {
        var closeBuffer = source.Delay(bufferTime, scheduler);
        return source.Buffer(bufferClosingSelector: () => closeBuffer);
    }

    /// <summary>
    /// 
    /// </summary>
    public static IObservable<IObservable<T>> SequentialInput<T, TS>(IObservable<T> source, IObservable<TS> reset)
    {
        var resetWindow = source.Select(x => reset).Switch();
        return source.Window(windowClosingSelector: () => resetWindow);
    }

    void Start()
    {
        var keyboard = GetComponent<UnityRxKeyboard>().Events();
        var scheduler = GetComponent<UnityRxScheduler>();

        var actions = from keyEvent in keyboard 
                      where keyEvent.Type == KeyEventType.KeyDown && BoxerData.ActionMapping.ContainsKey(keyEvent.KeyCode)
                      select BoxerData.ActionMapping[keyEvent.KeyCode];

        var actionWindow = InputBuffer(source: actions, bufferTime: TimeSpan.FromMilliseconds(20), scheduler: scheduler);
        //actionWindow.Subscribe(i => Debug.Log(i.ToString()));

        var moves = SequentialInput(actionWindow, Observable.Timer(TimeSpan.FromMilliseconds(400), scheduler))
            .SelectMany(move => move.Scan(new List<IList<BoxerAction>>(), (agg, elem) =>
                {
                    agg.Add(elem);
                    return agg;
                }))
            .Select(move => new InputSequence(move));
        moves.Subscribe(m => Debug.Log(m.ToString()));
    }
}
