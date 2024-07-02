using Kipon.Xrm.Extensions.Strings;
using System;
using System.Activities.Presentation.Services;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kipon.Solid.Plugin.Plugins.ActivitySubject
{
    public class ActivitySubjectPlugin : Kipon.Xrm.BasePlugin
    {
        public void OnPreCreate(Model.IActivitySubject target)
        {
            this.Handle(target);
        }

        public void OnPreUpdate(Model.IActivitySubject mergedimage)
        {
            this.Handle(mergedimage);
        }

        private void Handle(Model.IActivitySubject subject)
        {
            subject.EkstraSubject = $"{ subject.Subject }:{ subject.EkstraSubject?.Replace((subject.Subject ?? "#Empty#"),"") }".MaxLength(400);
        }
    }
}
