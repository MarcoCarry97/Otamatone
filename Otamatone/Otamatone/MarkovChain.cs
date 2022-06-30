using System;


namespace Otamatone
{
    namespace Probability
    {
        public class MarkovState
        {
            private Dictionary<float, MarkovState> states;

            private event Action OnEnter;
            private event Action OnExit;
            public MarkovState()
            {
                states = new Dictionary<float, MarkovState>();
                AddState(1, this);
            }

            public MarkovState(float[] probs) : this()
            {
                foreach (float prob in probs)
                {
                    MarkovState state = new MarkovState();
                    AddState(prob, state);
                }

            }

            public void AddState(float probability, MarkovState state)
            {
                List<float> keys = states.Keys.ToList<float>();
                float sum = keys.Sum(x => x);
                if (sum + probability > 1)
                    throw new Exception();
                if (states.ContainsKey(1))
                {
                    states.Remove(1);
                    states.Remove(0);
                }
                else
                {
                    states.Remove(sum);
                }
                states.Add(probability, state);
                states.Add(sum + probability, this);
            }

            public void AddOnEnterAction(Action action)
            {
                OnEnter += action;
            }

            public void AddOnExitAction(Action action)
            {
                OnExit += action;
            }
            public MarkovState Run()
            {
                OnEnter();
                float num = new Random().Next(1);
                List<float> keys = states.Keys.ToList<float>();
                MarkovState state = null;
                foreach (float key in keys)
                {
                    if (num <= key)
                        state = states[key];
                }
                if (!state.Equals(this))
                    OnExit();
                return state;
            }
        }

        public class MarkovChain
        {
            private MarkovState initialState;
            public MarkovState CurrentState { get; private set; }

            public MarkovChain(MarkovState initialState)
            {
                this.initialState =CurrentState= initialState; 
            }

            public void Run()
            {
                bool end = false;
                while(!end)
                {
                    MarkovState state = CurrentState.Run();
                    if (state != null)
                        CurrentState = state;
                }
            }
        }
    }
}
