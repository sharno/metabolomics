using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using Mark.Tidy;

namespace SBWModelParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() != 3)
            {
                Console.WriteLine("Incorrect Usage!");
                Console.WriteLine("Correct Usage: SBWModelParser.exe <SOURCE_DIRECTORY> <DESTINATION_DIRECTORY> <LEVELVERSION>");
                Console.WriteLine(@"e.g. SBWModelParser.exe C:\sbml-test-cases-2011-06-15 C:\models l2v4");
                return;
            }

            var srcDir = args[0];   // directory for the source SBML files to parse.
            var destDir = args[1];  // directory to copy fixed SBML files.
            var levelVersion = args[2]; // level and version of the SBML files to copy from SBW dataset.
            var sbmlFileNameMatchingPattern = String.Format("*{0}.xml", levelVersion);  // SBML file name convention.
            var descriptionFileNameMatchingPattern = "*-model.html";    // HTML file name convention.

            if (!Directory.Exists(srcDir))
            {
                Console.WriteLine("Source directory {0} not found", srcDir);
                return;
            }
            if (!Directory.Exists(destDir))
            {
                try
                {
                    Directory.CreateDirectory(destDir);
                    Console.WriteLine("Destination directory {0} has been created successfully", destDir);
                }
                catch
                {
                    Console.WriteLine("Destination directory {0} cannot be created. Permission denied.", destDir);
                    Console.WriteLine("Create destination directory in your computer and then run SBWModelParser");
                    return;
                }
            }

            var modelFileNames = Directory.GetFiles(srcDir, sbmlFileNameMatchingPattern, SearchOption.AllDirectories);

            if (modelFileNames.Count() == 0)
            {
                Console.WriteLine("There is not any model which matches {0} in {1}", levelVersion, srcDir);
                return;
            }

            foreach (var fileName in modelFileNames)
            {
                FileInfo f = new FileInfo(fileName);
                var content = File.ReadAllText(fileName);

                var descriptionFileNames = Directory.GetFiles(f.DirectoryName, descriptionFileNameMatchingPattern, SearchOption.TopDirectoryOnly);
                if (descriptionFileNames.Count() == 1)
                {
                    // append description into sbml just after <model> tag.
                    var modelTagIndex = content.IndexOf("<model");
                    var modelTagClosingIndex = content.IndexOf(">", modelTagIndex);
                    
                    // read notes from description file.
                    var notes = File.ReadAllText(descriptionFileNames[0]);


                    try
                    {
                        Document doc = new Document(notes);
                        doc.CleanAndRepair();
                        var fixedDoc = doc.Save();
                        
                        // Multiple character conversions for SBML Parser.
                        // Please loook at table http://htmlhelp.com/reference/html40/entities/symbols.html for reference.
                        fixedDoc = fixedDoc.Replace("&larr;", "&#8592;");
                        fixedDoc = fixedDoc.Replace("&uarr;", "&#8593;");
                        fixedDoc = fixedDoc.Replace("&rarr;", "&#8594;");
                        fixedDoc = fixedDoc.Replace("&darr;", "&#8595;");
                        fixedDoc = fixedDoc.Replace("&harr;", "&#8596;");
                        
                        fixedDoc = fixedDoc.Replace("&rlm;", "");
                        fixedDoc = fixedDoc.Replace("&lrm;", "");
                        fixedDoc = fixedDoc.Replace("&ge;", "&#8805;");
                        fixedDoc = fixedDoc.Replace("&le;", "&#8804;");
                        fixedDoc = fixedDoc.Replace("&minus;", "&#8722;");

                        // these special characters cause problem on Simulation Engine(rr).
                        fixedDoc = fixedDoc.Replace("&middot;", "."); // middot does not work with Simulation Engine, therefore single dot is used.
                        fixedDoc = fixedDoc.Replace("&times;", "x"); // times does not work with Simulation Engine, therefore x character is used.
                        
                        // Retrieve the notes part which is in <body> tags of fixed document.
                        var bodyStartIndex = fixedDoc.IndexOf("<body>") + 6;
                        var bodyEndIndex = fixedDoc.IndexOf("</body>");
                        fixedDoc = fixedDoc.Substring(bodyStartIndex, bodyEndIndex - bodyStartIndex);
                        content = content.Insert(modelTagClosingIndex + 1, String.Format("<notes>{0}</notes>", fixedDoc));
                    }
                    catch
                    {
                        Console.WriteLine("Incorrect notes tag for {0} model.", f.Name);
                    }
                }

                // add sbml to destdirectory
                File.WriteAllText(destDir + "\\" + f.Name, content);
                Console.WriteLine("Model {0} is copied", f.Name);
            }
        }
    }
}
