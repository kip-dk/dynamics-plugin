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
            this.Forms.Typings = doc.SelectSingleNode("/webresource/forms")?.Attributes["typings"]?.InnerText;
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
            this.Deploys.ManagedSolution = doc.SelectSingleNode("/webresource/deploys")?.Attributes["managed-solution"]?.InnerText;

            var solutions = doc.SelectNodes("/webresource/solutions/solution");
            if (solutions != null)
            {
                var res = new List<string>();
                foreach (System.Xml.XmlNode solution in solutions)
                {
                    res.Add(solution.Attributes["name"].InnerText);
                }
                this.Deploys.Solutions = res.ToArray();
            }

            var deploys = doc.SelectNodes("/webresource/deploys/deploy");
            foreach (System.Xml.XmlNode deploy in deploys)
            {
                this.Deploys.Add(new Deploy
                {
                    Name = deploy.Attributes["name"]?.InnerText,
                    Filename = deploy.Attributes["file"]?.InnerText,
                    Description = deploy.Attributes["description"]?.InnerText
                });
            }
        }

        public FormList Forms { get; set; }
        public DeployList Deploys { get; set; }
    }
}
