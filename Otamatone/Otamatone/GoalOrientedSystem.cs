using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otamatone
{
    namespace Utility
    {
        class Goal
        {
            public float Utility{ get; private set; }

            public string Name { get; private set; }

            public Goal(string name,float utility)
            {
                Name = name;
                Utility = utility;
            }
        }

        class GoalOrientedAction
        {
            public Action Action { get; private set; }

            public Dictionary<string,float> Costs { get; private set; }

            public GoalOrientedAction(Action action)
            {
                this.Action=action;
                Costs = new Dictionary<string, float>();
            }

            public void AddCost(string type, float cost)
            {
                Costs.Add(type, cost);
            }

            public void Apply()
            {
                Action();
            }

            public float MeasureDiscontentment(Func<float,float> measure)
            {
                return Costs.Select(x => x.Value).Sum(measure);
            }
        }

        class GoalOrientedSystem
        {
            private List<Goal> goals;

            private List<GoalOrientedAction> actions;

            private Func<float, float> measure;

            public GoalOrientedSystem(Func<float, float> measure)
            {
                goals = new List<Goal>();
                actions = new List<GoalOrientedAction>();
                this.measure = measure;
            }

            public void AddGoal(Goal goal)
            {
                goals.Add(goal);
            }

            public void AddAction(GoalOrientedAction action)
            {
                actions.Add(action);
            }

            private bool ControlCosts(GoalOrientedAction action,Dictionary<string,float> utilities)
            {
                bool end = false;
                for(int i=0;i<action.Costs.Count && !end;i++)
                {
                    string typeCost = action.Costs.Keys.ToList<string>()[i];
                    if (utilities[typeCost] + action.Costs[typeCost] < 0)
                        end = true;
                }
                return !end;
            }

            private List<GoalOrientedAction> recursiveSearch(List<GoalOrientedAction> actions,Dictionary<string,float> utilities,Func<float,float> measure)
            {
                if(actions.Count==0)
                    return new List<GoalOrientedAction>();
                else
                {
                    GoalOrientedAction? best = actions.Where(x=>ControlCosts(x,utilities)).MinBy(x => x.MeasureDiscontentment(measure));
                    if (best is null)
                        return new List<GoalOrientedAction>();
                    List<GoalOrientedAction> subActions = new List<GoalOrientedAction>(actions);
                    subActions.Remove(best);
                    foreach(string key in best.Costs.Keys.ToList<string>())
                    {
                        utilities[key] += best.Costs[key];
                    }
                    List<GoalOrientedAction> res=recursiveSearch(subActions,utilities, measure);
                    if (res.Count > 0)
                        res.Insert(0, best);
                    else
                        res.Add(best);
                    return res;
                }
            }

            private Dictionary<string,float> MakeDictionary()
            {
                Dictionary<string,float> dict = new Dictionary<string,float>();
                foreach(Goal goal in goals)
                {
                    dict.Add(goal.Name, goal.Utility);
                }
                return dict;
            }

            public void Step(Func<float,float> measure)
            {
                Dictionary<string, float> utilities = MakeDictionary();
                List<GoalOrientedAction> actions=recursiveSearch(this.actions,utilities,measure);
                foreach (GoalOrientedAction action in actions)
                    action.Apply();
            }
        }
    }
}
