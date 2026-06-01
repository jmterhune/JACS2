/*
' Copyright (c) 2023 Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Services.Search.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;

namespace tjc.Modules.Reports.Components
{
    /// -----------------------------------------------------------------------------
    /// <summary>
    /// The Controller class for Reports
    /// 
    /// The FeatureController class is defined as the BusinessController in the manifest file (.dnn)
    /// DotNetNuke will poll this class to find out which Interfaces the class implements. 
    /// 
    /// The IPortable interface is used to import/export content from a DNN module
    /// 
    /// The ISearchable interface is used by DNN to index the content of a module
    /// 
    /// The IUpgradeable interface allows module developers to execute code during the upgrade 
    /// process for a module.
    /// 
    /// Below you will find stubbed out implementations of each, uncomment and populate with your own data
    /// </summary>
    /// -----------------------------------------------------------------------------
    public class FeatureController : ModuleSearchBase
    {
        // feel free to remove any interfaces that you don't wish to use
        // (requires that you also update the .dnn manifest file)

        #region Optional Interfaces

        /// <summary>
        /// Gets the modified search documents for the DNN search engine indexer.
        /// </summary>
        /// <param name="moduleInfo">The module information.</param>
        /// <param name="beginDate">The begin date.</param>
        /// <returns></returns>
        public override IList<SearchDocument> GetModifiedSearchDocuments(ModuleInfo moduleInfo, DateTime beginDate)
        {
            var searchDocuments = new List<SearchDocument>();

            return searchDocuments;
        }

        #endregion
    }
}