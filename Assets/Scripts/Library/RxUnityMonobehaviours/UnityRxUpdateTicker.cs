using UnityEngine;
using System;
using System.Reactive.Subjects;

public class UnityRxUpdateTicker : MonoBehaviour
{
    private ISubject<float> _ticks; 

	void Awake () {
        _ticks = new Subject<float>();
	}

    public IObservable<float> Ticks()
    {
        return this._ticks;
    } 
	
	// Update is called once per frame
	void Update () {
        _ticks.OnNext(Time.deltaTime);
	}

    void OnDestroy()
    {
        _ticks.OnCompleted();
    }
}
