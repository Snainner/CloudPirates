using FSM;
using UnityEngine;

namespace FSM
{
    [CreateAssetMenu(menuName = ("FSM/Actions/Patrol"))]
    public class PatrolAction : FSMAction
    {
        public override void Execute(BaseStateMachine stateMachine)
        {
            //var patrolPoints = stateMachine.GetComponent<PatrolPoints>();

            //if (patrolPoints == null)
            //    Debug.Log("Patrol Action: No patrol points set");
            //if(patrolPoints.HasReached())
            //{
            //    var nextPos = patrolPoints.GetNextPos();

            //}
        }
    }
}
