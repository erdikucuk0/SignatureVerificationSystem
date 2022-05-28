using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SignatureVerificationSystem.Models
{
    public class PartModel
    {
        #region Member Fields

        public List<TextBlock> SupportSetTextBlocks { get; set; } = new List<TextBlock>();
        public Dictionary<string, string> SupportSetSafeFileName { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> SupportSetFileName { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> QuerySetSafeFileName { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, string> QuerySetFileName { get; set; } = new Dictionary<string, string>();

        #endregion
    }
}