using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;
using java.io;
using System;
using System.Text;
using System.Xml;

namespace NarutoBot3
{
    public class Questions
    {
        LexicalizedParser lp;

        public Questions(){

            // Loading english PCFG parser from file
            lp = LexicalizedParser.loadModel(@"models\lexparser\englishPCFG.ser.gz");
        }

        public string questionParser(string question)
        {
            if (lp == null)
            {
                lp = LexicalizedParser.loadModel(@"models\lexparser\englishPCFG.ser.gz");
            }
            string subjectNPL = string.Empty;

            var tokenizerFactory = PTBTokenizer.factory(new CoreLabelTokenFactory(), "");
            var sent2Reader = new StringReader(question);
            var rawWords2 = tokenizerFactory.getTokenizer(sent2Reader).tokenize();
            sent2Reader.close();
            var tree = lp.apply(rawWords2);

            

            //var rawWords = Sentence.toCoreLabelList(tokens);
            //var tree = lp.apply(rawWords);
            //tree.pennPrint();

            // Extract dependencies from lexical tree
            var tlp = new PennTreebankLanguagePack();
            var gsf = tlp.grammaticalStructureFactory();
            var gs = gsf.newGrammaticalStructure(tree);
            
            var tdl = gs.typedDependenciesCCprocessed();
            //System.Console.WriteLine("\n{0}\n", tdl);

            var tp = new TreePrint("xmlTree");

            PrintWriter p = new PrintWriter("parse.xml", "UTF-8");
            tp.printTree(tree, p);

            BufferedReader br = new BufferedReader(new FileReader("parse.xml"));
            String xmlS;
            try
            {
                StringBuilder sb = new StringBuilder();
                String line = br.readLine();

                while (line != null)
                {
                    sb.Append(line);
                    sb.Append('\r');
                    line = br.readLine();
                }
                xmlS = sb.ToString();
            }
            finally
            {
                br.close();
            }



            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlS);

            XmlNodeList xnList = xml.SelectNodes("//*");


            XmlNode npTree = null;

            foreach (XmlNode xn in xnList)
            {
                XmlAttributeCollection ac = xn.Attributes;

                for (int i = 0; i < ac.Count; i++)
                {
                    if (ac["value"].InnerText == "NP")
                    {
                        if (npTree == null)
                            npTree = xn;
                        break;
                    }
                }
                if (npTree != null) break;

            }

            if (npTree != null)
            {
                XmlNodeList words = npTree.SelectNodes(".//*");

                foreach (XmlNode xn in words)
                {
                    if (xn.Name == "leaf")
                    {
                        XmlAttributeCollection ac = xn.Attributes;

                        for (int i = 0; i < ac.Count; i++)
                        {
                            if (xn.ParentNode.Attributes["value"].InnerText == "," || ac["value"].InnerText == "_" || ac["value"].InnerText == "'" || ac["value"].InnerText == "'s")
                                subjectNPL = subjectNPL.Trim();


                            subjectNPL += ac["value"].InnerText + " ";
                        }
                    }
                }
            }
            return subjectNPL.Trim();
        }
    }
}
