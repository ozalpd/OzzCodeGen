using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.CodeEngines.Java;
using OzzCodeGen.CodeEngines.Java.Templates;

namespace OzzCodeGen.CodeEngines.Android.Templates
{
    public abstract partial class AbstractDataHelper : AbstractJavaTemplate
    {
        public AbstractDataHelper(AndroidEngine codeEngine, bool isCustomFile)
            : base(codeEngine)
        {
            IsCustomFile = isCustomFile;
        }

        public AbstractDataHelper(JavaEntitySetting entity, bool isCustomFile)
            : base(entity)
        {
            IsCustomFile = isCustomFile;
        }


        public string BaseClassName { protected set; get; }
        public bool IsCustomFile { protected set; get; }

        public AndroidEngine AndroidCodeEngine
        {
            get
            {
                return (AndroidEngine)CodeEngine;
            }
        }

        public override string GetPackage()
        {
            if (IsCustomFile)
            {
                return AndroidCodeEngine.GetCustomsPackage();
            }
            else
            {
                return AndroidCodeEngine.GetGeneratedsPackage();
            }
        }
    }
}
