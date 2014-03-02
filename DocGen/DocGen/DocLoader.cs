using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DocGen
{
    /// <summary>
    /// Loads all the data required for generating documentation
    /// </summary>
    public class DocLoader
    {
        private static XElement GetDocumentation(XElement root, Member member)
        {
            var elements = root.Element("members").Elements().Where(e => e.Attribute("name") != null);
            var element = elements.FirstOrDefault(e=>e.Attribute("name").Value==("T:" + member.FQN));
            return element;
        }
        public class Member
        {
            public string FullName;
            public string FQN;
            public string Summary;
            public XElement Documentation;
            public virtual void Load(XElement documentation)
            {
                var element = GetDocumentation(documentation, this);
                Documentation = element;
                if (element == null)
                    Summary = null;
                else
                    Summary = element.Element("summary").Value;
            }
            protected Type LoadType(string FullName, Assembly assembly)
            {
                var type = assembly.GetType(FullName);
                if (type == null)
                {
                    type = assembly.GetTypes().FirstOrDefault(t => t.FullName.Replace('+', '.') == FullName);
                }
                if (type == null)
                    type = Type.GetType(FullName);
                return type;
            }
        }
        /// <summary>
        /// Represents a type (class), with all of the required fields for documentation generation
        /// </summary>
        public class TypeMember : Member
        {
            public Type Type;
            public List<TypeMember> NestedTypes = new List<TypeMember>();
            public List<MethodMember> Methods = new List<MethodMember>();
            public TypeMember(Type type)
            {
                this.FullName = type.FullName;
                this.Type = type;
                this.FQN = this.FullName.Replace('+', '.');
            }
            public override void Load(XElement documentation)
            {
                base.Load(documentation);
                foreach (Type nestedType in Type.GetNestedTypes(BindingFlags.DeclaredOnly|BindingFlags.Public))
                {
                    NestedTypes.Add(new TypeMember(nestedType));
                }
                foreach (var nestedType in NestedTypes)
                    nestedType.Load(documentation);
            }
        }
        public class MethodMember : Member
        {
            public string Name;
            public Type Type;
            MethodInfo Method;
            /*
            public override void Load(Assembly assembly, XElement documentation)
            {
                base.Load(assembly,documentation);
                Name = FullName.Split('(')[0].Split('.').Last();
                var pieces = FullName.Split('(')[0].Split('.');

                Type = LoadType(string.Join(".", pieces.Take(pieces.Length - 1)),assembly);


                Type[] typeArray = new Type[] { };
                if (FullName.Contains('('))
                    typeArray = FullName.TrimEnd(')').Split('(')[1].Split(',').Select(t => LoadType(t, assembly)).ToArray();
                Method = Type.GetMethod(Name,typeArray);
            }*/
        }
        public List<Member> Members = new List<Member>();
        public IEnumerable<Member> AllMembers
        {
            get
            {
                return Members.SelectMany(m => AllChildMembers(m)).Union(Members);
            }
        }
        private IEnumerable<Member> AllChildMembers(Member member)
        {
            var typeMember = member as TypeMember;
            if (typeMember != null)
            {
                foreach (var nestedType in typeMember.NestedTypes)
                    yield return nestedType;
            }
            yield break;
        }
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
            var document = XDocument.Load(filename + ".xml").Element("doc");
            foreach (var type in assembly.GetTypes().Where(t=>t.DeclaringType==null))
            {
                Console.WriteLine(type);
                var typeMember = new TypeMember(type);
                Members.Add(typeMember);
            }
            foreach (var member in Members) member.Load(document);
            /*
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

            System.Diagnostics.Debug.Assert(assembly.GetName().Name == AssemblyName);*/
        }
    }
}
