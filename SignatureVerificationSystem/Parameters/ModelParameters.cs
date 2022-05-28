using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Parameters
{
    public class ModelParameters
    {
        #region Member Fields

        public string ImageSize { get; set; } = null;
        public int NumberOfClasses { get; set; } = -1;
        public int SupportSet { get; set; } = -1;

        #endregion

        #region Constructors

        public ModelParameters() { }

        public ModelParameters(string ImageSize, int NumberOfClasses, int SupportSet)
        {
            this.ImageSize = ImageSize;
            this.NumberOfClasses = NumberOfClasses;
            this.SupportSet = SupportSet;
        }

        #endregion
    }
}