namespace WindowsFormsApp
{
    public class NguoiDungInfo
    {
        public int IDNCC { get; set; }
        public string HoVaTen { get; set; }
        public string QuyenHan { get; set; }

        public override string ToString()
        {
            return HoVaTen;
        }
    }
}

