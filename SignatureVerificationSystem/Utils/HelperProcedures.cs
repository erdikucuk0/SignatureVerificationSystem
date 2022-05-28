using SignatureVerificationSystem.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SignatureVerificationSystem.Utils
{
    public static class HelperProcedures
    {
        public static void InitTextBlocks(Grid texBlockGrid  )
        {
            PartModelHelper.Instance.SupportSetTextBlocks = UIHelper.FindVisualChildren<TextBlock>(texBlockGrid).OrderBy(x => x.Name).Select(x => x).ToList();
        }

        public static string ModelParametersImageSizeConvert(ImageSize imageSizeEnum)
        {
            switch (imageSizeEnum)
            {
                case ImageSize.Size_55x55:
                    return "55x55";
                
                case ImageSize.Size_105x105:
                    return "105x105";
                
                default:
                    return "";
            }
        }

        public static string ModelParametersNumberOfClassesConvert(NumberOfClasess numberOfClasessEnum)
        {
            switch (numberOfClasessEnum)
            {
                case NumberOfClasess.Twenty:
                    return "20";

                case NumberOfClasess.Five:
                    return "5";

                default:
                    return "";
            }
        }

        public static string ModelParametersSupportSetConvert(SupportSet supportSetEnum)
        {
            switch (supportSetEnum)
            {
                case SupportSet.Five:
                    return "5";

                case SupportSet.One:
                    return "1";

                default:
                    return "";
            }
        }
    }
}
