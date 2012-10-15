using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Text;
using System.Web.UI.HtmlControls;

namespace BulkAssetDownload
{
    public partial class ConfirmDownload : LayoutsPageBase
    {

        int selectedFilesTotalSizeLimit;
        string eulaText;
        Guid listId;
        string webUrl;
        List<int> itemIds;
        string itemIdsString;
        double selectedFilesTotalSize;
        string fileName;
        private const string WEBTRENDSSCRIPT = "<script type='text/javascript'>var fileurls={0};var username={1};</script>";

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder fileurls = new StringBuilder();
            int i = 0;
            Helpers.Log("Entered the Page Loag method of the ConfirmDownload.aspx page");

            Helpers.Log("Reading Configuration values");
            GetConfigurationValues();

            Helpers.Log("Reading query string values");
            ReadQueryStringParameters();           

            if (!Page.IsPostBack)
            {
                Helpers.Log("Adding EULA");
                AddEULA();

                Helpers.Log("Getting total file size");
                selectedFilesTotalSize = GetFileSize();

                if (selectedFilesTotalSize > selectedFilesTotalSizeLimit)
                {
                    Helpers.Log("File size exceeds.");
                    btnSubmit.Enabled = false;
                    lblTotalFileSize.Text = String.Format("Total file size exceeds {0} MB", selectedFilesTotalSizeLimit);
                }
                else
                {
                    Helpers.Log("Setting file size");
                    SetFileSizeInPage();
                }
                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists[listId];
                        SPListItem item = list.GetItemById(itemIds[0]);
                        fileName = Path.GetFileNameWithoutExtension(item.File.Name);

                        foreach (int itemId in itemIds)
                        {
                            item = list.GetItemById(itemId);
                            fileurls.Append(web.Url+"/"+item.File.Url);
                            if (i.ToString() != (itemIds.Count - 1).ToString())
                            {
                                fileurls.Append("|");
                            }
                            i++;
                        }
                        ScriptManager.RegisterHiddenField(this, "hdnfileurls", fileurls.ToString());
                        ScriptManager.RegisterHiddenField(this, "hdnusername",SPContext.Current.Web.CurrentUser.LoginName.ToString());
                    }
                }
            }
        }


        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            Helpers.Log("Entered the btnSubmit_Click method");
            tblConfirmDownloadTable.Visible = false;

            switch (rdlDownloadFormat.SelectedIndex)
            {
                case 0:
                    {
                        Helpers.Log("Creating download links for Individual archives");                        
                        Table tblIndividualFileDownloadLinks = GetTableForIndividualArchiveFiles();                        
                        pnlDownloadContents.Controls.Add(tblIndividualFileDownloadLinks);
                        break;
                    }

                case 1:
                    {
                        Helpers.Log("Creating download links for Single archive");                        
                        Table tblSingleArchiveFileDownloadLinks = GetTableForSingleArchive();                        
                        pnlDownloadContents.Controls.Add(tblSingleArchiveFileDownloadLinks);
                        break;
                    }

                case 2:
                    {
                        Helpers.Log("Creating download links for direct downloads");                       
                        Table tblNoArchivingDownloadLinks = GetTableForNoArchiving();                        
                        pnlDownloadContents.Controls.Add(tblNoArchivingDownloadLinks);
                        break;
                    }
            }

        }      
        private Table GetTableForNoArchiving()
        {            
            Table table = new Table();             
            try
            {                
                foreach (int itemId in itemIds)
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listId];
                            SPListItem item = list.GetItemById(itemId);
                            String downloadLink = String.Format("{0}/_layouts/download.aspx?SourceUrl={1}", webUrl, item.File.Url);
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();                            
                            HyperLink downloadLinkControl = new HyperLink();                            
                            downloadLinkControl.Target = "_self";
                            downloadLinkControl.Text = Path.GetFileNameWithoutExtension(item.File.Name);
                            downloadLinkControl.NavigateUrl = downloadLink;                   
                            cell.Controls.Add(downloadLinkControl);
                            row.Cells.Add(cell);
                            table.Rows.Add(row);                                            
                        }
                    }

                }                
            }
            catch (Exception e)
            {
                Helpers.Log(e);
                throw e;
            }
            return table;
        }

        private Table GetTableForSingleArchive()
        {   
            Table table = new Table();                       
            try
            {
                string currentusername = SPContext.Current.Web.CurrentUser.Name.ToString();
                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists[listId];
                        SPListItem item = list.GetItemById(itemIds[0]);
                        fileName = Path.GetFileNameWithoutExtension(item.File.Name);
                    }
                }                
                
                String downloadLink = String.Format("{0}/_layouts/BulkAssetDownload/BulkAssetDownloadHandler.ashx?webUrl={1}&listId={2}&itemIds={3}&fileName={4}", webUrl, webUrl, listId.ToString(), itemIdsString, fileName);
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                HyperLink downloadLinkControl = new HyperLink();                
                downloadLinkControl.Text = fileName;
                downloadLinkControl.Target = "_self";
                downloadLinkControl.NavigateUrl = downloadLink;
                cell.Controls.Add(downloadLinkControl);
                row.Cells.Add(cell);
                table.Rows.Add(row);                
                
            }
            catch (Exception e)
            {
                Helpers.Log(e);
                throw e;
            }
            return table;
        }

        private Table GetTableForIndividualArchiveFiles()
        {            
            Table table = new Table();
            try
            {               
                foreach (int itemId in itemIds)
                {
                    using (SPSite site = new SPSite(webUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            SPList list = web.Lists[listId];
                            SPListItem item = list.GetItemById(itemId);

                            String downloadLink = String.Format("{0}/_layouts/BulkAssetDownload/BulkAssetDownloadHandler.ashx?webUrl={1}&listId={2}&itemIds={3}&fileName={4}", webUrl, webUrl, listId.ToString(), itemId, Path.GetFileNameWithoutExtension(item.File.Name));
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();
                            HyperLink downloadLinkControl = new HyperLink();                            
                            downloadLinkControl.Text = Path.GetFileNameWithoutExtension(item.File.Name);
                            downloadLinkControl.Target = "_self";
                            downloadLinkControl.NavigateUrl = downloadLink;
                            cell.Controls.Add(downloadLinkControl);
                            row.Cells.Add(cell);
                            table.Rows.Add(row);                            
                        }
                    }

                }
            }
            catch (Exception e)
            {
                Helpers.Log(e);
                throw e;
            }

            return table;
        }

        private void GetConfigurationValues()
        {

            string sizeSetting = ConfigurationManager.GetSetting("TranscodingTotalSizeLimit");

            if (!Int32.TryParse(sizeSetting, out selectedFilesTotalSizeLimit))
            {
                selectedFilesTotalSizeLimit = 200;
            }

            eulaText = ConfigurationManager.GetRichTextSetting("TranscodeEULA");

        }

        private void AddEULA()
        {
            HtmlGenericControl eula = new HtmlGenericControl("div");
            eula.InnerHtml = eulaText;
            pnlEULAText.Controls.Add(eula);
        }

        private void ReadQueryStringParameters()
        {
            Helpers.Log("Entered ReadQueryStringParameters() method");
            try
            {
                Helpers.Log("Reading item Ids");
                itemIdsString = Request.QueryString["items"];

                Helpers.Log("Reading source");
                string listIdString = Request.QueryString["source"];

                Helpers.Log("Reading location");
                webUrl = Request.QueryString["location"];

                Helpers.Log("Reading url");
                string paramFileUrl = Request.QueryString["url"];

                
                if (!String.IsNullOrEmpty(listIdString))
                {
                    Helpers.Log("Trying to parse Guid " + listIdString);
                    listId = new Guid(listIdString);
                }

                itemIds = new List<int>();
                if (!String.IsNullOrEmpty(itemIdsString))
                {
                    Helpers.Log("This request must have originated from a Library");
                    char[] splitters = new char[] { '|' };

                    StringCollection itemIdStrings = new StringCollection();

                    itemIdStrings.AddRange(itemIdsString.Split(splitters, StringSplitOptions.RemoveEmptyEntries));

                    Helpers.Log("Trying to parse each item id");
                    foreach (String itemIdString in itemIdStrings)
                    {
                        itemIds.Add(Int32.Parse(itemIdString));
                    }
                    Helpers.Log("Parsed all item id");
                }
                else if (!String.IsNullOrEmpty(paramFileUrl))
                {
                    Helpers.Log("This request must have originated from a search result page");
                    using (SPSite site = new SPSite(paramFileUrl))
                    {
                        using (SPWeb web = site.OpenWeb())
                        {
                            string siteUrl = site.Url;

                            Helpers.Log("Get the position of the last slash so that the string can be split");
                            int lastSlashPosition = paramFileUrl.LastIndexOf('/');

                            Helpers.Log("Get folder url");
                            string folderUrl = paramFileUrl.Substring(0, lastSlashPosition);

                            Helpers.Log("Get fileUrl");
                            string fileUrl = paramFileUrl.Substring(lastSlashPosition + 1);

                            Helpers.Log("Get folder object");
                            SPFolder folder = web.GetFolder(folderUrl);

                            Helpers.Log("Get file object");
                            SPFile file = folder.Files[fileUrl];

                            Helpers.Log("Get the list item");
                            SPListItem item = file.Item;

                            if (file.Item != null)
                            {
                                Helpers.Log("Get File ListItem id");
                                webUrl = file.Item.ParentList.ParentWeb.Url;
                                listId = file.Item.ParentList.ID;
                                itemIds.Add(file.Item.ID);
                                itemIdsString = file.Item.ID.ToString();
                            }
                            else
                            {
                                throw new Exception("Specified file to download is invalid");
                            }
                        }
                    }

                }
                else
                {
                    throw new Exception("Specified file to download is invalid");
                }


            }
            catch (Exception e)
            {
                Helpers.Log(e);
                Exception newException = new Exception("The page has been accessed in an invalid manner", e);
                throw e;
            }


        }

        private double GetFileSize()
        {
            Helpers.Log("Entered GetFileSize()");
            double size = 0;
            try
            {
                double sizeInBytes = 0;

                using (SPSite site = new SPSite(webUrl))
                {
                    using (SPWeb web = site.OpenWeb())
                    {
                        SPList list = web.Lists[listId];

                        foreach (int itemId in itemIds)
                        {
                            SPListItem item = list.GetItemById(itemId);
                            sizeInBytes += item.File.Length;
                        }

                        size = sizeInBytes / (1024 * 1024);
                    }
                }
            }
            catch (Exception e)
            {
                Exception newException = new Exception("The page has been accessed in an invalid manner", e);
                throw e;
            }
            finally
            {
                Helpers.Log("Exitting GetFileSize");
            }
            return size;
        }

        private void SetFileSizeInPage()
        {
            if (selectedFilesTotalSize == 0)
            {
                lblTotalFileSize.Text = "File Size not available";
            }
            else
            {
                lblTotalFileSize.Text = String.Format("Total size of the selected files = {0} MB", selectedFilesTotalSize.ToString("F"));
            }
        }        
    }
}
