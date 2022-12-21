using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Typescript
{
    public class Interface : AbstractBlock
    {
        public Interface(int indent, Writer writer, string code) : base(indent, writer, $"interface {code}")
        {
        }

        public Interface Stub(string stub)
        {
            this.Writeline(stub);
            return this;
        }
    }
}
