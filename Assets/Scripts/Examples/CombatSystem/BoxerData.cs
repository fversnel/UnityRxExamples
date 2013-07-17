using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Boxer
{
    public enum BoxerAction { Back, Forward, Up, Down, LightPunch, MediumPunch, HeavyPunch }

    public enum Move { LightPunch, MediumPunch, HeavyPunch, SuperPunch, Fireball }

    public struct InputSequence
    {
        private readonly IList<IList<BoxerAction>> _sequence;

        public InputSequence(IList<IList<BoxerAction>> sequence)
        {
            _sequence = sequence;
        }
        
        public InputSequence(BoxerAction[,] sequence)
        {
            _sequence = new List<IList<BoxerAction>>();
            for (var i = 0; i < sequence.GetLength(0); i++)
            {
                var input = new List<BoxerAction>();
                for (var j = 0; j < sequence.GetLength(1); j++)
                {
                    input.Add(sequence[i, j]);
                }
                _sequence.Add(input);
            }
        }

        public IList<IList<BoxerAction>> Value
        {
            get { return _sequence; }
        }

        public override string ToString()
        {
            var inputSequenceStrings = _sequence.Select(input =>
                {
                    var inputString = input.Select(e => e.ToString());
                    return "{" + string.Join(", ", inputString.ToArray()) + "}";
                });
            return "[ " + string.Join(", ", inputSequenceStrings.ToArray()) + " ]";
        }
    }


    public static class BoxerData
    {
        public static readonly IDictionary<KeyCode, BoxerAction> ActionMapping = new Dictionary<KeyCode, BoxerAction>();

        public static readonly IDictionary<InputSequence, Move> MoveMapping = new Dictionary<InputSequence, Move>();

        static BoxerData()
        {
            ActionMapping.Add(KeyCode.UpArrow, BoxerAction.Up);
            ActionMapping.Add(KeyCode.DownArrow, BoxerAction.Down);
            ActionMapping.Add(KeyCode.LeftArrow, BoxerAction.Back);
            ActionMapping.Add(KeyCode.RightArrow, BoxerAction.Forward);
            ActionMapping.Add(KeyCode.Q, BoxerAction.LightPunch);
            ActionMapping.Add(KeyCode.W, BoxerAction.MediumPunch);
            ActionMapping.Add(KeyCode.E, BoxerAction.HeavyPunch);

            MoveMapping.Add(new InputSequence(new[,] { { BoxerAction.LightPunch } }), Move.LightPunch);
            MoveMapping.Add(new InputSequence(new[,] { { BoxerAction.MediumPunch } }), Move.MediumPunch);
            MoveMapping.Add(new InputSequence(new[,] { { BoxerAction.HeavyPunch } }), Move.HeavyPunch);
            MoveMapping.Add(new InputSequence(new[,] { { BoxerAction.LightPunch, BoxerAction.MediumPunch, BoxerAction.HeavyPunch } }), Move.SuperPunch);
            MoveMapping.Add(new InputSequence(new[,] { { BoxerAction.Forward }, { BoxerAction.LightPunch } }), Move.SuperPunch);
        }
    }
}
