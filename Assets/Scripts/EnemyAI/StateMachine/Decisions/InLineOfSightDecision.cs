using UnityEngine;

namespace FSM
{
    public class InLineOfSightDecision : Decision
    {
        public override bool Decide(BaseStateMachine state)
        {
            return state.IsPlayerSighted; 
        }
    }
}


