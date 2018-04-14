using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maple.BuildTask
{
    public class TestTask : Microsoft.Build.Utilities.Task
    {
        private string outputFile;

        [Microsoft.Build.Framework.Required]
        public string OutputFile
        {
            get { return outputFile; }
            set { outputFile = value; }
        }

        public override bool Execute()
        { 
            Log.LogWarning("OutputFile = " + this.outputFile);
            return true;
        }
    }
}
