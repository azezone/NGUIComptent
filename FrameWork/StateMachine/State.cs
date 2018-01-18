using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    public class State
    {
        public string Name { get; set; }
        private Dictionary<State, List<Condition>> _connectionMap = new Dictionary<State, List<Condition>>();


        public State(string na) { Name = na; }

        /// <summary>
        /// 注册一条连接条件
        /// </summary>
        public void AddConnection(State target, Condition condition)
        {
            List<Condition> list = null;
            _connectionMap.TryGetValue(target, out list);
            if (list == null)
            {
                list = new List<Condition>();
                list.Add(condition);
                _connectionMap.Add(target, list);
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].Name.Equals(condition.Name))
                    {
                        return;
                    }
                }
                list.Add(condition);
            }
        }

        /// <summary>
        /// 设置某个条件值，实现状态跳转
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public State SetValue<T>(string name, T value)
        {
            foreach (var item in _connectionMap)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i].IsValid<T>(name))
                    {
                        item.Value[i].SetValue<T>(name, value);
                        if (item.Value[i].IsPass())
                        {
                            return item.Key;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 根据目前状态值更新自身状态
        /// </summary>
        public State Update()
        {
            foreach (var item in _connectionMap)
            {
                for (int i = 0; i < item.Value.Count; i++)
                {
                    if (item.Value[i].IsPass())
                    {
                        return item.Key;
                    }
                }
            }
            return null;
        }

        public override string ToString()
        {
            return string.Format("{0}", this.Name);
        }
    }
}