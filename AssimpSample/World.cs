// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample {


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable {
        #region Atributi


        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 20.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 500.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        //ZA TESKTURE
        private uint[] m_textures = null;
        private enum TextureObjects { WATER = 0, WOOD, METAL };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;
        private string[] m_textureFiles = {
            ".//textures//water.jpg",
            ".//textures//wood.jpg",
            ".//textures//metal.png"
        };

        //ONO ZA SLAJDERE
        //SKALIRANJE STUBOVA, RGB ZA SVETLO I POMERANJE STEPENICA
        private float size = 1f, r =1f, g = 0f, b = 0f, stairs = -15f;

        //ZA ANIMACIJU
        private float shipOffsetX = 0, shipOffsetZ = 0;//RELATIVNA POZICIJA BRODA U ODNOSU NA ONU NA KOJOJ NA POCEKTU STOJI
        private float shipRotation = 0;//ROTACIJA BRODA OKO Y-OSE
        private float stairsY = 40;//VISINA VRATA
        private float doorRotationOld = 0;//SLUZI DA SE SACUVA VREDNOST SA SLAJDERA ZA VRATA, PA KAD SE UGASI ANIMACIJA DA SE VRATA VRATE NA TU VREDNSOT
        private bool animation = false; //DA LI JE POKRENUTA AKTIVNA
        private DispatcherTimer timer; //TAJMER KOJI CE POZIVATI METODU KOJA POMERA BROD I OSTALO

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene {
            get { return m_scene; }
            set { m_scene = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX {
            get { return m_xRotation; }
            set { if(value>0 && value<90) m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance {
            get { return m_sceneDistance; }
            set { if(value>50)m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height {
            get { return m_height; }
            set { m_height = value; }
        }

        public float Stairs {
            get { return stairs; }
            set { stairs = value; }
        }
        public float Size {
            get { return size; }
            set { size = value; }
        }

        public float R {
            get { return r; }
            set { r = value; }
        }

        public float B {
            get { return b; }
            set { b = value; }
        }


        public float G {
            get { return g; }
            set { g = value; }
        }

        public bool Animation {
            get { return animation; }
            set { animation = value; }
        }

        public bool Modulate { get; set; }


        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl) {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);  //instanciranje assimp scene
            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World() {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl) {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.Enable(OpenGL.GL_CULL_FACE);

            m_scene.LoadScene();
            m_scene.Initialize();

            //DRUGA TACKA

            gl.Enable(OpenGL.GL_NORMALIZE); // normalizacija nad normalama
            gl.Enable(OpenGL.GL_COLOR_MATERIAL); //color tracking mehanizam
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE); //gl color ambijentalna i difuzna komponenta materijala

            //TEKSTURE
            InitializeTextures(gl);
            //SVETLA
            Lighting(gl);
        }

        private void InitializeTextures(OpenGL gl) {
            m_textures = new uint[m_textureCount];
            gl.Enable(OpenGL.GL_TEXTURE_2D); //ukljuci teskture
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); //stapanje sa materijalom
            gl.GenTextures(m_textureCount, m_textures); //generisemo identifikatore tekstura
            for (int i = 0; i < m_textureCount; ++i) {
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);  //bindujemo odgovarajuci identifikator
                Bitmap image = new Bitmap(m_textureFiles[i]);    //ucitavamo sliku
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);  //rotacija mora zbog koordinatnog sistema gla
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);  //da bi prebacili sliku u bit mapu
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, (int)OpenGL.GL_RGBA8, imageData.Width, imageData.Height, 0,
                            OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST); //najblizi sused filtriranje 
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT); //wrap i repeat
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
                image.UnlockBits(imageData);
                image.Dispose();
            }

        }
        //SVETLA
        private void Lighting(OpenGL gl) {
            gl.Enable(OpenGL.GL_LIGHTING); //omogucuje upotrebu svetla
            gl.Enable(OpenGL.GL_LIGHT0);//ukljuci svetlo 0, bice stacionarno
            gl.Enable(OpenGL.GL_LIGHT1);//  1 ce biti iznad mola
            float[] white = new float[] { .7f, 0.7f, 0.7f, 1.0f };

            //TACKASTI IZVOR
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, white);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, white);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, white);

            float[] pos = { 0, 2000, 0f, 1.0f };  //poz deo vertikalne ose
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, pos);//pozicija svetla 0, gore

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF,30f);//cutoff za ovo je  30

           
        }



        //POKRECE ANIMACIJU
        internal void DoAnimation() {
            animation = true;
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += new EventHandler(UpdatePositions);//poziva ovu metodu svakih 30ms
            doorRotationOld = stairs;//stara vrednost za rotacuiju vrata
            stairs = 90;//ova ide na 90, vrata stoje uz brod
            shipRotation = 20;//brod stoji ukoso dok nailazi
            stairsY = -20;//vrata su spustena
            shipOffsetZ = -80;//pomeri ga od mola
            shipOffsetX = 160;
            //SLAJDERE ISKLJUCI
            MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
            mainWindow.cb.IsEnabled= mainWindow.r.IsEnabled = mainWindow.g.IsEnabled = mainWindow.b.IsEnabled = mainWindow.doorSlider.IsEnabled = mainWindow.scaleSlider.IsEnabled = false;
            mainWindow.cb2.IsEnabled = false;
            timer.Start();
        }

        //
        private void UpdatePositions(object sender, EventArgs e) {

            if (shipOffsetZ != 0) {//NIJE DOSAO U POCETNI POLOZAJ
                shipOffsetZ++;//POMERAJ KA MOLU
                shipOffsetX -= 2;
                if (shipRotation != 0)//rotira ga dok ne bude paralelan sa molom
                    shipRotation -= 0.25f;

            }
            else {//DOSAO JE U POZICIJU
                if (stairsY != 40)//PODIZU SE VRATA DO GORE
                    stairsY++;
                else if (stairs != -15)//AKO JE PODIGAO VRTA ROTIRA IH KA MOLU
                    stairs--;
                else {//KRAJ ANIMACJE
                    timer.Stop();
                    stairs = doorRotationOld;//stara vrednost za rotacuiju vrata          
                    animation = false;
                    //UKLJUCI SLAJDERE
                    MainWindow mainWindow = (MainWindow)System.Windows.Application.Current.MainWindow;
                    mainWindow.cb.IsEnabled =mainWindow.r.IsEnabled = mainWindow.g.IsEnabled = mainWindow.b.IsEnabled = mainWindow.doorSlider.IsEnabled = mainWindow.scaleSlider.IsEnabled = true;
                    mainWindow.cb2.IsEnabled = true;

                }
            }



        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl) {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.Viewport(0, 0, m_width, m_height);

            gl.PushMatrix();
            gl.LookAt(-m_sceneDistance, 0f, 0f, 0f, 0f, 0, 0f, 1f, 0f);//sa leve strane je - deo X-ose
           // gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 0.0f, 0.0f, 1.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            ///REFLEKTOR
            float[] rc = new float[] { r, g, b, 1.0f };

            //BOJA REGLEKTORA
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, rc);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, rc);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPECULAR, rc);

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, new float[] { 0, 100, 195, 1.0f }); //svetlo iznad mola
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, new float[] { 0.0f, -1.0f, 0.0f }); //sija na dole 

            //podloga vode
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WATER]); //VODA ZA PODLOGU
            //bindujemo teksturu
            gl.MatrixMode(OpenGL.GL_TEXTURE);// UINUTAR TEXT MATRICE SE SKALIRA ,mora da bi mogle transformacije sa teksturama
            gl.PushMatrix();     //unutar texture space se sad sve ovo radi
            gl.Scale(10, 10, 1);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);  //vracamo na model view

            gl.PushMatrix();
            //gl.Translate(0.0f, 0.0f, -120f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0f, 1f, 0f);//normala za podlogu
            gl.Color(0.0f, 0.0f, 1.0f);
            gl.TexCoord(0f, 0f);//koordinate teksture za podlogu
            gl.Vertex(200f, -0.5f, -200f);
            gl.TexCoord(0f, 1f);
            gl.Vertex(-200f, -0.5f, -200f);
            gl.TexCoord(1f, 1f);
            gl.Vertex(-200f, -0.5f, 200f);
            gl.TexCoord(1f, 0f);
            gl.Vertex(200f, -0.5f, 200f);
            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WOOD]); //drvo za mol i stubove

           if(Modulate)
                gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);


            gl.Translate(0.0f, 0.0f, 120f);
            //cube
            Cube cube = new Cube();
            gl.PushMatrix();
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.Translate(0.0f, 20.0f, 70.0f);
            gl.Scale(30.0f, 5.0f, 30.0f); //dimenzije
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            //gl.Scale(size, size, size); //dimenzije

            //cilindar1
            gl.PushMatrix();
            gl.Translate(-20.0f, -2.0f, 90.0f);
            gl.Translate(0, 25, 0);//I VRATI ZA 25 NA GORE DA GORNNJA IVICA BUDE ISPOD KOCKE (3)
            gl.Scale(size, size, size);//SKLIARA SE ZA ONO SA SLEJDERA (2)
            gl.Translate(0, -25, 0);//VISINA IM JE 25, ZATO -25 DA BUDE GORANJA IVICA NA 0 JER TADA SKALIRANJE SIRI STUB SAMO DOLE (1)

            gl.Rotate(-90f, 0f, 0f);

            gl.Scale(5.0f, 5.0f, 5.0f); //dimenzije
            Cylinder cil = new Cylinder();
            cil.TextureCoords = true;//KOORDIANTE TEKSTURA ZA CILINDRE
            cil.CreateInContext(gl);
            cil.BaseRadius = 0.5;
            cil.Height = 5;
            cil.TopRadius = 0.5;
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //cilindar2
            gl.PushMatrix();
            gl.Translate(20.0f, -2.0f, 90.0f);
            gl.Translate(0, 25, 0);
            gl.Scale(size, size, size);//SKALIRANJE
            gl.Translate(0, -25, 0);
            gl.Rotate(-90f, 0f, 0f);
            gl.Scale(5.0f, 5.0f, 5.0f); //dimenzije
            cil.CreateInContext(gl);
            cil.BaseRadius = 0.5;
            cil.Height = 5;
            cil.TopRadius = 0.5;
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //cilindar3
            gl.PushMatrix();
            gl.Translate(20.0f, -2.0f, 50.0f);
            gl.Translate(0, 25, 0);
            gl.Scale(size, size, size);//SKALIRANJE
            gl.Translate(0, -25, 0);
            gl.Rotate(-90f, 0f, 0f);
            gl.Scale(5.0f, 5.0f, 5.0f); //dimenzije
            cil.CreateInContext(gl);
            cil.BaseRadius = 0.5;
            cil.Height = 5;
            cil.TopRadius = 0.5;
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();

            //cilindar4
            gl.PushMatrix();
            gl.Translate(-20.0f, -2.0f, 50.0f);
            gl.Translate(0, 25, 0);
            gl.Scale(size, size, size);//SKALIRANJE
            gl.Translate(0, -25, 0);
            gl.Rotate(-90f, 0f, 0f);
            gl.Scale(5.0f, 5.0f, 5.0f); //dimenzije
            cil.CreateInContext(gl);
            cil.BaseRadius = 0.5;
            cil.Height = 5;
            cil.TopRadius = 0.5;
            cil.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();


            //cube stepenice(vrata)
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.METAL]); //metal za stepenice
            gl.PushMatrix();
            gl.Translate(0.0f + shipOffsetX, 0.0f, 50.0f + shipOffsetZ);//pomera brod
            gl.Rotate(0, shipRotation, 0);
            Cube cubeStairs = new Cube();
            gl.PushMatrix();
            gl.Color(0.5f, 0.50f, 0.50f);
            gl.Translate(0.0f, stairsY, -53.0f);//poemri da se ivica poklopi sa brodom, stairsY KAD NEMA ANIMACIJE JE 40, TOKOM ANIMACIJE JE NA POCETKU -20 KASNIJE SE PODIZE
            gl.Rotate(-stairs, 1f, 0.0f, 0.0f);//ROTIRA ZA ONO SA SLAJDERA, TOKOM ANIMACIJE SE ISTO OVO MENJA
            gl.Scale(30.0f, 1.0f, 30.0f); //dimenzije
            gl.Translate(0, 1, 1);//pomeri se da bi se skalirao samo na gore u prema molu
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_ADD); //za brod je stapanje ADD
            gl.Translate(-20.0f, 0f, 0.0f);//priblizi se brod molu
            gl.Color(0.5f, 0.50f, 0.50f);

            gl.Scale(0.2f, 0.32f, 0.32f);
            m_scene.Draw();
            
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_DECAL); //ZA OSTALO OPET VRATIMO NA DECAL
            gl.PopMatrix();

           

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            //Tekst
            gl.PushMatrix();
            gl.DrawText(m_width - 200, 90, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "Predmet:Racunarska grafika");
            gl.DrawText(m_width - 200, 90, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "_______________________");
            gl.DrawText(m_width - 200, 70, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "Sk.god: 2020/21.");
            gl.DrawText(m_width - 200, 70, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "______________");
            gl.DrawText(m_width - 200, 50, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "Ime: Maja ");
            gl.DrawText(m_width - 200, 50, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "________");
            gl.DrawText(m_width - 200, 30, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "Prezime: Dragojlovic");
            gl.DrawText(m_width - 200, 30, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "________________");
            gl.DrawText(m_width - 200, 10, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "Sifra zad: 10.1");
            gl.DrawText(m_width - 200, 10, 1.0f, 1.0f, 0.0f, "Helvetica", 14, "____________");
            gl.PopMatrix();
            gl.Flush();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height) {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height); //viewport preko celog prozora
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60.0f, (double)width / height, 1.0f, 10000f);  //
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                m_scene.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose() {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
