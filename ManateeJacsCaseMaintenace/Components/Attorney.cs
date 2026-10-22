/*
' Copyright (c) 2026 jterhune
'  All rights reserved.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED
' TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
' THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF
' CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'
*/

using System;

namespace tjc.Modules.JacsCaseMaint.Components
{
    // Populated from the jacs.tjc_get_attorney_by_barnumber stored procedure (BARNUM, Name, PHONENUM, EMAIL, ACTIVE)
    internal class Attorney
    {
        public string BARNUM { get; set; }
        public string NAME { get; set; }
        public string PHONENUM { get; set; }
        public string EMAIL { get; set; }
        public string ACTIVE { get; set; }

        public bool IsActive
        {
            get { return string.Equals(ACTIVE, "Y", StringComparison.OrdinalIgnoreCase); }
        }
    }
}
