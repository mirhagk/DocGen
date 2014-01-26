using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DocGen
{
    /// <summary>
    /// Loads all the date required for generating documentation
    /// </summary>
    class DocLoader
    {
        class Member
        {
            public string FullName;
            public string Summary;
            public virtual void Load(Assembly assembly)
            {
            }
        }
        class TypeMember:Member
        {
            public Type Type;
            public override void Load(Assembly assembly)
            {
                base.Load(assembly);
                Type = assembly.GetType(FullName);
            }
        }
        class MethodMember : Member
        {
            public string Name;
            public string Type;
            MethodInfo Method;
            public override void Load(Assembly assembly)
            {
                base.Load(assembly);
                Name = FullName.Split('(')[0].Split('.').Last();
                var pieces = FullName.Split('(')[0].Split('.');
                Type = string.Join(".", pieces.Take(pieces.Length - 1));
                Method = assembly.GetType(Type).GetMethod(Name);
            }
        }
        List<Member> Members = new List<Member>();
        /// <summary>
        /// The name of the assembly from the loaded file
        /// </summary>
        public string AssemblyName { get; set; }
        /// <summary>
        /// Loads the comments for an assembly and the xml file for it
        /// </summary>
        /// <param name="filename">The name of the file to load</param>
        public void Load(string filename)
        {
            var assembly = Assembly.LoadFile(System.IO.Path.GetFullPath(filename + ".exe"));
            var document = System.Xml.Linq.XDocument.Load(filename+".xml").Element("doc");
            AssemblyName = document.Element("assembly").Element("name").Value;
            foreach (var member in document.Element("members").Elements())
            {
                Member result;
                switch (member.Attribute("name").Value[0])
                {
                    case 'T':
                        result = new TypeMember();
                        break;
                    case 'M':
                        result = new MethodMember();
                        break;
                    default:
                        result = new Member();
                        Console.WriteLine("Did not understand {0}", member.Attribute("name").Value[0]);
                        break;
                }
                result.FullName = member.Attribute("name").Value.Substring(2);
                result.Summary = member.Element("summary").Value;
                result.Load(assembly);
                Members.Add(result);
            }

            System.Diagnostics.Debug.Assert(assembly.GetName().Name == AssemblyName);
        }
    }
}
