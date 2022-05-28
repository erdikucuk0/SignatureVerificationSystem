using SignatureVerificationSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignatureVerificationSystem.Utils
{
    public sealed class PartModelHelper
    {
        private static readonly object lockObject = new object();

        private static PartModel partModel;

        public static PartModel Instance
        {
            get
            {
                if (partModel == null)
                {
                    lock (lockObject)
                    {
                        if (partModel == null)
                        {
                            partModel = new PartModel();
                        }
                    }
                }
                return partModel;
            }
        }
    }
}