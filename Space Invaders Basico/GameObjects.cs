using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Windows.Forms;

namespace SpaceInvadersBasico
{
    public class GameObject
    {
        public PictureBox Imagen { get; set; }
        public int PosX { get; set; }
        public int PosY { get; set; }
        public int Velocidad { get; set; }

        private const int scaleFactor = 3;

        public GameObject(Image imagen, int posX, int posY)
        {
            Imagen = new PictureBox();
            Imagen.Image = imagen;
            Imagen.SizeMode = PictureBoxSizeMode.StretchImage;
            Imagen.Size = new Size(imagen.Width * scaleFactor, imagen.Height * scaleFactor);
            PosX = posX;
            PosY = posY;
            Imagen.Location = new Point(PosX, PosY);
        }

        public virtual void Mover()
        {

        }
    }


    public class Tanque : GameObject
    {
        public int Vidas { get; set; } = 3;

        public Tanque(Image imagen, int posX, int posY) : base(imagen, posX, posY)
        {
        }

        public void MoverIzquierda(int desplazamiento)
        {
            PosX -= desplazamiento;
            Imagen.Location = new Point(PosX, PosY);
        }

        public void MoverDerecha(int desplazamiento)
        {
            PosX += desplazamiento;
            Imagen.Location = new Point(PosX, PosY);
        }

        public Bala Disparar()
        {
            int balaPosX = PosX + Imagen.Width / 2;
            int balaPosY = PosY - 10;
            return new Bala(Properties.Resources.Bala, balaPosX, balaPosY);
        }
    }
    public class BalaEnemiga : GameObject
    {
        public BalaEnemiga(Image imagen, int posX, int posY) : base(imagen, posX, posY)
        {
            Velocidad = 10;
        }

        public override void Mover()
        {
            PosY += Velocidad;
            Imagen.Location = new Point(PosX, PosY);
        }
    }


    public class Bala : GameObject
    {
        public Bala(Image imagen, int posX, int posY) : base(imagen, posX, posY)
        {
            Velocidad = 10;
        }

        public override void Mover()
        {
            PosY -= Velocidad;
            Imagen.Location = new Point(PosX, PosY);
        }
    }

    public class Enemigo : GameObject
    {
        public int Puntos { get; set; }
        public Image Imagen1 { get; set; }
        public Image Imagen2 { get; set; }
        private bool toggleImagen = false;

        public Enemigo(Image imagen1, Image imagen2, int puntos, int posX, int posY)
           : base(imagen1, posX, posY)
        {
            Imagen1 = imagen1;
            Imagen2 = imagen2;
            Puntos = puntos;
        }


        public void AlternarImagen()
        {
            toggleImagen = !toggleImagen;
            Imagen.Image = toggleImagen ? Imagen2 : Imagen1;
        }
    }
}
