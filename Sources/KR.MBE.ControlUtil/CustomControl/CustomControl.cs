using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace KR.ITIER.UI.ControlUtil.CustomControl
{
	//All the defined shape type
	public enum ShapeType{	Rectangle,
							RoundedRectangle,
		                    Diamond,
		                    Crane,
							Crane2,
							Crane3,
							Ellipse, 
							TriangleUp,
							TriangleDown,
							TriangleLeft,
							TriangleRight,
		                    BallonNE,
		                    BallonNW,
							BallonSW,
							BallonSE,
		                    CustomPolygon,
		                    CustomPie,
                            LineDown,
                            LineUp,
                            LineHorizontal,
                            LineVertical
		                    
						}


	public class CustomControl : System.Windows.Forms.Control
	{
		public string mCraneID;
		public string mBlockID;
		public int mXPos, mYPos;

		private GraphicsPath _custompath = new GraphicsPath();
        private IContainer components;

		ShapeType _shape=ShapeType.Rectangle;
		DashStyle _borderstyle=DashStyle.Solid;
		Color _bordercolor=Color.FromArgb(255,255,0,0);
		int _borderwidth=3;
		GraphicsPath _outline=new GraphicsPath();
		bool _usegradient=false;
        bool _blink = false;
        bool _vibrate = false;
        bool _voffseted = false;

        bool _animateborder = false;
        int _prevborderwidth = 3;
        Color _prevbordercolor = Color.Red;
        bool _btoggled = false;
        int _static_ds = 0;

        DashStyle _prevborderstyle = DashStyle.Solid;

 
		Color _centercolor=Color.FromArgb(100,255,0,0) ;
		Color _surroundcolor=Color.FromArgb(100,0,255,255);
        private Timer timer1;
        private Timer timer2;
        string _tag2 = "";
        private Timer timer3;
        Bitmap _bm;

        [Category("Shape"), Description("Additional user-defined data")]
        public string Tag2
        {
            
            get { return _tag2; }
            set
            {
                _tag2 = value;
            }
        }

        [Category("Shape"), Description("Causes the control border to animate")]
        public bool AnimateBorder
        {
            get { return _animateborder; }
            set
            {

                _animateborder = value;
                if (_animateborder)
                {
                    //save all the border
                    _prevborderstyle = _borderstyle;
                    _prevborderwidth = _borderwidth;
                    _prevbordercolor = _bordercolor;

                    if (_borderwidth == 0)
                        _borderwidth = 3;
                    int a, r, g, b;
                    a = _bordercolor.A;
                    r = _bordercolor.R;
                    g = _bordercolor.G;
                    b = _bordercolor.B;

                    _bordercolor = Color.FromArgb(255, r, g, b);


                }
                else
                {
                  
                    _borderwidth=_prevborderwidth ;
                    _bordercolor=_prevbordercolor ; 
                    this.BorderStyle =_prevborderstyle;
                }
                
                timer3.Enabled = _animateborder;
               
            }
        }

        [Category("Shape"), Description("Causes the control to blink")]
        public bool Blink
        {
            get { return _blink; }
            set { 
                
                _blink = value; 
                timer1.Enabled = _blink;
                if (!_blink) this.Visible = true;
            }
        }

        [Category("Shape"), Description("Causes the control to vibrate")]
        public bool Vibrate
        {
            get { return _vibrate; }
            set
            {

                _vibrate = value;
                timer2.Enabled = _vibrate;
                if (!_vibrate)
                    if (_voffseted) { this.Top += 5; _voffseted = false; }
            }
        }

        [Category("Shape"), Description("Background Image to define outline")]
        public Image ShapeImage
        {
            get
            {
                return _bm;
            }
            set
            {
               
  
                if (value != null)
                {
                    _bm = (Bitmap)value.Clone();
                    Width = 150;
                    Height = 150;
                    OnResize(null);
                }
                else
                {
                    if(_bm!=null) 
                       _bm = null;

                    OnResize(null);
                }
            }
        }


		[Category("Shape"),Description("Text to display")]
		public override string Text
		{
			get
			{
                
				return base.Text;
               
			}
			set
			{

				
				  base.Text = value;

			}
		}



		//Overide the BackColor Property to be associated with our custom editor
		[Category("Shape"),Description("Back Color")]
		[BrowsableAttribute(true)]
		[EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]        
		public override Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value; this.Refresh(); 

			}
		}


		[Category("Shape"),Description("Using Gradient to fill Shape")]	
		public bool UseGradient
		{
			get{ return _usegradient;}
			set{ _usegradient=value; this.Refresh();}
		}

		//For Gradient Rendering, this is the color at the center of the shape
		[Category("Shape"),Description("Color at center")]
		[BrowsableAttribute(true)]
		[EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]        
		public Color CenterColor
		{
			get {return _centercolor;}
			set { _centercolor=value; this.Refresh();}
		}

        //For Gradient Rendering, this is the color at the edges of the shape
		[Category("Shape"),Description("Color at the edges of the Shape")]
		[BrowsableAttribute(true)]
		[EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]        
		public Color SurroundColor
		{
			get {return _surroundcolor;}
			set { _surroundcolor=value; this.Refresh();}
		}


		[Category("Shape"),Description("Border Width")]
		public int BorderWidth
		{
			get{
                
                return _borderwidth;
            
            }
			set{
				_borderwidth=value;
				if (_borderwidth<0) _borderwidth=0;
                OnResize(null);		
				}
		}

		[Category("Shape"),Description("Border Color")]
		[BrowsableAttribute(true)]
		[EditorAttribute(typeof(ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]        
		public Color BorderColor
		{
			get{return _bordercolor;}
			set{_bordercolor=value;this.Refresh();}
		}

		[Category("Shape"),Description("Border Style")]
		public DashStyle BorderStyle
		{
			get{return _borderstyle;}
			set{_borderstyle=value;this.Refresh();}
		}

		[Category("Shape"),Description("Select Shape")]
		[BrowsableAttribute(true)]
		[EditorAttribute(typeof(ShapeTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]        
		public ShapeType Shape
		{
			get{return _shape;}
			set
			{
				_shape=value;
                if (_shape == ShapeType.LineVertical ||
                    _shape == ShapeType.LineHorizontal ||
                    _shape == ShapeType.LineUp ||
                    _shape == ShapeType.LineDown)
                ForeColor = Color.FromArgb(0, 255, 255, 255);

                if (_shape == ShapeType.LineVertical)
                    this.Width = 20;
                if (_shape == ShapeType.LineHorizontal)
                    this.Height = 20;
			    OnResize(null);		
			}
		}

		public CustomControl()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			this.DoubleBuffered = true;
			//Using of Double Buffer allow for smooth rendering 
			//minizing flickering
			this.SetStyle(ControlStyles.SupportsTransparentBackColor |
						  ControlStyles.DoubleBuffer |
						  ControlStyles.AllPaintingInWmPaint |
						  ControlStyles.UserPaint, true);

            //set the default backcolor and font
            this.BackColor = Color.FromArgb(0, 255, 255, 255);
            this.Font = new Font("Arial", 12, FontStyle.Bold);
            this.Width = 22;
            this.Height = 91;
            this.Text = "";
		}

		public CustomControl(string blockID, string craneID, int xPos, int yPos)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            this.DoubleBuffered = true;
			//Using of Double Buffer allow for smooth rendering 
			//minizing flickering
			this.SetStyle(ControlStyles.SupportsTransparentBackColor |
						  ControlStyles.DoubleBuffer |
						  ControlStyles.AllPaintingInWmPaint |
				          ControlStyles.UserPaint,true);

			//set the default backcolor and font
			//this.BackColor=Color.FromArgb(0,255,255,255);
			//this.Font =new Font("Arial",12,FontStyle.Bold);
			//         this.Width = 22;
			//         this.Height = 91;
			//         this.Text = "";

			//this.Shape = ShapeControl.ShapeType.Crane2;
			//this.Size = new Size(22, 91);
			//this.Location = new Point(xPos - 2, yPos - 6);
			//this.BorderWidth = 1;
			//this.BackColor = Color.Orange;
			//this.BorderColor = Color.Black;

			mBlockID = blockID;
			mCraneID = craneID;
			mXPos = xPos;
			mYPos = yPos;

			this.Location = new Point(xPos, yPos);
		}

		public void setContainerLocation(int xPos, int yPos)
		{
			mXPos = mXPos + xPos;
			mYPos = mYPos + yPos;

			this.Location = new Point(mXPos, mYPos);
		}

		public void setCranePosition(int xPos, int yPos)
		{
			mXPos = xPos;
			mYPos = yPos;

			this.Location = new Point(mXPos, mYPos);
		}

		public void setCraneLocation(int xPos, int yPos)
		{
			mXPos = xPos - 2;
			mYPos = yPos - 6;

			this.Location = new Point(mXPos, mYPos);
		}

		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if( components != null )
					components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code

		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Interval = 200;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 200;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
			// 
			// timer3
			// 
			this.timer3.Interval = 300;
			this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // CustomControl1
            // 
            this.TextChanged += new System.EventHandler(this.ShapeControl_TextChanged);
            this.ResumeLayout(false);

		}

		#endregion

		//This function creates the path for each shape
		//It is also being used by the ShapeTypeEditor to create the various shapes
		//for the Shape property editor UI
		internal static void updateOutline(ref GraphicsPath outline,ShapeType shape,int width,int height,int borderwidth)
		{
            
			switch(shape)
			{

                case ShapeType.LineVertical :
                    outline.AddPolygon(new PointF[]{
                        new PointF((float)width /2- (float)(borderwidth)/2 ,0),
                        new PointF ((float)width /2+ (float)(borderwidth)/2 ,0),
                        new PointF((float)width /2+ (float)(borderwidth)/2 ,(float)height),
                        new PointF((float)width /2- (float)(borderwidth)/2 ,(float)height)
                            }
                      );
                    break;

                case ShapeType.LineHorizontal:
                    outline.AddPolygon(new PointF[]{
                        new PointF(0, (float)height /2- (float)(borderwidth)/2 ),
                        new PointF (0,(float)height/2+ (float)(borderwidth)/2 ),
                        new PointF((float)width,(float)height/2+ (float)(borderwidth)/2 ),
                        new PointF((float)width,(float)height /2- (float)(borderwidth)/2 )
                            }
                    );
                    break;
                case ShapeType.LineUp:

                    outline.AddPolygon(new PointF[]{
                        new PointF(0,(float)height -(float)(borderwidth+1)/2 ),
                        new PointF ((float)(borderwidth+1)/2 ,(float)height),
                        new PointF((float)width,(float)(borderwidth+1)/2),
                        new PointF((float)width-(float)(borderwidth+1)/2,0)
                                    }
                    );
                    break;

                case ShapeType.LineDown :

                    outline.AddPolygon(new PointF[]{
                        new PointF((float)(borderwidth+1)/2,0),
                        new PointF (0,(float)(borderwidth+1)/2 ),
                        new PointF(width-(float)(borderwidth+1)/2,height),
                        new PointF(width,height-(float)(borderwidth+1)/2 )
                                    }
                    );
                     
                    break;
                case ShapeType.CustomPie:
					outline.AddPie(0,0,width,height,180,270);
					break;
				case ShapeType.CustomPolygon:
					outline.AddPolygon(new Point[]{
								  new Point(0,0),
						          new Point(width/2,height/4),
						          new Point(width,0),
						          new Point((width*3)/4,height/2),
						          new Point(width,height),
						          new Point(width/2,(height*3)/4),
						          new Point(0,height),
						          new Point(width/4,height/2)
												  }
						);
					break;
				case ShapeType.Diamond :
					outline.AddPolygon(new Point[]{ 
								new Point(0,height/2),
						        new Point(width/2,0),
						        new Point(width,height/2),
						        new Point(width/2,height)
												  });
					break;

				case ShapeType.Crane:
					outline.AddPolygon(new PointF[]{
						new PointF(0 ,0),
						new PointF(width ,0),
						new PointF(width , (float)height*2/10),
						new PointF((float)width/2 + (float)width/5 ,(float)height*2/10),
						new PointF((float)width/2 + (float)width/5 ,(float)height*9/10),
						new PointF(width , (float)height*9/10),
						new PointF(width , (float)height),
						new PointF(0 , (float)height),
						new PointF(0 , (float)height*9/10),
						new PointF((float)width/2 - (float)width/5 , (float)height*9/10),
						new PointF((float)width/2 - (float)width/5 , (float)height*2/10),
						new PointF(0 , (float)height*2/10)
					});
					break;

				case ShapeType.Crane2:
					outline.AddPolygon(new PointF[]{
						new PointF(0 ,0),
						new PointF(width ,0),
						new PointF(width , (float)height*5/100),
						new PointF((float)width*7/10 ,(float)height*5/100),
						new PointF((float)width*7/10 ,(float)height*95/100),
                        new PointF(width , (float)height*95/100),
                        new PointF(width , (float)height),
                        new PointF(0 , (float)height),
                        new PointF(0 , (float)height*95/100),
                        new PointF((float)width*3/10 , (float)height*95/100),
                        new PointF((float)width*3/10 , (float)height*5/100),
                        new PointF(0 , (float)height*5/100)
                    });
					break;

				case ShapeType.Crane3:
					outline.AddPolygon(new PointF[]{
						new PointF(0 ,0),
						new PointF(width ,0),
						new PointF(width , (float)height*5/100),
						new PointF((float)width*9/10 ,(float)height*5/100),
						new PointF((float)width*9/10 ,(float)height*95/100),
						new PointF(width , (float)height*95/100),
						new PointF(width , (float)height),
						new PointF(0 , (float)height),
						new PointF(0 , (float)height*95/100),
						new PointF((float)width*1/10 , (float)height*95/100),
						new PointF((float)width*1/10 , (float)height*5/100),
						new PointF(0 , (float)height*5/100)
					});
					outline.AddRectangle(new RectangleF((float)width * 25 / 100, (float)height * 5 / 100, (float)width * 50 / 100, (float)height * 40 / 100));
					outline.AddRectangle(new RectangleF((float)width * 25 / 100, (float)height * 55 / 100, (float)width * 50 / 100, (float)height * 40 / 100));

					break;

				case ShapeType.Rectangle : 
					outline.AddRectangle(new Rectangle(0,0,width,height));
					break;

				case ShapeType.Ellipse: 
					outline.AddEllipse(0,0,width,height);
					break;

				case ShapeType.TriangleUp:
					outline.AddPolygon(new Point[]{new Point(0,height),new Point(width,height),new Point(width/2,0)});
					break;

				case ShapeType.TriangleDown:
					outline.AddPolygon(new Point[]{new Point(0,0),new Point(width,0),new Point(width/2,height)});
					break;

				case ShapeType.TriangleLeft:
					outline.AddPolygon(new Point[]{new Point(width,0),new Point(0,height/2),new Point(width,height)});
					break;

				case ShapeType.TriangleRight:
					outline.AddPolygon(new Point[]{new Point(0,0),new Point(width,height/2),new Point(0,height)});
					break;

				case ShapeType.RoundedRectangle:
					outline.AddArc(0,0,width/4,width/4,180,90);
					outline.AddLine(width/8,0,width-width/8,0);
					outline.AddArc(width-width/4,0,width/4,width/4,270,90);
					outline.AddLine(width,width/8,width,height-width /8);
					outline.AddArc(width-width/4,height-width/4,width/4,width/4,0,90);
					outline.AddLine(width-width/8,height,width/8,height);
					outline.AddArc(0,height-width/4,width/4,width/4,90,90);
					outline.AddLine(0,height-width/8,0,width/8);
					break;
				
				case ShapeType.BallonSW:
					outline.AddArc(0,0,width/4,width/4,180,90);
					outline.AddLine(width/8,0,width-width/8,0);
					outline.AddArc(width-width/4,0,width/4,width/4,270,90);
					outline.AddLine(width,width/8,width,(height*0.75f)-width /8);
					outline.AddArc(width-width/4,(height *0.75f )-width/4,width/4,width/4,0,90);
					outline.AddLine(width-width/8,(height*0.75f),width/8 +(width/4),(height*0.75f));
					outline.AddLine(width/8 +(width/4),height*0.75f,width/8 +(width/8),height);
					outline.AddLine(width/8 +(width/8),height,width/8 +(width/8),(height*0.75f));
					outline.AddLine(width/8 +(width/8),(height*0.75f),width/8,(height*0.75f));
					outline.AddArc(0,(height*0.75f)-width/4,width/4,width/4,90,90);
					outline.AddLine(0,(height*0.75f)-width/8,0,width/8);
					break;

				case ShapeType.BallonSE:
					outline.AddArc(0,0,width/4,width/4,180,90);
					outline.AddLine(width/8,0,width-width/8,0);
					outline.AddArc(width-width/4,0,width/4,width/4,270,90);
					outline.AddLine(width,width/8,width,(height*0.75f)-width /8);
					outline.AddArc(width-width/4,(height *0.75f )-width/4,width/4,width/4,0,90);
					outline.AddLine(width-width/8,(height*0.75f),width-(width/4),(height*0.75f));
					outline.AddLine(width-(width/4),height*0.75f,width-(width/4),height);
					outline.AddLine(width-(width/4),height,width -(3*width/8),(height*0.75f));
					outline.AddLine(width -(3*width/8),(height*0.75f),width/8,(height*0.75f));
					outline.AddArc(0,(height*0.75f)-width/4,width/4,width/4,90,90);
					outline.AddLine(0,(height*0.75f)-width/8,0,width/8);
					break;

				case ShapeType.BallonNW:
					outline.AddArc(width-width/4,(height )-width/4,width/4,width/4,0,90);
					outline.AddLine(width-width/8,(height),width-(width/4),(height));					
					outline.AddArc(0,(height)-width/4,width/4,width/4,90,90);
					outline.AddLine(0,(height)-width/8,0,height *0.25f+width/8);
					outline.AddArc(0,height *0.25f,width/4,width/4,180,90);
					outline.AddLine(width/8,height *0.25f,width/4,height *0.25f);
					outline.AddLine(width/4,height *0.25f,width/4,0);
					outline.AddLine(width/4,0,3*width/8 ,height *0.25f);
					outline.AddLine(3*width/8 ,height *0.25f,width -width/8,height *0.25f);
					outline.AddArc(width-width/4,height *0.25f,width/4,width/4 ,270,90);
					outline.AddLine(width,width/8+height *0.25f,width,(height)-width /8);
					break;

				case ShapeType.BallonNE:
					outline.AddArc(width-width/4,(height )-width/4,width/4,width/4,0,90);
					outline.AddLine(width-width/8,(height),width-(width/4),(height));
					outline.AddArc(0,(height)-width/4,width/4,width/4,90,90);
					outline.AddLine(0,(height)-width/8,0,height *0.25f+width/8);
					outline.AddArc(0,height *0.25f,width/4,width/4,180,90);
					outline.AddLine(width/8,height *0.25f,5*width/8,height *0.25f);
					outline.AddLine(5*width/8,height *0.25f,3*width/4,0);
					outline.AddLine(3*width/4,0,3*width/4 ,height *0.25f);
					outline.AddLine(3*width/4 ,height *0.25f,width -width/8,height *0.25f);
					outline.AddArc(width-width/4,height *0.25f,width/4,width/4 ,270,90);
					outline.AddLine(width,width/8+height *0.25f,width,(height)-width /8);
					break;

				default:break;
			}
		}

		protected override void OnResize(EventArgs e)
		{
            if ((this.Width < 0) || (this.Height <= 0)) return;

            if (_bm == null)
            {

                _outline = new GraphicsPath();

                updateOutline(ref _outline, _shape, this.Width, this.Height, this.BorderWidth);
            }
            else
            {
                Bitmap bm = (Bitmap)_bm.Clone();
                Bitmap bm2 = new Bitmap(Width, Height);
                System.Diagnostics.Debug.WriteLine(bm2.Width + "," + bm2.Height);
                Graphics.FromImage(bm2).DrawImage(bm, new RectangleF(0, 0, bm2.Width, bm2.Height), new RectangleF(0, 0, bm.Width, bm.Height), GraphicsUnit.Pixel);
                TraceOutline.CTraceOuline trace = new TraceOutline.CTraceOuline();
                string s = trace.TraceOutlineN(bm2, 0, bm2.Height / 2, bm2.Width / 2, Color.Black, Color.White, true, 1);
                Point[] p = trace.StringOutline2Polygon(s);
                _outline = new GraphicsPath();
                _outline.AddPolygon(p);
            }
            if (_outline != null)
            {
                if (this.Shape == ShapeType.Rectangle)
                {
                    GraphicsPath tmp1 = (GraphicsPath)_outline.Clone();
                    GraphicsPath tmp = (GraphicsPath)_outline.Clone();
                    tmp.Widen(new Pen(_bordercolor, 1));
                    tmp1.AddPath(tmp, false);
                    tmp1.CloseAllFigures();

                    this.Region = new Region(tmp1);
                }
                else
                {
                    this.Region = new Region(_outline);
                }

            }

            this.Refresh();
            base.OnResize(e);
        }

		
		
		protected override void OnPaint(PaintEventArgs pe)
		{
			//Rendering with Gradient
			if (_usegradient)
			{
				PathGradientBrush br=new PathGradientBrush(this._outline);
				br.CenterColor=this._centercolor;
				br.SurroundColors=new Color[]{this._surroundcolor};
				pe.Graphics.FillPath(br,this._outline);
			}

			//Rendering with Border
			if(_borderwidth>0)
			{
				Pen p=new Pen(_bordercolor,_borderwidth*2);
				p.DashStyle=_borderstyle;
                if (p.DashStyle == DashStyle.Custom)
                    p.DashPattern = new float[] { 1, 1, 1, 1 };

                if(this.AnimateBorder)
                {

                    p.DashStyle =DashStyle.Custom; 
                   
                    _static_ds = (_static_ds++) % 10;


					p.DashPattern = this._btoggled ? new float[] { 0.01f + 0.05f * _static_ds, 1, 1, 1 }:new float[]{1,1,1,1};
					//p.DashPattern = this._btoggled ? new float[] { 2, 2, 2, 2 } : new float[] { 1, 1, 1, 1 };
				}
				pe.Graphics.SmoothingMode=SmoothingMode.HighQuality;
				pe.Graphics.DrawPath(p,this._outline);
				p.Dispose();
			}
				
			//Rendering the text to be at the center of the shape
			StringFormat sf=new StringFormat();
			sf.Alignment=StringAlignment.Center;
			sf.LineAlignment=StringAlignment.Center;
			switch(_shape)
			{
				case ShapeType.BallonNE:
				case ShapeType.BallonNW:
					pe.Graphics.DrawString(this.Text,this.Font,new SolidBrush(this.ForeColor),new RectangleF(0,this.Height*0.25f,this.Width ,this.Height*0.75f),sf);
					break;

				case ShapeType.BallonSE:
				case ShapeType.BallonSW:
				  pe.Graphics.DrawString(this.Text,this.Font,new SolidBrush(this.ForeColor),new RectangleF(0,0,this.Width ,this.Height*0.75f),sf);
			      break;


				case ShapeType.Crane:
					pe.Graphics.DrawString(this.Text, this.Font, new SolidBrush(this.ForeColor), new RectangleF(0, 0, this.Width, this.Height * 0.25f), sf);
					break;

				default: 
					pe.Graphics.DrawString(this.Text,this.Font,new SolidBrush(this.ForeColor),new Rectangle(0,0,this.Width ,this.Height),sf);
					break;
			}

			// Calling the base class OnPaint

			base.OnPaint(pe);
		}


		private void ShapeControl_TextChanged(object sender, System.EventArgs e)
		{
			this.Refresh();
		}

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Visible = !this.Visible;
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!_vibrate) return;
            _voffseted = !_voffseted;
            this.Top =_voffseted ?this.Top -5:this.Top +5;
  

        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            this.Refresh();
            _btoggled = !_btoggled;
        }
	}
}
