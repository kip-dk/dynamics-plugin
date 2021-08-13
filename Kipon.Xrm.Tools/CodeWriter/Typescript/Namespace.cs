using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Typescript
{
    public class Namespace : AbstractBlock
    {
        private readonly Writer writer;

        public Namespace(Writer writer, string name) : base(0, writer, $"declare namespace {name}")
        {
            this.writer = writer;
        }


        public Module Module(string name)
        {
            return new Module(this.indent, this.writer, name);
        }

    }
}
