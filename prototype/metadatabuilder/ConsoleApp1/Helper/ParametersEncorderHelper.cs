using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Helper
{
    public class ParametersEncorderHelper
    {
        public string[] parameterList { get; set; }
        public void ParameterAction(ParametersEncoder parameters)
        {
            foreach (var parameter in parameterList)
            {
                //todo
                if (parameter == "string")
                {
                    parameters.AddParameter().Type().String();
                }
                else if(parameter == "int")
                {
                    parameters.AddParameter().Type().Int32();
                }
            }
        }
    }
}
