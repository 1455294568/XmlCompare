using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlCompare
{
    class Program
    {
        static List<string> xpathList = new List<string>();
        static StringBuilder diff = new StringBuilder();
        static XmlDocument doc = new XmlDocument();
        static XmlDocument doc2 = new XmlDocument();
        static void Main(string[] args)
        {
            doc.Load("new.xml");
            doc2.Load("origin.xml");

            // <?xml version="1.0" 下一个节点就是整个文档的父节点
            var node1 = doc.FirstChild.NextSibling;
            var node2 = doc2.FirstChild.NextSibling;

            StringBuilder sb = new StringBuilder();

            NodesTravel(node1, ref sb);
            //NodesTravel(node2, list2);

            xpathList.Clear();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine(diff.ToString());

            Console.ReadLine();
        }

        static void NodesTravel(XmlNode node, ref StringBuilder sb)
        {
            string xpath = sb.ToString() + node.Name;

            var cnt = xpathList.Count(s => s == xpath);
            xpathList.Add(xpath);
            if (cnt > 0)
            {
                xpath += string.Format("[{0}]", cnt + 1);
            }

            sb.Clear();
            sb.Append(xpath + "/");

            var node2 = doc2.SelectSingleNode(xpath);
            if (node2 != null)
            {
                var str = string.Format("{0}\n{1}\r\n", node.Name, node2.Name);
                Console.WriteLine(str);
                CompareNodeAttr(node, node2, xpath);
                CompareNodeText(node, node2, xpath);
            }
            else
            {
                var str = string.Format("{0}节点在文件2中不存在\r\n", xpath);
                diff.Append(str);
            }

            if (node.HasChildNodes)
            {
                foreach (XmlNode n in node.ChildNodes)
                {
                    if (n.NodeType != XmlNodeType.Element)
                    {
                        continue;
                    }

                    NodesTravel(n, ref sb);
                    var aa = sb.ToString().TrimEnd('/');
                    var index = aa.LastIndexOf("/");
                    sb.Clear();
                    sb.Append(aa.Substring(0, index) + "/");
                }
            }
        }

        static void CompareNodeAttr(XmlNode node1, XmlNode node2, string xpath)
        {
            var attrs1 = node1.Attributes;
            var attrs2 = node2.Attributes;
            foreach (XmlAttribute attr1 in attrs1)
            {
                int i = 0;
                for (; i < attrs2.Count; i++)
                {
                    var attr2 = attrs2[i];
                    if (attr1.Name == attr2.Name)
                    {
                        if (attr1.Value != attr2.Value)
                        {
                            var str = string.Format("{0} 里 {1} 的值不相同, 文件1: {2}, 文件2: {3}.\r\n", xpath, attr1.Name, attr1.Value, attr2.Value);
                            diff.Append(str);
                        }
                        break;
                    }
                }
                if (i >= attrs2.Count)
                {
                    var str = string.Format("{0} 里 {1} 在文件2中不存在.\r\n", xpath, attr1.Name);
                    diff.Append(str);
                }
            }
        }

        static void CompareNodeText(XmlNode node1, XmlNode node2, string xpath)
        {
            var text1 = string.Empty;
            var text2 = string.Empty;
            if (node1.ChildNodes.Count == 1 && node1.FirstChild.NodeType == XmlNodeType.Text)
            {
                text1 = node1.FirstChild.Value;
            }
            if (node2.ChildNodes.Count == 1 && node2.FirstChild.NodeType == XmlNodeType.Text)
            {
                text2 = node2.FirstChild.Value;
            }

            if (text1 != text2)
            {
                var str = string.Format("{0} 里的值不相同, 文件1: {1}, 文件2: {2}.\r\n", xpath, text1, text2);
                diff.Append(str);
            }
        }
    }
}
