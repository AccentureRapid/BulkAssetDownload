using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using Ionic.Zip;
using Microsoft.SharePoint;

namespace BulkAssetDownload
{
    public partial class BulkAssetDownloadHandler : IHttpHandler
    {
        public bool IsReusable
        {
            get { return false; }
        }

        Guid listId;
        string webUrl;
        List<int> itemIds;
        string token = String.Empty;
        string fileName;

        public void ProcessRequest(HttpContext context)
        {
            Helpers.Log("Entered ProcessRequest() method of th Http handler");
            try
            {
                Helpers.Log("Getting user agent");
                String userAgent = context.Request.Headers.Get("User-Agent");

                Helpers.Log("Reading query strings");
                ReadQueryStrings(context);
                Helpers.Log("Finished reading query strings");


                context.Response.Clear();
                context.Response.BufferOutput = false;
                context.Response.ContentType = "application/zip";

                if (userAgent.Contains("MSIE"))
                {
                    context.Response.AddHeader("content-disposition", "filename=" + HttpUtility.UrlPathEncode(fileName) );
                }
                else
                {
                    context.Response.AddHeader("content-disposition", "filename=\"" + fileName + "\"");
                }

                Helpers.Log("Connecting to site " + webUrl);
                using (SPSite site = new SPSite(webUrl))
                {   
                    Helpers.Log("Connecting to web");
                    using (SPWeb web = site.OpenWeb())
                    {
                        Helpers.Log("Opening list");
                        SPList list = web.Lists[listId];

                        Helpers.Log("Creating a zip file");

                        int totalSize = 0;
                        int limit = 200;

                        Int32.TryParse(ConfigurationManager.GetSetting("TranscodingTotalSizeLimit"), out limit);
                        
                        using (ZipFile zipFile = new ZipFile())
                        {
                            Helpers.Log("Iterating through all ids");
                            foreach (int itemId in itemIds)
                            {
                                Helpers.Log("Getting item of ID " + itemId);
                                SPListItem item = list.GetItemById(itemId);
                                zipFile.AddEntry(item.File.Name, item.File.OpenBinaryStream());
                                Helpers.Log("Added item to zip file");

                                totalSize = (int)(totalSize + (item.File.Length / (1024 * 1024)));
                                if (totalSize > limit)
                                {
                                    throw new Exception(String.Format("Total file size is {0} MB, It exceeds the limit {1} MB", totalSize, limit));
                                }

                                
                            }
                            Helpers.Log("Saving zip file to output stream");
                            zipFile.Save(context.Response.OutputStream);
                        }
                    }
                }

                context.Response.Close();

            }
            catch (Exception e)
            {
                Helpers.Log(e);              
                context.Response.Write("<script>alert('" +e.Message + "');</script>");
                context.Response.Flush();
                context.Response.Close();
                context.Response.End();
            }


        }

       

        private void ReadQueryStrings(HttpContext context)
        {
            try
            {

                webUrl = context.Request.QueryString["webUrl"];
                string listIdString = context.Request.QueryString["listId"] ?? String.Empty ;
                string itemIdsString = context.Request.QueryString["itemIds"] ?? String.Empty;
                fileName = context.Request.QueryString["fileName"] ?? String.Empty;




                if (!String.IsNullOrEmpty(listIdString))
                {
                    listId = new Guid(listIdString);
                }
                else
                {
                    throw new Exception("Invalid List ID");
                }

                if (!String.IsNullOrEmpty(itemIdsString))
                {
                    char[] splitters = new char[] { '|' };

                    StringCollection itemIdStrings = new StringCollection();
                    itemIds = new List<int>();
                    itemIdStrings.AddRange(itemIdsString.Split(splitters, StringSplitOptions.RemoveEmptyEntries));

                    foreach (String itemIdString in itemIdStrings)
                    {
                        itemIds.Add(Int32.Parse(itemIdString));
                    }
                }
                else
                {
                    throw new Exception("Invalid Item ID(s)");
                }

                if (String.IsNullOrEmpty(webUrl))
                {
                    throw new Exception("Invalid Web Url");
                }

                if (String.IsNullOrEmpty(fileName))
                {
                    fileName = "Assets.zip"; 
                }
                else if (!fileName.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
                {
                    fileName = fileName + ".zip";
                }
            }
            catch (Exception e)
            {
                throw new Exception("An error occurred while reading the query string parameters", e);
            }
        }

        
    }
}
