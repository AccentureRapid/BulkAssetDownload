<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConfirmDownload.aspx.cs" Inherits="BulkAssetDownload.ConfirmDownload" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<title></title>
    <link rel="stylesheet" type="text/css" href="/Style%20Library/en-US/Themable/Core%20Styles/controls.css" />
    <link rel="stylesheet" type="text/css" href="/_layouts/styles/dlgframe.css?rev=nFv%2BiaF39HSvTLAgsyDiTA%3D%3D" />
    <link rel="stylesheet" type="text/css" href="/_layouts/1033/styles/Themable/forms.css?rev=HUrQavHMn48%2BZ29vw1Ivaw%3D%3D" />
    <link rel="stylesheet" type="text/css" href="/_layouts/1033/styles/portal.css?rev=PNTsUZomOQOqawWKxgZcBA%3D%3D" />
    <link rel="stylesheet" type="text/css" href="/_layouts/1033/styles/Themable/search.css?rev=Uoc0fsLIo87aYwT%2FGX5UPw%3D%3D" />
    <link rel="stylesheet" type="text/css" href="/_layouts/1033/styles/Themable/corev4.css?rev=w9FW7ASZnUjiWWCtJEcnTw%3D%3D" />
    <link rel="stylesheet" type="text/css" href="h/_layouts/1033/styles/Themable/minimalv4.css" />
    <style type="text/css">
        #footer
        {
            position: relative;
        }
        
        #footer-content
        {
            position: absolute;
            bottom: 0;
            left: 0;
        }
    </style>


    

</asp:Content>
<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <asp:HiddenField ID="hdnWebUrl" runat="server" />
    <asp:HiddenField ID = "hdnListId" runat="server" />
    <asp:HiddenField ID = "hdnItemIds" runat="server" />
    <asp:HiddenField ID = "hdnFileName" runat="server" />
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <table runat="server" id="tblConfirmDownloadTable" cellpadding="2" cellspacing="2">
        <tr>
            <td>
                <asp:panel id="pnlTranscodeOptions" runat="server">
                    <div id="divlabelSelectedFileSize" class="s4-mini-header">
                        <h3>Transcode Assets</h3>
                        <h3><asp:Label ID="lblTotalFileSize" runat="server"></asp:Label></h3>
                    </div>
                    <asp:Panel ID="pnlEULAText" runat="server"></asp:Panel>
                    <hr />
                    <div id="divDownloadOptions">
                        <h3>Archiving Options</h3>
                    </div>
                    <br />
                    <div>
                        <asp:RadioButtonList ID="rdlDownloadFormat" runat="server">
                            <asp:ListItem Value="2">Download assets and compress into individual archive files</asp:ListItem>
                            <asp:ListItem Value="0" Selected="True">Download assets compressed into a single archive file</asp:ListItem>
                            <asp:ListItem Value="1">Download assets without archiving</asp:ListItem>
                        </asp:RadioButtonList>
                        <div>
                            <br />
                            <table id="tblButtonsTable" width="100%">
                                <tr>
                                <td align="center">
                                    <asp:Button ID="btnSubmit" Text="Submit" runat="server" Width="100" OnClick="btnSubmit_Click" OnClientClick="javascript:LogWebtrendsBulkDownload();" />
                                    
                                </td>
                                <td align="center">
                                    <input type="button" title = "Cancel" value="Cancel" name="Cancel" onclick="javascript:parent.commonModalDialogClose(1,'The download operation was cancelled.');" />
                                </td>
                            </tr>
                            </table>
                        </div>
                    </div>
                </asp:panel>
            </td>
        </tr>
    </table>    
    <asp:Panel ID="pnlDownloadContents" runat="server">
    </asp:Panel>


</asp:Content>
<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
</asp:Content>
<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
</asp:Content>
