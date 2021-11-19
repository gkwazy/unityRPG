 using UnityEngine;

namespace RPG.Core
{

    public class ActionContoller : MonoBehaviour 
    {
        IMode currentAction;

        public void StartAction(IMode action)
        {
            if (currentAction == action) return;

            if ( currentAction != null){
                currentAction.Cancel();
            }
          
            currentAction = action;
        } 
        public void CancelCurrentAction()
        {
            StartAction(null);
        }       
    }
}