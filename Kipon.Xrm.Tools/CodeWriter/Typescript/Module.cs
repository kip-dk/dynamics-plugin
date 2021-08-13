using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Typescript
{
    public class Module : AbstractBlock
    {
        private readonly Writer writer;

        public Module(int indent, Writer writer, string name): base(indent, writer, $"module {name}")
        {
            this.writer = writer;
        }

        public Interface Interface(string name)
        {
            return new Interface(this.indent+1, this.writer, name);
        }
    }
}
