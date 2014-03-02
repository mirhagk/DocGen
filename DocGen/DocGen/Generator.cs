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
            foreach (DocLoader.TypeMember type in loader.Members.Where(m => m is DocLoader.TypeMember))
            {
                var template = new Template.HTML.TypeTemplate();
                template.Member=type;
                File.WriteAllText(Path.Combine(path, type.FullName+".html"), template.TransformText());
            }
            foreach (DocLoader.MethodMember method in loader.Members.Where(m => m is DocLoader.MethodMember))
            {
                var template = new Template.HTML.MethodTemplate();
                template.Member = method;
                File.WriteAllText(Path.Combine(path, method.FullName + ".html"), template.TransformText());
            }
        }
    }
}
