﻿using System;


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
                AddState(1,this);
            }

            public MarkovState(float[] probs) : this()
            {
                foreach (float prob in probs)
                {
                    MarkovState state = new MarkovState();
                    AddState(prob, state);
                }
                AddState(probs.Sum(x => x), this);

            }

            public void AddState(float probability, MarkovState state)
            {
                List<float> keys = states.Keys.ToList<float>();
                float sum = keys.Sum((float x) => !states[x].Equals(this) ? x : 0);
                if (sum + probability > 1)
                    throw new Exception();
                states.Remove(1 - sum);
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
                float limit = 0;
                foreach (float key in keys)
                {
                    if (limit<=num && num <= limit+key)
                        state = states[key];
                    limit += key;
                }
                if (!this.Equals(state))
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

            public void Step()
            {
                MarkovState state = CurrentState.Run();
                if (state != null)
                    CurrentState = state;
            }
        }
    }
}
