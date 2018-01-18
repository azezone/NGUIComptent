using UnityEngine;

namespace StateMachine
{
    public class TestStateMachine : MonoBehaviour
    {
        private Context _context;
        private void Start()
        {
            _context = new Context();
            State s1 = new State("state1");
            State s2 = new State("state2");
            State s3 = new State("state3");
            State s4 = new State("state4");

            Condition c1 = new Condition("condition1", typeof(bool), false, true);
            Condition c2 = new Condition("condition2", typeof(int), 0, 1);
            Condition c3 = new Condition("condition3", typeof(int), 0, 2);
            Condition c4 = new Condition("condition4", typeof(int), 0, 3);

            _context.AddConnection(s1, s2, c1);
            _context.AddConnection(s2, s3, c2);
            _context.AddConnection(s3, s4, c3);
            _context.AddConnection(s4, s1, c4);
            _context.SetOriginState(s1);

        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                _context.SetValue<bool>("condition1", true);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                _context.SetValue<int>("condition2", 1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                _context.SetValue<int>("condition3", 2);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                _context.SetValue<bool>("condition1", false);
                _context.SetValue<int>("condition4", 3);
            }
        }
    }
}
