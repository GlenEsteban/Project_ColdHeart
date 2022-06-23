using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace coldheart_combat {
    public class Combat : MonoBehaviour {
        bool isGuarding;
        
        public void SetIsGuarding(bool state) {
            isGuarding = state;
        }
    }
}
