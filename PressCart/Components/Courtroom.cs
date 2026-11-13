/*
' Copyright (c) 2017 12th Judicial Circuit
'  All rights reserved.
' 
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
' 
*/

using DotNetNuke.ComponentModel.DataAnnotations;
using System;
using System.Web.Caching;

namespace tjc.Modules.PressCart.Components
{
    [TableName("tjc_presscart_courtrooms")]
    //setup the primary key for table
    [PrimaryKey("CourtroomId", AutoIncrement = true)]
    //configure caching using PetaPoco
    [Cacheable("Courtrooms", CacheItemPriority.Default, 20)]
    public class Courtroom
    {
        public int CourtroomId { get; set; }
        public string CourtroomName { get; set; }
        public int CourthouseId { get; set; }
    }

    [TableName("tjc_presscart_courtrooms_by_courthouse")]
    //setup the primary key for table
    [PrimaryKey("CourtroomId", AutoIncrement = true)]
    //configure caching using PetaPoco
    public class CourtroomList
    {
        public int CourtroomId { get; set; }
        public string CourtroomName { get; set; }
        public int CourthouseId { get; set; }
        public string CourthouseName { get; set; }
    }
}
