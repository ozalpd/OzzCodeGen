using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.AppEngines.Java;
using OzzCodeGen.AppEngines.Java.Templates;

namespace OzzCodeGen.AppEngines.Android.Templates
{
    public abstract partial class AbstractDataHelper : AbstractJavaTemplate
    {
        public AbstractDataHelper(AndroidEngine appEngine, bool isCustomFile)
            : base(appEngine)
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

        public AndroidEngine AndroidAppEngine
        {
            get
            {
                return (AndroidEngine)AppEngine;
            }
        }

        public override string GetPackage()
        {
            if (IsCustomFile)
            {
                return AndroidAppEngine.GetCustomsPackage();
            }
            else
            {
                return AndroidAppEngine.GetGeneratedsPackage();
            }
        }
    }
}
