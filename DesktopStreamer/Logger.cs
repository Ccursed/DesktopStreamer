using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesktopStreamer
{
    public delegate void OnAddHandler(string line);

    public class Logger : List<string>
    {
        public event OnAddHandler onAdd;

        public Logger() : base()
        {
        }

        public new void Add(string toAdd)
        {
            base.Add(toAdd);
            if(onAdd != null) onAdd(toAdd);
        }
    }
}
