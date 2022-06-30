using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Otamatone
{
    namespace Trees
    {
        public delegate bool DecisionCondition();
        public class DecisionNode
        {
            public DecisionCondition Condition { get; private set; }

            public object Left { get; private set; }

            public object Right { get; private set; }

            public DecisionNode(DecisionCondition condition,object left,object right)
            {
                Condition=condition;
                Left = left;
                Right = right;
            }
        }
        public class DecisionTree
        {
            public DecisionNode Root { get; private set; }

            public DecisionTree(DecisionNode root)
            {
                Root = root;
            }



            private void recursiveSearch(object node)
            {
                if (node is Action)
                {
                    Action action = node as Action;
                    action();
                }
                else if (node is DecisionNode)
                {
                    DecisionNode decision = node as DecisionNode;
                    if (decision.Condition())
                        recursiveSearch(decision.Left);
                    else recursiveSearch(decision.Right);
                }
                else
                {
                    throw new Exception();
                }
            }

            public void MakeDecision()
            {
                recursiveSearch(Root);
            }
        }
    }
}
