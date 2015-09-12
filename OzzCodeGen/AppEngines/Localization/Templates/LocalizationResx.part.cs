using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Templates;
using OzzUtils;
using OzzLocalization;

namespace OzzCodeGen.AppEngines.Localization.Templates
{
    public partial class LocalizationResx : AbstractTemplate
    {
        public LocalizationResx(LocalizationEntitySetting entitySetting)
        {
            EntitySetting = entitySetting;
        }

        public LocalizationResx(LocalizationEntitySetting entitySetting, Vocabulary vocabulary)
        {
            EntitySetting = entitySetting;
            Vocabulary = vocabulary;
        }

        public Vocabulary Vocabulary { get; private set; }

        public LocalizationEntitySetting EntitySetting
        {
            get { return _entitySetting; }
            set
            {
                _entitySetting = value;
            }
        }
        LocalizationEntitySetting _entitySetting;

        protected string ModelName
        {
            get
            {
                return EntitySetting.Name;
            }
        }

        protected string ModelTitle
        {
            get
            {
                return GetTranslation(EntitySetting.Name);
            }
        }

        protected string GetPropertyLabel(LocalizationPropertySetting property)
        {
            return GetTranslation(property.Name);
        }

        public string GetTranslation(string name)
        {
            Vocab vocab = null;
            if (Vocabulary != null)
            {
                vocab = Vocabulary
                        .FirstOrDefault(var => var.Name == name);

                if (vocab == null && name.EndsWith("Id", StringComparison.InvariantCultureIgnoreCase))
                {
                    string s = name.Substring(0, name.Length - 2);
                    vocab = Vocabulary.FirstOrDefault(v => v.Name == s);

                    if (vocab != null)
                        return vocab.Translation;// +" ID";
                }
                else if (vocab == null && (name.EndsWith("X") || name.EndsWith("Y") || name.EndsWith("Z")))
                {
                    string s = name.Substring(0, name.Length - 1);
                    vocab = Vocabulary.FirstOrDefault(v => v.Name == s);
                    if (vocab != null)
                    {
                        string end = name.Substring(name.Length - 1, 1);
                        return string.Format("{0} {1}", vocab.Translation, end);
                    }
                }
            }

            if (vocab != null)
            {
                return vocab.Translation;
            }
            else
            {
                return name.PascalCaseToTitleCase();
            }
        }

        public override string GetDefaultFileName()
        {
            if (Vocabulary == null)
            {
                return EntitySetting.Name + "String.Resx";
            }
            else
            {
                return string.Format("{0}String.{1}.Resx", EntitySetting.Name, Vocabulary.CultureCode);
            }
        }

        protected string GetPropertyRequiredMsg(LocalizationPropertySetting property)
        {
            return string.Format(RequiredMsg, GetPropertyLabel(property));
        }

        protected string GetPropertyValidationMsg(LocalizationPropertySetting property)
        {
            return string.Format(ValidationMsg, GetPropertyLabel(property));
        }

        protected string RequiredMsg
        {
            get
            {
                if (string.IsNullOrEmpty(_requiredMsg))
                    _requiredMsg = GetRequiredMsg();
                return _requiredMsg;
            }
        }
        string _requiredMsg;

        private string GetRequiredMsg()
        {
            Vocab vocab = null;
            if (Vocabulary != null)
            {
                vocab = Vocabulary
                        .FirstOrDefault(var => var.Name == "RequiredMsg");
            }
            if (vocab == null)
            {
                return "Please enter the information for {0} field!";
            }
            else
            {
                return vocab.Translation;
            }
        }

        protected string ValidationMsg
        {
            get
            {
                if (string.IsNullOrEmpty(_validationMsg))
                    _validationMsg = GetValidationMsg();
                return _validationMsg;
            }
        }
        string _validationMsg;

        private string GetValidationMsg()
        {
            Vocab vocab = null;
            if (Vocabulary != null)
            {
                vocab = Vocabulary
                        .FirstOrDefault(var => var.Name == "ValidationMsg");
            }
            if (vocab == null)
            {
                return "Please correct the information in {0} field!";
            }
            else
            {
                return vocab.Translation;
            }
        }
    }
}
