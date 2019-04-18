using IniParser.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Automaton.Utils
{
    public class DynamicIniData : DynamicObject
    {
        private IniData value;

        public DynamicIniData(IniData value)
        {
            this.value = value;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = new SectionData(value[binder.Name]);
            return true;
        }
    }

    class SectionData : DynamicObject
    {
        private KeyDataCollection _coll;

        public SectionData(KeyDataCollection coll)
        {
            this._coll = coll;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = _coll[binder.Name];
            return true;
        }
    }
    
}
