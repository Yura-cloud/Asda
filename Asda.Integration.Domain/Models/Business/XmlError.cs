namespace Asda.Integration.Domain.Models.Business
{
    public class XmlError
    {
        public int Index { get; set; }
        public string Message { get; set; }
        public string SKU { get; set; }

        public override string ToString()
        {
            return Message;
        }
    }
}