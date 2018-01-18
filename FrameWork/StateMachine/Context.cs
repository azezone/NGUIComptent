using System.Collections.Generic;
using UnityEngine;

namespace StateMachine
{
    public class Context
    {
        private Dictionary<string, State> _stateDict = new Dictionary<string, State>();
        private State _current;

        public void SetOriginState(State state)
        {
            this.AddState(state);
            _current = state;
        }

        /// <summary>
        /// 新增某个状态
        /// </summary>
        /// <param name="sname"></param>
        /// <param name="state"></param>
        private void AddState(State state)
        {
            if (state != null && !_stateDict.ContainsKey(state.Name))
            {
                _stateDict.Add(state.Name, state);
            }
        }

        /// <summary>
        /// 注册一条连接条件
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="con"></param>
        public void AddConnection(State A, State B, Condition con)
        {
            this.AddState(A);
            this.AddState(B);
            A.AddConnection(B, con);
        }

        /// <summary>
        /// 设置某个条件值，实现状态跳转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetValue<T>(string name, T value)
        {
            foreach (var item in _stateDict)
            {
                item.Value.SetValue<T>(name, value);
            }

            if (_current != null)
            {
                State state = _current;
                while (state != null)
                {
                    state = state.Update();
                    if (state != null)
                    {
                        _current = state;
                        Debug.Log("state change to :" + _current.ToString());
                    }
                }
            }
        }
    }
}
