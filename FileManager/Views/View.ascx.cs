/*
' Copyright (c) 2026  12th Judicial Circuit
'  All rights reserved.
*/

using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Modules.Actions;
using DotNetNuke.Security;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Localization;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using tjc.Modules.FileManager.Components;

namespace tjc.Modules.FileManager.Views
{
    /// <summary>
    /// Shows the files of one share on the public site as a tree. Who may see the module is
    /// controlled by the standard DNN module view permissions; the share is chosen in Settings.
    /// </summary>
    public partial class View : PortalModuleBase, IActionable
    {
        private const string DownloadHandler = "~/DesktopModules/tjc.modules/FileManager/Handlers/Download.ashx";
        private const string ImagesPath = "~/DesktopModules/tjc.modules/FileManager/images/";

        public ModuleActionCollection ModuleActions
        {
            get { return new ModuleActionCollection(); }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string share = Convert.ToString(Settings["ShareKey"]);
                if (string.IsNullOrWhiteSpace(share))
                {
                    ShowMessage(IsEditable
                        ? "Choose the share to display in this module's settings."
                        : "This module has not been set up yet.");
                    return;
                }

                var client = new RemoteFileClient();
                if (!client.IsConfigured)
                {
                    ShowMessage(IsEditable
                        ? "Remote file access is not configured. Set FileManagerRemote.BaseUrl and FileManagerRemote.Secret in web.config."
                        : "Files are not available right now.");
                    return;
                }

                RemoteListing listing;
                try
                {
                    listing = client.GetListing(share);
                }
                catch (Exception ex)
                {
                    // Log it, but don't show users details of the connection to the public site.
                    Exceptions.LogException(ex);
                    ShowMessage("The file list is temporarily unavailable. Please try again later.");
                    return;
                }

                // The setting overrides the folder's own name; the share key is the last resort.
                string rootLabel = Convert.ToString(Settings["RootLabel"]);
                if (string.IsNullOrWhiteSpace(rootLabel)) rootLabel = listing.Root;
                if (string.IsNullOrWhiteSpace(rootLabel)) rootLabel = share;
                BuildTree(listing, rootLabel.Trim());
            }
            catch (Exception exc)
            {
                Exceptions.ProcessModuleLoadException(this, exc);
            }
        }

        private void ShowMessage(string text)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.Visible = true;
        }

        private void BuildTree(RemoteListing listing, string rootLabel)
        {
            string folderIcon = ResolveUrl(ImagesPath + "folder.gif");

            // Like the public module: one root folder, open, with everything else closed beneath it.
            var root = new TreeNode(rootLabel) { SelectAction = TreeNodeSelectAction.Expand, ImageUrl = folderIcon, Expanded = true };
            tvFiles.Nodes.Add(root);

            // The API lists a folder before its contents, so a folder's parent node already exists.
            var folderNodes = new Dictionary<string, TreeNode>(StringComparer.OrdinalIgnoreCase);

            foreach (string folder in listing.Directories)
            {
                var node = new TreeNode(LastSegment(folder))
                {
                    SelectAction = TreeNodeSelectAction.Expand,
                    ImageUrl = folderIcon,
                    Expanded = false
                };
                folderNodes[folder] = node;
                ParentNode(ParentOf(folder), folderNodes, root).ChildNodes.Add(node);
            }

            string handler = ResolveUrl(DownloadHandler);
            var iconByExtension = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (RemoteFileEntry file in listing.Files)
            {
                string name = LastSegment(file.Path);
                var node = new TreeNode(name)
                {
                    NavigateUrl = handler + "?tabid=" + TabId + "&mid=" + ModuleId + "&p=" + Server.UrlEncode(file.Path),
                    ImageUrl = FileIcon(name, iconByExtension),
                    ToolTip = string.Format("{0:N0} KB, modified {1:d}", Math.Max(1, (file.Size + 1023) / 1024), file.Modified.ToLocalTime())
                };
                ParentNode(ParentOf(file.Path), folderNodes, root).ChildNodes.Add(node);
            }
        }

        private static TreeNode ParentNode(string parentPath, Dictionary<string, TreeNode> folderNodes, TreeNode root)
        {
            TreeNode parent;
            return parentPath.Length > 0 && folderNodes.TryGetValue(parentPath, out parent) ? parent : root;
        }

        /// <summary>The module's icon for the file's extension (e.g. images/icons/pdf.gif), or the generic file icon.</summary>
        private string FileIcon(string fileName, Dictionary<string, string> cache)
        {
            string ext = System.IO.Path.GetExtension(fileName).TrimStart('.');
            string url;
            if (cache.TryGetValue(ext, out url))
            {
                return url;
            }

            string icon = ImagesPath + "icons/" + ext.ToLowerInvariant() + ".gif";
            url = ext.Length > 0 && ext.IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) < 0 && System.IO.File.Exists(Server.MapPath(icon))
                ? ResolveUrl(icon)
                : ResolveUrl(ImagesPath + "icons/file.gif");
            cache[ext] = url;
            return url;
        }

        private static string ParentOf(string path)
        {
            int slash = path.LastIndexOf('/');
            return slash < 0 ? string.Empty : path.Substring(0, slash);
        }

        private static string LastSegment(string path)
        {
            return path.Substring(path.LastIndexOf('/') + 1);
        }
    }
}
