using Kipon.Xrm.Extensions.TypeConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Entities
{
    public partial class kipon_vetest
    {
        public static string[] names = new string[]
        {
            "Kipon ApS",
            "Portalworkers ApS",
            "Atea A/S",
            "Arbodania",
            "Decard AG",
            "Energinet A/S",
            "HOFOR A/S",
            "ChangeGroup ApS",
            "Emagine A/S",
            "Leman A/S",
            "GN Store Nord A/S",
            "Novo Nordisk A/S",
            "Nilfisk A/S",
            "DR",
            "TV2",
            "Børnehuset Aske",
        };

        private static kipon_vetest[] data;

        public static kipon_vetest[] Testdata()
        {
            if (data == null)
            {
                var result = new List<kipon_vetest>();


                for (var i = 1; i <= 100; i++)
                {
                    var ix = 1;
                    foreach (var name in names)
                    {
                        var next = new kipon_vetest
                        {
                            kipon_vetestId = i.ToGuid(ix),
                            kipon_accountid = new Microsoft.Xrm.Sdk.EntityReference(Entities.Account.EntityLogicalName, new Guid("CB5AA321-B543-E711-A962-000D3A27D441")),
                            kipon_date = System.DateTime.UtcNow.AddDays(i - 50),
                            kipon_name = $"{name} ({ix}) ({i})",
                            kipon_decimalfield = ((decimal)(i * ix) + 34M) / (i + ix),
                            kipon_optionsetfield = new Microsoft.Xrm.Sdk.OptionSetValue(ix % 4),
                            kipon_truefalse = (ix * i) % 3 == 0,
                            kipon_os = new Microsoft.Xrm.Sdk.OptionSetValue(i % 2),
                            // multi option set is causing break on prem solutions. kipon_multioptionfield = Col(i)
                        };
                        result.Add(next);
                        ix++;
                    }
                }
                data = result.ToArray();
            }
            return data;
        }

        public static Microsoft.Xrm.Sdk.OptionSetValueCollection Col(int i)
        {
            var result = new Microsoft.Xrm.Sdk.OptionSetValueCollection();

            var vals = (i % 4);
            for (var t = 0; t < vals; t++)
            {
                result.Add(new Microsoft.Xrm.Sdk.OptionSetValue(t));
            }

            return result;
        }
    }
}
