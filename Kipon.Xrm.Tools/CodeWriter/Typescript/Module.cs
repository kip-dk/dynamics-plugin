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

        public Module(Writer writer, string name): base(0, writer, $"module {name}")
        {
            this.writer = writer;
            this.indent = 0;
        }

        public Module(int indent, Writer writer, string name) : base(indent, writer, $"module {name}")
        {
            this.writer = writer;
            this.indent = indent;
        }

        public Interface Interface(string name)
        {
            return new Interface(this.indent, this.writer, name);
        }
    }
}
