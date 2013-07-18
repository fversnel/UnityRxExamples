using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using UnityEngine;
using System.Collections;
using System.Threading;

public class UnityRxScheduler : MonoBehaviour, IScheduler
{
    static readonly DateTimeOffset beginningOfTime = DateTime.Now;

    NodeList<DateTimeOffset, Action> futureTasks = new NodeList<DateTimeOffset, Action>();

    public IDisposable Schedule(Action action)
    {
        return futureTasks.Add(Now.AddTicks(1), action);
    }

    public IDisposable Schedule(Action action, TimeSpan dueTime)
    {
        return futureTasks.Add(Now + dueTime, action);
    }

    public DateTimeOffset Now
    {
        get { return beginningOfTime.AddSeconds(Time.time); }
    }

    void Update()
    {
        DateTimeOffset now = Now;

        var node = futureTasks.FirstNode();
        while (node != null && node.Key <= now)
        {
            node.FlagAsDeleted();
            node.Value();
            node = futureTasks.FirstNode();
        }
    }

    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        return futureTasks.Add(dueTime, () => { action(this, state); });
    }

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
    {
        return futureTasks.Add(Now + dueTime, () => { action(this, state); });
    }

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
    {
        return futureTasks.Add(Now.AddTicks(1), () => { action(this, state); });
    }
}

public class NodeList<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : IComparable<TKey>
{
    readonly Node start = new Node(default(TKey), default(TValue));

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        for (Node n = start.GetNext(); n != null; n = n.GetNext())
        {
            yield return new KeyValuePair<TKey, TValue>(n.key, n.value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public Node Add(TKey key, TValue value)
    {
        Node parent;
        Node node;
        var newNode = new Node(key, value);
        do
        {
            node = FindNode(key, out parent);
        } while (!parent.InsertChild(newNode, node));
        return newNode;
    }

    Node FindNode(TKey key, out Node parent)
    {
        Node n = start;
        Node node = n.GetNext();
        while ((node != null) && node.key.CompareTo(key) <= 0)
        {
            n = node;
            node = n.GetNext();
        }
        parent = n;
        return node;
    }

    public Node FirstNode()
    {
        return start.GetNext();
    }

    public class Node : IDisposable
    {
        internal readonly TKey key;
        internal readonly TValue value;
        NodeState state;

        public Node(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
            state = new NodeState(false, null);
        }

        public TKey Key
        {
            get { return key; }
        }

        public TValue Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            return key + ", " + value;
        }

        public void Dispose()
        {
            FlagAsDeleted();
        }

        internal bool InsertChild(Node newNode, Node successor)
        {
            NodeState oldState = state;

            if ((!oldState.isDeleted) && (oldState.next == successor))
            {
                var newState = new NodeState(false, newNode);
                newNode.state = new NodeState(false, oldState.next);
                return CasState(oldState, newState);
            }
            return false;
        }

        public Node GetNext()
        {
            Node node = state.next;
            // remove everything that is flagged as deleted, until a node is reached that is not deleted (garbage collection at each tick)
            while ((node != null) && (node.state.isDeleted))
            {
                TryDeleteNext(node);
                node = state.next;
            }
            return node;
        }

        void TryDeleteNext(Node next)
        {
            NodeState oldState = state;
            if (oldState.next == next)
            {
                var newState = new NodeState(oldState.isDeleted, next.state.next);
                CasState(oldState, newState);
            }
        }

        public void FlagAsDeleted()
        {
            NodeState newState;
            NodeState oldState;
            do
            {
                oldState = state;
                newState = new NodeState(true, oldState.next);
            } while (!CasState(oldState, newState));
        }

        bool CasState(NodeState oldState, NodeState newState)
        {
            return oldState == Interlocked.CompareExchange(ref state, newState, oldState);
        }
    }

    class NodeState
    {
        internal readonly bool isDeleted;
        internal readonly Node next;

        public NodeState(bool isDeleted, Node next)
        {
            this.isDeleted = isDeleted;
            this.next = next;
        }
    }
}
