using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.AppEngines.ObjectiveC.Templates
{
    public abstract partial class AbstractObjcTemplate : OzzCodeGen.Templates.AbstractUnixTemplate
    {
        public AbstractObjcTemplate(ObjcEngine appEngine)
        {
            _appEngine = appEngine;
        }

        public AbstractObjcTemplate(ObjcEntitySetting entity)
        {
            Entity = entity;
        }

        public ObjcEngine AppEngine
        {
            get
            {
                if (_appEngine == null & Entity != null)
                {
                    _appEngine = Entity.AppEngine;
                }
                return _appEngine;
            }
        }
        ObjcEngine _appEngine;

        public ObjcEntitySetting Entity { get; private set; }
        public string ObjectiveCName
        {
            get
            {
                if (string.IsNullOrEmpty(_objectiveCName))
                {
                    _objectiveCName = Entity.ObjectiveCName;
                }
                return _objectiveCName;
            }
            set { _objectiveCName = value; }
        }
        string _objectiveCName;

        public List<string> Imports
        {
            get
            {
                if (_imports == null)
                    _imports = GetDefaultImports();
                return _imports;
            }
            set { _imports = value; }
        }
        private List<string> _imports;
        
        protected virtual List<string> GetDefaultImports()
        {
            return new List<string>();
        }
    }
}
