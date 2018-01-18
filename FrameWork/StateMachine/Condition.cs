using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StateMachine
{
    public class Condition
    {
        public string Name { get; set; }
        private object _origin;
        private object _target;
        private Type _type;

        public Condition(string name, Type type, object o1, object o2)
        {
            this.Name = name;
            this._type = type;
            this._origin = o1;
            this._target = o2;
        }

        public static Condition Create<T>(string name, T origin, T target)
        {
            Condition con = new Condition(name, origin.GetType(), origin, target);
            return con;
        }

        public bool IsValid<T>(string name)
        {
            if (this.Name.Equals(name) && _type.Equals(typeof(T)))
            {
                return true;
            }
            return false;
        }

        public void SetValue<T>(string name, T value)
        {
            if (IsValid<T>(name))
            {
                _origin = value;
            }
        }

        public bool IsPass()
        {
            return _origin.Equals(_target);
        }
    }
}