using System;
using System.IO;
using System.Xml;
using System.Web.Services;
using RestSharp;

namespace WebApplication1
{
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class WebService1 : System.Web.Services.WebService
    {

        [WebMethod]
        public int SumOfNums(int First, int Second)
        {
            return First + Second;
        }

        //string path = "C:\\viewspecs";
        [WebMethod]
        public string ReadXmlFiles(string path, string lni)
        {
            string val = null;
            string attFile = null;
            string uri = null;
            string status = null;
            string target = @"C:\Learnings\XMLFiles\Target";
            DirectoryInfo di = new DirectoryInfo(path);
            FileInfo[] rgFiles = di.GetFiles("*.xml");
            foreach (FileInfo fi in rgFiles)
            {

                XmlTextReader reader = new XmlTextReader(fi.FullName);

                XmlDocument xml = new XmlDocument();
                xml.Load(reader);

                var nsmgr = new XmlNamespaceManager(xml.NameTable);
                nsmgr.AddNamespace("xsl", "http://www.w3.org/1999/XSL/Transform");

                val = xml.InnerXml;
                //val = xml.SelectNodes("title");
                uri = xml.BaseURI;

                Directory.CreateDirectory(target + "\\" + Path.GetFileNameWithoutExtension(uri));


                XmlNodeList elemList = xml.GetElementsByTagName("attachment");
                try
                {

                    for (int i = 0; i < elemList.Count; i++)
                    {
                        attFile = elemList[i].InnerText;
                        //string var = attFile.ToString();

                        DirectoryInfo dirInWhichtoSearch = new DirectoryInfo(@"C:\Learnings\Attachment");
                        //FileInfo[] filesInDir = dirInWhichtoSearch.GetFiles(attFile);
                        string[] files = Directory.GetFiles(@"C:\Learnings\Attachment", attFile);
                        foreach (string s in files)
                        {
                            //uri = xml.BaseURI;

                            File.Copy(s, target + "\\" + Path.GetFileNameWithoutExtension(uri) + "\\" + Path.GetFileName(s), true);
                            //File.Copy(s, Directory.CreateDirectory(target + "\\" + Path.GetFileNameWithoutExtension(uri)) + "\\" + Path.GetFileName(s), true);
                        }

                        //if (StatusStr == "Title 1")
                        //{
                        //    uri = xml.BaseURI;

                        //}

                    };
                    status = "Done";
                }
                catch (InvalidCastException e)
                {
                    // Perform some action here, and then throw a new exception.
                    status = "Some Issue";
                }
                /*if (StatusStr != "Title 2")
                {
                    File.AppendAllText(@"C:\Test\result.txt", p);
                    File.AppendAllText(@"C:\Test\result.txt", StatusStr);
                }*/

                //string valaa = xml.SelectNodes("//stylesheet/@name", nsmgr);

                /*XmlNodeList xnList = xml.SelectNodes("/vs:fmtDef/vs:fmtBlockDef/vs:fmtElems/vs:fmtPlaceholder/vs:processPlan/stylesheet");
                foreach (XmlNode xnode in xnList)
                {
                    val =  xnode.Value;
                }*/



            }
            return status;

        }

        [WebMethod]
        public string GenerateUnformatted(string GDSDirectory, string VS, string rollup1)
        {
            string[] gdsFile = Directory.GetFiles(GDSDirectory);
            foreach (string lniGDS in gdsFile)
            {
                //string unformattedOld = CreateUnformatted(subdirectory, lniInGDS);

                string[] vspefile = Directory.GetFiles("C:\\ContentBuildAsia\\WIP\\Sprint60\\ExistingVS\\viewspecs", VS + "*");
                string vsString = System.IO.File.ReadAllText(vspefile[0]);

                string lnifile = System.IO.File.ReadAllText(lniGDS);
                string lnifileString = lnifile.Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n", "");

                string rollup = rollup1 + Path.GetFileNameWithoutExtension(lniGDS);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                using (StreamReader sr = new StreamReader(@"C:\ContentBuildAsia\WIP\Sprint60\Test1\Unformatted_bodydata.xml"))
                {
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }

                sb.Replace("<vs/>", vsString);
                sb.Replace("<res/>", rollup);

                var client = new RestClient("http://localhost:26011/shared/document/vstesting");
                var request = new RestRequest(Method.POST);
                request.AddHeader("postman-token", "1c31e401-797f-036f-9254-0da4d039ed17");
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("content-type", "application/x-www-form-urlencoded");
                request.AddHeader("x-ln-retrieveoptions", "<docoptions> <appoverride name=\"S.BESTPARAGRAPH\" value=\"Y\"/><appoverride name=\"X.MAXDOCSIZE\" value=\"3000000000\"/><appoverride name=\"X.DOFALLBACK\" value=\"Y\"/><appoverride name=\"X.DOSHEPSIGNALS\" value=\"N\"/><appoverride name=\"X.DOLINKS\" value=\"Y\"/><appoverride name=\"S.SELECTOR\" value=\"UNFORMATTED\"/><appoverride name=\"S.VIEW\" value=\"FULL\"/><appoverride name=\"RETR.STYLESHEETPATH\" value=\"C:\\stylesheets\"/></docoptions>");
                request.AddHeader("content-location", rollup);
                request.AddHeader("accept", "application/xml");
                request.AddHeader("x-ln-session", "<ns2:sessionToken xmlns:ns2=\"http://services.lexisnexis.com/xmlschema/session-token/1\"><sessionID>ee5d5101-34f3-4096-a257-2ddf28fb7094</sessionID><issued>2017-02-01T10:20:48.329-05:00</issued><userPermIDUrn>urn:user:DD10538522</userPermIDUrn><authorizationPermID>1000202</authorizationPermID><formattingLocale>en-US</formattingLocale><languageLocale>en-CA</languageLocale><timezone>US/Eastern</timezone><signature>v1-1ba9d47f4ead098536eb9675f72f8b29</signature></ns2:sessionToken>");
                request.AddHeader("x-ln-request", "<rt:requestToken xmlns:rt=\"http://services.lexisnexis.com/xmlschema/request-token/1\"><transactionID>0016ce12-178e-10fc-cd0f-ff0f263f85d8</transactionID><sequence>1.2</sequence><featurePermID/><clientID>test client</clientID><cpmFeatureCode/><billBackString>test</billBackString><contextualFeaturePermID>1000516</contextualFeaturePermID><other><priceToken>Demo</priceToken></other></rt:requestToken>");
                request.AddHeader("x-ln-price-token", "jmzAQi5dSCp1RhebIw9m/7gMiGDyjrMMYXSl0duWxWOJEntZsOmxaiZQAh3ge1iPL73C8J2Pc0ZwP76u/RH7iMU2Fn4f2TQmpscpLPsCLzzusJsB6H/pgQ==");
                request.AddHeader("x-ln-application", "<ns2:applicationToken xmlns:ns2=\"http://services.lexisnexis.com/xmlschema/application-token/1\"><applicationPermID>1000202</applicationPermID><issued>2017-01-18T06:39:19.990-05:00</issued><signature>v1-8a3687879a483c3a572ee1b7c528aea5</signature></ns2:applicationToken>");

                string sss = sb.ToString();

                request.AddParameter("application/xml", sb, ParameterType.RequestBody);

                IRestResponse response = client.Execute(request);

                //if (!IsReturnedStatusCodeOK(response))
                //{
                //    throw new HttpRequestException("Request issue -> HTTP code:" + response.StatusCode);
                //}
                string finalResponse = response.Content;

                string path = @"C:\ContentBuildAsia\WIP\Sprint60\Output";
                XmlDocument xdoc = new XmlDocument();
                xdoc.LoadXml(finalResponse);
                xdoc.Save(path + "\\" + Path.GetFileNameWithoutExtension(lniGDS) + "_Unformatted.xml");
               
                //return finalResponse;

            }
            return "Done";

            
        }
    }
}
   
        

    


