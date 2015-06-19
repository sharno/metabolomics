using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using PathwaysLib.ServerObjects;
using System.Data.SqlTypes;
using libsbmlcs;
namespace PathwaysLib.SBMLParser
{

   
    public static class AnnotationParser
    {    
        public static void Parse()
        {

                ServerModel[] models = ServerModel.AllModels();
                int index=0;
                foreach (ServerModel serverModel in models)
                {
                    Guid modelId = Guid.Empty;
                    String modelName = null;
                    String notes = null;
                    DateTime creationDate = new DateTime(1900, 1,1);
                    DateTime modificationDate = new DateTime(1900, 1, 1);
                    List<ServerAuthor> creatorList = null;
                    String pubmedId = null;
                    

                    SBMLReader reader = new SBMLReader();
                    SBMLDocument doc = reader.readSBMLFromString(serverModel.SbmlFile);
                    Model model = doc.getModel();
                    XMLNode annotation = model.getAnnotation();
                    ModelHistory provanence = RDFAnnotationParser.parseRDFAnnotation(annotation);
                    if (provanence == null) {
                        Console.WriteLine("Null"+index);
                        continue;
                    }
                    index++;
                    modelId = serverModel.ID;
                    modelName = serverModel.Name;

                    if (model != null)
                    {
                        XmlDocument docx = new XmlDocument();
                        docx.LoadXml(serverModel.SbmlFile);
                        XmlNode describedBy = docx.GetElementsByTagName("bqmodel:isDescribedBy")[0];
                        if (describedBy != null)
                        {
                            pubmedId = describedBy.FirstChild.FirstChild.Attributes["rdf:resource"].Value;
                            if (pubmedId != null)
                            {
                                pubmedId = pubmedId.Substring(pubmedId.LastIndexOf(':') + 1);
                            }
                        }

                        notes = model.getNotesString();

                        DateTime min = DateTime.Parse("1/1/1753 12:00:00AM");
                        DateTime max = DateTime.Parse("12/31/9999 11:59:59PM");
                        if (provanence.getCreatedDate() != null)
                        {
                            creationDate = new DateTime((int)provanence.getCreatedDate().getYear(), (int)provanence.getCreatedDate().getMonth(), (int)provanence.getCreatedDate().getDay(),
                                 (int)provanence.getCreatedDate().getHour(), (int)provanence.getCreatedDate().getMinute(), (int)provanence.getCreatedDate().getSecond());
                            if (!(creationDate > min && creationDate < max))
                            {
                                creationDate = new DateTime(1900, 1, 1);
                            }
                        }

                        if (provanence.getModifiedDate() != null)
                        {
                            modificationDate = new DateTime((int)provanence.getModifiedDate().getYear(), (int)provanence.getModifiedDate().getMonth(), (int)provanence.getModifiedDate().getDay(),
                                (int)provanence.getModifiedDate().getHour(), (int)provanence.getModifiedDate().getMinute(), (int)provanence.getModifiedDate().getSecond());
                            if (!(modificationDate > min && modificationDate < max))
                            {
                                modificationDate = new DateTime(1900, 1, 1);
                            }
                        }

                        ServerModelMetadata meta = new ServerModelMetadata();
                        int parsedPubmedId;
                        meta.AddModelMetaData(modelId, modelName, creationDate, modificationDate, notes, (Int32.TryParse(pubmedId, out parsedPubmedId)) ? parsedPubmedId : -1);

                        int creatorCount = (int)provanence.getNumCreators();
                        if (creatorCount != 0)
                        {
                            creatorList = new List<ServerAuthor>();
                            for (int i = 0; i < creatorCount; i++)
                            {
                                Guid authorId = Guid.NewGuid();
                                ServerAuthor sa = new ServerAuthor();
                                if (!(provanence.getCreator(i).getGivenName() == "" && provanence.getCreator(i).getFamilyName() == "" && provanence.getCreator(i).getEmail() == "" && provanence.getCreator(i).getOrganization() == ""))
                                {
                                    authorId = sa.AddAuthor(authorId, provanence.getCreator(i).getGivenName(), provanence.getCreator(i).getFamilyName(), provanence.getCreator(i).getEmail(), provanence.getCreator(i).getOrganization());
                                    ServerDesignedBy designed = new ServerDesignedBy();
                                    designed.AddDesignedBy(Guid.NewGuid(), modelId, authorId);
                                }
                            }
                        }

                    }

                    
               
                }
            }

            public static void Insert(Guid ID, String modelName, String notes, DateTime creationDate, DateTime modificationDate, List<ServerAuthor> creatorList, String pubmedId)
            {
                if(modelName != null)
                {
              

                    if(creatorList != null)
                    {
                        foreach (ServerAuthor author in creatorList)
                        {
                            author.UpdateDatabase();


                        }


                    }
                }

            }



            /*  doc = new XmlDocument();
              doc.Load(file);
              XmlReader reader = XmlReader.Create(file);
              reader.ReadToFollowing("annotation");

                    XmlNodeList list = doc.GetElementsByTagName("annotation");
                  XmlNode annotation = list.Item[0];
            
             */

        }
    
}
