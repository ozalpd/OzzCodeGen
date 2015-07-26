using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.Providers.SourceCode
{
    /// <summary>
    /// Keeps regions of source code
    /// </summary>
    public class SourceCodeBlock
    {
        public SourceCodeBlock(BaseCodeParser parser)
        {
            InnerCodeBlocks = new List<SourceCodeBlock>();
            Parser = parser;
        }
        public SourceCodeBlock(BaseCodeParser parser, SourceCodeBlock parent)
        {
            InnerCodeBlocks = new List<SourceCodeBlock>();
            Parser = parser;
            ParentCodeBlock = parent;
        }

        
        public bool HasParent { get { return ParentCodeBlock != null; } }

        public string InnerText
        {
            get { return _innerText; }
            set
            {
                _innerText = value;
                Parser.ParseInnerCodeBlocks(this);
            }
        }
        private string _innerText;

        public SourceCodeBlock AddChildBlock(string innerText)
        {
            if (InnerText.Equals(innerText)) return null;

            var child = new SourceCodeBlock(Parser, this);
            InnerCodeBlocks.Add(child);
            child.InnerText = innerText;

            return child;
        }
        public List<SourceCodeBlock> InnerCodeBlocks { get; private set; }
        public SourceCodeBlock ParentCodeBlock { get; set; }
        public BaseCodeParser Parser { get; set; }

        public virtual string GetValueByKey(string key)
        {
            return Parser.GetValueByKey(this, key)
                        .Replace("\"", "");
        }

        public virtual SourceCodeBlock GetChildByKey(string key)
        {
            var child = Parser.GetChildByKey(this, key);
            if (child == null)
            {
                foreach (var item in this.InnerCodeBlocks)
                {
                    child = item.GetChildByKey(key);
                    if (child != null) break;
                }
            }
            return child;
        }


        public override string ToString()
        {
            return InnerText;
        }
    }
}
