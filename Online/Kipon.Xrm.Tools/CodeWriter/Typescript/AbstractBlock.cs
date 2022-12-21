using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.CodeWriter.Typescript
{
    public abstract class AbstractBlock : IDisposable
    {
        protected int indent { get; set; }
        private readonly Writer writer;

        public AbstractBlock(int indent, Writer writer, string code)
        {
            this.indent = indent;
            this.writer = writer;
            this.Writeline(code + " {");
            this.indent++;
        }


        protected void Writeline(string code)
        {
            for (var i=0;i<this.indent;i++)
            {
                this.writer.writer.Write("    ");
            }
            this.writer.writer.WriteLine(code?.Replace("'","\""));
        }


        public void Dispose()
        {
            this.indent--;
            for (var i = 0; i < (this.indent); i++)
            {
                this.writer.writer.Write("    ");
            }
            this.writer.writer.WriteLine("}");
            this.writer.writer.WriteLine("");
        }
    }
}
