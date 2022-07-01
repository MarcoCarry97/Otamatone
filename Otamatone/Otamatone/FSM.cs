using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otamatone
{
    namespace Automa
    {
        public delegate bool FSMCondition();

        public class FSMState
        {
            private Dictionary<FSMCondition, FSMState> states;

            private event Action onStay;
            private event Action onEnter;
            private event Action onExit;

            private bool firstTime;

            public FSMState()
            {
                states = new Dictionary<FSMCondition, FSMState>();
                firstTime = false;
            }

            public void addState(FSMCondition condition, FSMState state)
            {
                states[condition] = state;
            }

            public FSMState Run()
            {
                FSMState state = null;
                if (firstTime)
                {
                    onEnter();
                }
                else
                {
                    bool stay = true;
                    foreach (FSMCondition condition in states.Keys)
                    {
                        if (condition())
                        {
                            onExit();
                            state = states[condition];
                            stay = false;
                            break;
                        }
                    }
                    if (stay)
                        onStay();
                }
                return state;
            }

            public void AddOnStayAction(Action action)
            {
                onStay+=action;
            }

            public void AddOnEnterAction(Action action)
            {
                onEnter += action;
            }

            public void AddOnExitAction(Action action)
            {
                onExit += action;
            }

            public void Initialize()
            {
                firstTime = true;
                foreach (FSMCondition condition in states.Keys)
                    states[condition].Initialize();
            }

        }

        public class FiniteStateMachine
        {
            private FSMState initialState;
            public FSMState CurrentState { get; private set; }

            public FiniteStateMachine(FSMState currentState)
            {
                initialState=CurrentState = currentState;
            }

            public void Step()
            {
                FSMState state = CurrentState.Run();
                if (state != null)
                    CurrentState = state;
            }

            public void Initialize()
            {
                CurrentState = initialState;
                initialState.Initialize();
            }
        }

    }
}
