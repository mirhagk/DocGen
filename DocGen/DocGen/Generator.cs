using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DocGen
{
    class Generator
    {
        public void Generate(string path, DocLoader loader)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            foreach (DocLoader.Member type in loader.Members)
            {
                var template = new Templates.HtmlTemplate();
                template.Member=type;
                File.WriteAllText(Path.Combine(path, type.FullName+".html"), template.TransformText());
            }
        }
    }
}
