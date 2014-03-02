using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DocGen
{
    /// <summary>
    /// Generator provides methods to generate documentation
    /// </summary>
    class Generator
    {
        public void ClearDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return;
            }
            foreach (var file in Directory.EnumerateFiles(path))
                File.Delete(file);
            foreach (var directory in Directory.EnumerateDirectories(path))
                Directory.Delete(directory);
        }
        public void Generate(string path, DocLoader loader)
        {
            path = Path.GetFullPath(path);
            ClearDirectory(path);
            foreach (DocLoader.TypeMember type in loader.AllMembers.Where(m => m is DocLoader.TypeMember))
            {
                var template = new Template.HTML.TypeTemplate();
                template.Member=type;
                File.WriteAllText(Path.Combine(path, type.SafeName + ".html"), template.TransformText());
            }
            foreach (DocLoader.MethodMember method in loader.AllMembers.Where(m => m is DocLoader.MethodMember))
            {
                var template = new Template.HTML.MethodTemplate();
                template.Member = method;
                File.WriteAllText(Path.Combine(path, method.SafeName + ".html"), template.TransformText());
            }
        }
    }
}
