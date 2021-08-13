using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Xrm.Tools.Configs
{
    public class Webresource
    {
        public Webresource()
        {
        }

        public Webresource(System.IO.Stream xml)
        {
            var doc = new System.Xml.XmlDocument();
            doc.Load(xml);

            this.Forms = new FormList();
            this.Forms.Namespace = doc.SelectSingleNode("/webresource/forms")?.Attributes["namespace"]?.InnerText;
            var forms = doc.SelectNodes("/webresource/forms/form");
            foreach (System.Xml.XmlNode form in forms)
            {
                this.Forms.Add(new Form
                {
                    EntityLogicalName = form.Attributes["entitylogicalname"]?.InnerText,
                    Name = form.Attributes["name"]?.InnerText,
                    Title = form.Attributes["title"]?.InnerText
                });
            }

            this.Deploys = new DeployList();
            this.Deploys.Solution = doc.SelectSingleNode("/webresource/deploys")?.Attributes["solution"]?.InnerText;
            var deploys = doc.SelectNodes("/webresource/deploys/deploy");
            foreach (System.Xml.XmlNode deploy in deploys)
            {
                this.Deploys.Add(new Deploy
                {
                    Name = deploy.Attributes["name"]?.InnerText,
                    Script = deploy.Attributes["script"]?.InnerText
                });
            }
        }

        public FormList Forms { get; set; }
        public DeployList Deploys { get; set; }
    }
}
