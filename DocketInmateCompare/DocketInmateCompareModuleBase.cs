/*
' Copyright (c) 2026  Joe Terhune
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.Abstractions;
using DotNetNuke.Entities.Modules;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace tjc.Modules.DocketInmateCompare
{
    public class DocketInmateCompareModuleBase : PortalModuleBase
    {
        private readonly INavigationManager _navigationManager;
        public DocketInmateCompareModuleBase()
        {
            _navigationManager = DependencyProvider.GetRequiredService<INavigationManager>();
        }

        public string ModuleHomeURL
        {
            get
            {
                return _navigationManager.NavigateURL();
            }

        }
    }
}