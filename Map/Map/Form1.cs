using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using GMap.NET;
using GMap.NET.MapProviders;
using GMap.NET.WindowsForms;
using GMap.NET.WindowsForms.Markers;

namespace Map
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            GMaps.Instance.Mode = AccessMode.ServerAndCache;
            gMapC.MapProvider = GMapProviders.GoogleChinaMap;//设置底图为谷歌中国地图
            //gMapC.SetPositionByKeywords("Fuzhou, China");
            gMapC.DragButton = MouseButtons.Left;
            gMapC.MaxZoom = 19;//最大显示级别
            gMapC.MinZoom = 3;//最小级别
            gMapC.Zoom = 12;//初始化时的缩放级别（参考瓦片地图的级别划分）
            gMapC.Position = new PointLatLng(26.10, 119.20);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            listBox.Items.Clear();
            listBox.Items.Add("26.03298775,119.204552503912,福建师范大学");
            listBox.Items.Add("26.0613597,119.193340735152,福州大学");
            listBox.Items.Add("26.07798425,119.268145831242,福州大学至诚学院");
            listBox.Items.Add("26.0888446,119.367328358362,福建工程学院");
            listBox.Items.Add("26.086128,119.232176390959,福建农林大学金山学院");
        }

        private void listBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var itemStrs = listBox.SelectedItem.ToString().Split(',');
            var latLng = new PointLatLng(Convert.ToDouble(itemStrs[0].Trim()), Convert.ToDouble(itemStrs[1].Trim()));
            //var markersOverlay = new GMapOverlay("imarker");
            //markersOverlay.Markers.Add(new GMarkerGoogle(latLng, GMarkerGoogleType.red_dot)
            //    {ToolTipText = itemStrs[2].Trim()});
            //gMapC.Overlays.Add(markersOverlay);
            gMapC.Position = latLng;
            gMapC.Zoom = 15;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            gMapC.Zoom -= 1;
            gMapC.Overlays.Clear();
            var markersOverlay = new GMapOverlay("markers");
            Task.Run(() =>
            {
                foreach (var item in listBox.Items)
                {
                    var itemStrs = item.ToString().Split(',');
                    markersOverlay.Markers.Add(
                        new GMarkerGoogle(
                                new PointLatLng(Convert.ToDouble(itemStrs[0].Trim()), Convert.ToDouble(itemStrs[1].Trim())),
                                GMarkerGoogleType.red_dot)
                            { ToolTipText = itemStrs[2].Trim() });
                }
            });
            gMapC.Overlays.Add(markersOverlay);
            gMapC.Zoom += 1;
            gMapC.Refresh();
        }
    }
}
