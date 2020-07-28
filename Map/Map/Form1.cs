using System;
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
            gMapC.Zoom = 13;
            GMapOverlay markersOverlay = new GMapOverlay("markers");
            markersOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(26.03298775, 119.204552503912),
                GMarkerGoogleType.red_dot){ToolTipText = "FJNU"});
            markersOverlay.Markers.Add(new GMarkerGoogle(new PointLatLng(26.0613597, 119.193340735152),
                    GMarkerGoogleType.red_dot) { ToolTipText = "FZU" });
            gMapC.Overlays.Add(markersOverlay);
            gMapC.Zoom = 12;
        }
    }
}
