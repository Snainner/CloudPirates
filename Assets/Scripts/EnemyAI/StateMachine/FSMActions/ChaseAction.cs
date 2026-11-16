using UnityEngine;

namespace FSM
{
    [CreateAssetMenu(menuName = ("FSM/Actions/Chase"))]
    public class ChaseAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            //get enemy sight sensor and set the destination to the the player position 
        }
    }

}

