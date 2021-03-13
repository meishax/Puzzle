using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Media;

namespace myPuzzle
{

    public partial class Form_Main : Form
    {//图片列表
        PictureBox[] pictureList = null;
        //图片位置字典
        SortedDictionary<string,Bitmap>pictureLocationDict=new SortedDictionary<string,Bitmap>();
        Point[] pointList = null;
        //图片控件字典
        SortedDictionary<string, PictureBox> pictureBoxLocationDict = new SortedDictionary<string, PictureBox>();
        //拼图时间
        int secong = 0;
        //所拖拽的图片
        PictureBox currentPictureBox = null;
        //被迫需要移动的图片
        PictureBox haveToPictureBox = null;
        //原位置
        Point oldLocation = Point.Empty;
        //新位置
        Point newLocation = Point.Empty;
        //鼠标按下坐标（control控件的相对坐标）
        Point mouseDownPoint = Point.Empty;
        //显示拖动效果的矩形
        Rectangle rect = Rectangle.Empty;
        //是否正在拖拽
        bool isDrag = false;
        //图片位置
        public string originalpicpath;
        /// <summary>
        /// 每个方向上要切割成的块数
        /// </summary>
        SoundPlayer b = new SoundPlayer(Properties.Resources.背景);
        int seconds = 0;
        private int ImgNumbers
        {
            get
            {
                return (int)this.numericUpDown1.Value;
            }
                
        }
        /// <summary>
        /// 要切割成的正方形图片边长
        /// </summary>
        private int SideLength
        {
            get

            {
                return 600 / this.ImgNumbers;
            }
        }
        private void InitRandomPictureBox()
        {
            pnl_Picture.Controls.Clear();
            pictureList = new PictureBox[ImgNumbers * ImgNumbers];
            pointList = new Point[ImgNumbers * ImgNumbers];
            for (int i = 0; i < this.ImgNumbers;i++)
            {
                for(int j=0;j<this.ImgNumbers;j++)
                {
                    PictureBox pic = new PictureBox();
                    pic.Name = "pictureBox" + (j + i * ImgNumbers + 1).ToString();
                    pic.Location = new Point(j * SideLength, i * SideLength);
                    pic.Size = new Size(SideLength, SideLength);
                    pic.Visible = true;
                    pic.BorderStyle = BorderStyle.FixedSingle;
                    pic.MouseDown += new MouseEventHandler(pictureBox_MouseDown);
                    pic.MouseMove += new MouseEventHandler(pictureBox_MouseMove);
                    pic.MouseUp += new MouseEventHandler(pictureBox_MouseUp);
                    pnl_Picture.Controls.Add(pic);
                    pictureList[j + i * ImgNumbers] = pic;//图片切割后按从左到右，从上到下的顺序放入一维数组
                    pointList[j + i * ImgNumbers] = new Point(j * SideLength, i * SideLength);
                }
            }
        }
        public void Flow(string path,bool disorder)
        {
            InitRandomPictureBox();
            Image bm = CutPicture.Resize(path, 600, 600);
            CutPicture.BitMapList = new List<Bitmap>();
            for (int y=0;y<600;y+=SideLength)
            {
                for(int x=0;x<600;x+=SideLength)
                {
                    Bitmap temp = CutPicture.Cut(bm, x, y, SideLength, SideLength);
                    CutPicture.BitMapList.Add(temp);
                }
            }
            ImportitMap(disorder);
               
        }
        public void ImportitMap(bool disorder)
        {
            try
            {
                int i = 0;
                foreach (PictureBox item in pictureList)
                {
                    Bitmap temp = CutPicture.BitMapList[i];
                    item.Image = temp;
                    i++;
                }
                if (disorder)
                    ResetPictureLocation();
            }
            catch (Exception exp)
            {
                Console.WriteLine(exp.Message);
            }
        }
        public void InitGame()
        {

            if (!Directory.Exists(Application.StartupPath.ToString() + "\\picture"))
            {
                Directory.CreateDirectory(Application.StartupPath.ToString() + "\\picture");
                Properties.Resources._0.Save(Application.StartupPath.ToString() + "\\picture\\1.jpg");
                Properties.Resources._1.Save(Application.StartupPath.ToString() + "\\picture\\2.jpg");
                Properties.Resources._2.Save(Application.StartupPath.ToString() + "\\picture\\3.jpg");
                Properties.Resources._3.Save(Application.StartupPath.ToString() + "\\picture\\4.jpg");
                Properties.Resources._4.Save(Application.StartupPath.ToString() + "\\picture\\5.jpg");
                Properties.Resources._5.Save(Application.StartupPath.ToString() + "\\picture\\6.jpg");
                Properties.Resources._6.Save(Application.StartupPath.ToString() + "\\picture\\7.jpg");
            }
            Random r = new Random();
            int i = r. Next(7);
            originalpicpath = Application.StartupPath.ToString() + "\\Picture\\" + i.ToString() + ".jpg";
            Flow(originalpicpath, true);
        }
        public PictureBox GetPictureBoxByLocation(int x,int y)
        {
            PictureBox pic = null;
            foreach(PictureBox item in pictureList)
            {
                if(x>item.Location.X&&y>item.Location.Y&&item.Location.X+SideLength>x&&item.Location.Y+SideLength>y)
                {
                    pic = item;
                }
            }
            return pic;
        }



        public Form_Main()
        {
            InitializeComponent();
            InitGame();
           
        }
        private void pictureBox_MouseDown(object sender,MouseEventArgs e)
        {
            oldLocation = new Point(e.X, e.Y);
            currentPictureBox = GetPictureBoxByHashCode(sender.GetHashCode().ToString());
            MoseDown(currentPictureBox, sender, e);
        }
        public void MoseDown(PictureBox pic,object sender,MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                oldLocation = e.Location;
                rect = pic.Bounds;
;            }
        }
        private void pictureBox_MouseMove(object sender,MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                isDrag = true;
                rect.Location = getPointToForm(new Point(e.Location.X - oldLocation.X, e.Location.Y - oldLocation.Y));
                this.Refresh();
            }
        }
        private void reset()
        {
            mouseDownPoint = Point.Empty;
            rect = Rectangle.Empty;
            isDrag = false;
        }
        private void pictureBox_MouseUp(object sender,MouseEventArgs e)
        {
            oldLocation = new Point(currentPictureBox.Location.X, currentPictureBox.Location.Y);
            if(oldLocation.X+e.X>600||oldLocation.Y+e.Y>600||oldLocation.X+e.X<0||oldLocation.Y+e.Y<0)
            {
                return;
            }
            haveToPictureBox = GetPictureBoxByLocation(oldLocation.X + e.X, oldLocation.Y + e.Y);
            newLocation = new Point(haveToPictureBox.Location.X, haveToPictureBox.Location.Y);
            haveToPictureBox.Location = oldLocation;
            PictureMouseUp(currentPictureBox, sender, e);
            if(Judge())
            {
                lab_result.Text = "成功！";
                timer1.Enabled = false;
                timer2.Enabled = false;
                SoundPlayer a = new SoundPlayer(Properties.Resources.成功);
                a.Play();

            }
        }
        public void PictureMouseUp(PictureBox pic,object sender,MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Left)
            {
                if(isDrag)
                {
                    isDrag = false;
                    pic.Location = newLocation;
                    this.Refresh();
                }
                reset();
            }
        }
        public bool Judge()
        {
            bool result = true;
            int i = 0;
            foreach (PictureBox item in pictureList)
            {
                if (item.Location != pointList[i])
                {
                    result = false;
                }
                i++;
            }
            return result;
            }

        private void pnl_Picture_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pnl_Picture_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void splitContainer1_Panel1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        //试玩新图
        private void btn_import_Click(object sender, EventArgs e)
        {
            OpenFileDialog new_picture = new OpenFileDialog();
            if (new_picture.ShowDialog() == DialogResult.OK)
            {
                lab_result.Text = "";
                originalpicpath = new_picture.FileName;
                CutPicture.PicturePath = new_picture.FileName;
                Flow(CutPicture.PicturePath, true);
                //CountTime();
              
              

            }
            b.Play();
        }

        //切换图片
        private void btn_Changepic_Click(object sender, EventArgs e)
        {
            Random r = new Random();
            int i = r.Next(7);
            originalpicpath = Application.StartupPath.ToString() + "\\Picture\\" + i.ToString() + ".jpg";
            Flow(originalpicpath, true);
            b.Play();
        }

        //查看原图
        private void btn_Originalpic_Click(object sender, EventArgs e)
        {
            Form_Original original = new Form_Original();
            original.picpath = originalpicpath;
            original.ShowDialog();
        }

        //图片重排
        private void btn_Reset_Click(object sender, EventArgs e)
        {
            Flow(originalpicpath, true);
            lab_result.Text = "";
            b.Play();
                
        }
        public void ResetPictureLocation()
        {
            Point[] temp = DisOrderLocation();
            int i = 0;
            foreach (PictureBox item in pictureList)
            {
                item.Location = temp[i];
                i++;
            }
        }
        public Point[] DisOrderLocation()
        {
            Point[] tempArray = (Point[])pointList.Clone();
            for (int i = tempArray.Length - 1; i > 0; i--)
            {
                Random rand = new Random();
                int p = rand.Next(i);
                Point temp = tempArray[p];
                tempArray[p] = tempArray[i];
                tempArray[i] = temp;
            }
            return tempArray;
        }
        public PictureBox GetPictureBoxByHashCode(string hascode)
        {
            PictureBox pic = null;
            foreach (PictureBox item in pictureList)
            {
                if (hascode == item.GetHashCode().ToString())
                {
                    pic = item;
                }
            }
            return pic;
        }
        private Point getPointToForm(Point p)
        {
            return this.PointToClient(pictureList[0].PointToScreen(p));
        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void Form_Main_Load(object sender, EventArgs e)
        {
            b.Play();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if(!Judge())
            {
                MessageBox.Show("挑战失败！");
                timer1.Enabled = false;
                timer2.Enabled = false;
            }
        }

        //挑战模式
        private void challege_Click(object sender, EventArgs e)
        {
            b.Play();
            seconds = 0;
            timer2.Enabled = true;
            timer1.Enabled = true;
            if (Judge())
            {

                Flow(originalpicpath, true);
                lab_result.Text = "";
                b.Play();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {

            label1.Text = seconds.ToString()+" s";
             seconds++;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
    
}

