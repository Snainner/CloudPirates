using UnityEngine;

namespace FSM
{
    public class Transition : MonoBehaviour
    {
        public Decision Decision;
        public BaseState TrueState;
        public BaseState FalseState;

        public void Execute(BaseStateMachine stateMachine)
        {
            if (Decision.Decide(stateMachine) && !(TrueState is RemainInState))
                stateMachine.CurrentState = TrueState;
            else if(!(FalseState is RemainInState))
                stateMachine.CurrentState = FalseState;
        }
    }
}
