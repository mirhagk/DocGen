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
            var template = new Template.TypeTemplate();
            foreach (DocLoader.TypeMember type in loader.Members.Where(m => m is DocLoader.TypeMember))
            {
                template.Member=type;
                File.WriteAllText(Path.Combine(path, type.FullName+".html"), template.TransformText());
            }
        }
    }
}
