using edu.stanford.nlp.parser.lexparser;
using edu.stanford.nlp.process;
using edu.stanford.nlp.trees;
using java.io;
using System;
using System.Text;

namespace NarutoBot3
{
    public class Questions
    {
        LexicalizedParser lp;

        public Questions(){

            // Loading english PCFG parser from file
            lp = LexicalizedParser.loadModel(@"models\lexparser\englishPCFG.ser.gz");
        }

        public string questionParser(string question, string user)
        {
            if (lp == null)
            {
                
            }

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
            String everything;
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
                everything = sb.ToString();
            }
            finally
            {
                br.close();
            }


            return everything;
        }
    }
}
