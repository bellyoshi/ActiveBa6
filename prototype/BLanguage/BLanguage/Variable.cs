using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLanguage
{
    public  class Variable : IComparable<Variable>
    {
        public static Variable VOID = new Variable(new object());
        private object _value;
        public Variable()
        {
            _value = new object();
        }

        public Variable(object value)
        {
            if(value == null)
            {
                throw new Exception("Value cannot be null");
            }
            _value = value;
            if (!IsString() && !IsNumber() && !IsBoolean() && !IsDouble())
            {
                throw new Exception("value must be a string, int, bool or double");
            }
        }

        public bool IsBoolean()
        {
            return _value is bool;
        }

        public bool IsDouble()
        {
            return _value is double;
        }

        public bool ToBoolean()
        {
            bool.TryParse(_value.ToString(), out var result);
            return result;
        }

        public override string ToString()
        {
            return _value?.ToString() ;
        }

        public double ToDouble()
        {
            return double.Parse(_value.ToString());
        }

        public bool IsNumber()
        {
            return double.TryParse(ToString(), out var result);
        }

        public bool IsString()
        {
            return _value is string;
        }

        public override bool Equals(object? other)
        {
            if(this == VOID || other == VOID )
            {
                throw new Exception("can't use VOID: " + this + "==/!= " + other);

            }
            if(this == other)
            {
                return true;
            }
            if(other == null || this.GetType() != other.GetType())
            {
                return false;
            }
            var that = (Variable)other;
            if(this.IsNumber() && that.IsNumber())
            {
                double diff = Math.Abs(this.ToDouble() - that.ToDouble());
                return diff < 0.001;
            }
            else
            {
                return this._value.Equals(that._value);
            }
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();

        }
        public int CompareTo(Variable other)
        {

        }
    }
}
