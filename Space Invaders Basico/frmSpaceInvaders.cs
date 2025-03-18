using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.Emit;
using System.Windows.Forms;
using SpaceInvadersBasico.Properties;

namespace SpaceInvadersBasico
{
    public partial class frmSpaceInvaders : Form
    {

        Tanque tanque;
        List<Bala> balas = new List<Bala>();
        List<Enemigo> enemigos = new List<Enemigo>();
        Random random = new Random();
        List<BalaEnemiga> balasEnemigas = new List<BalaEnemiga>();
        private List<PictureBox> vidasIcons = new List<PictureBox>();



        bool movimientoDerecha = true;
        int pasoHorizontal = 10;
        int pasoVertical = 10;
        int enemyMoveElapsed = 0;
        private int puntaje = 0;

        public frmSpaceInvaders()
        {
            InitializeComponent();
            this.KeyPreview = true;
            InicializarJuego();
            pnlJuego.MouseMove += PnlJuego_MouseMove;
            pnlJuego.MouseClick += PnlJuego_MouseClick;
        }


        private void InicializarVidas()
        {
            int vidasIniciales = tanque.Vidas;

            int iconWidth = 30;
            int iconHeight = 20;
            int separation = 5;

            foreach (PictureBox pb in vidasIcons)
            {
                this.Controls.Remove(pb);
            }
            vidasIcons.Clear();

            int startX = 10;
            int startY = this.ClientSize.Height - iconHeight - 10;

            for (int i = 0; i < vidasIniciales; i++)
            {
                PictureBox pb = new PictureBox();

                pb.Image = Properties.Resources.Tanque;
                pb.SizeMode = PictureBoxSizeMode.StretchImage;
                pb.Size = new Size(iconWidth, iconHeight);
                pb.Location = new Point(startX + i * (iconWidth + separation), startY);
                pb.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
                this.Controls.Add(pb);
                pb.BringToFront();
                vidasIcons.Add(pb);
            }
        }



        private void InicializarJuego()
        {
            int posX = pnlJuego.Width / 2 - Properties.Resources.Tanque.Width / 2;
            int posY = pnlJuego.Height - Properties.Resources.Tanque.Height - 20;
            tanque = new Tanque(Properties.Resources.Tanque, posX, posY);
            pnlJuego.Controls.Add(tanque.Imagen);
            InicializarVidas();
            CrearEnemigos();
            pnlJuego.Controls.Add(lblPuntaje);
            pnlJuego.Controls.Add(lblVidas);
            timer1.Start();
        }



        private void CrearEnemigos()
        {
            int columnas = 10;
            int filas = 5;
            int scaleFactor = 3;
            int enemyWidth = Properties.Resources.Calamar1.Width * scaleFactor;
            int enemyHeight = Properties.Resources.Calamar1.Height * scaleFactor;

            int separacionHorizontal = 40;
            int separacionVertical = 10;

            int totalWidth = columnas * enemyWidth + (columnas - 1) * separacionHorizontal;
            int inicioX = (pnlJuego.Width - totalWidth) / 2;
            int inicioY = 20;

            for (int fila = 0; fila < filas; fila++)
            {
                for (int col = 0; col < columnas; col++)
                {
                    int posX = inicioX + col * (enemyWidth + separacionHorizontal);
                    int posY = inicioY + fila * (enemyHeight + separacionVertical);
                    Enemigo enemy = null;

                    if (fila == 0)
                    {
                        enemy = new Enemigo(Properties.Resources.Calamar1, Properties.Resources.Calamar2, 30, posX, posY);
                    }
                    else if (fila == 1 || fila == 2)
                    {
                        enemy = new Enemigo(Properties.Resources.Alien1, Properties.Resources.Alien2, 20, posX, posY);
                    }
                    else if (fila == 3 || fila == 4)
                    {
                        enemy = new Enemigo(Properties.Resources.Pulpo1, Properties.Resources.Pulpo2, 10, posX, posY);
                    }

                    if (enemy != null)
                    {
                        enemigos.Add(enemy);
                        pnlJuego.Controls.Add(enemy.Imagen);
                    }
                }
            }
        }




        private void frmSpaceInvaders_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
            {
                tanque.MoverIzquierda(10);
            }
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
            {
                tanque.MoverDerecha(10);
            }
            else if (e.KeyCode == Keys.Space)
            {
                Bala nuevaBala = tanque.Disparar();
                balas.Add(nuevaBala);
                pnlJuego.Controls.Add(nuevaBala.Imagen);
            }
        }
        private void PnlJuego_MouseMove(object sender, MouseEventArgs e)
        {
            int nuevaPosicionX = e.X - (tanque.Imagen.Width / 2);

            if (nuevaPosicionX < 0)
                nuevaPosicionX = 0;
            else if (nuevaPosicionX + tanque.Imagen.Width > pnlJuego.Width)
                nuevaPosicionX = pnlJuego.Width - tanque.Imagen.Width;

            tanque.Imagen.Left = nuevaPosicionX;
            tanque.PosX = nuevaPosicionX;
        }



        private void timer1_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < balas.Count; i++)
            {
                balas[i].Mover();
                if (balas[i].PosY < 0)
                {
                    pnlJuego.Controls.Remove(balas[i].Imagen);
                    balas.RemoveAt(i);
                    i--;
                }
            }

            enemyMoveElapsed += timer1.Interval;
            if (enemyMoveElapsed >= 500)
            {
                enemyMoveElapsed = 0;
                MoverEnemigos();
            }

            if (enemigos.Count > 0)
            {
                int chance = random.Next(100);
                if (chance < 20)
                {
                    int index = random.Next(enemigos.Count);
                    Enemigo shooter = enemigos[index];
                    int bulletPosX = shooter.PosX + shooter.Imagen.Width / 2;
                    int bulletPosY = shooter.PosY + shooter.Imagen.Height;
                    BalaEnemiga nuevaBalaEnemiga = new BalaEnemiga(Properties.Resources.BalaEnemiga, bulletPosX, bulletPosY);
                    balasEnemigas.Add(nuevaBalaEnemiga);
                    pnlJuego.Controls.Add(nuevaBalaEnemiga.Imagen);
                }
            }

            for (int i = 0; i < balasEnemigas.Count; i++)
            {
                balasEnemigas[i].Mover();
                if (balasEnemigas[i].PosY > pnlJuego.Height)
                {
                    pnlJuego.Controls.Remove(balasEnemigas[i].Imagen);
                    balasEnemigas.RemoveAt(i);
                    i--;
                    continue;
                }

                if (balasEnemigas[i].Imagen.Bounds.IntersectsWith(tanque.Imagen.Bounds))
                {
                    tanque.Vidas--;
    
                    lblVidas.Text = "Vidas: " + tanque.Vidas;
    
                    if (vidasIcons.Count > 0)
                    {
                        PictureBox lastIcon = vidasIcons[vidasIcons.Count - 1];
                        this.Controls.Remove(lastIcon);
                        vidasIcons.RemoveAt(vidasIcons.Count - 1);
                    }
    
                    pnlJuego.Controls.Remove(balasEnemigas[i].Imagen);
                    balasEnemigas.RemoveAt(i);
                    i--;
    
                    if (tanque.Vidas <= 0)
                    {
                        timer1.Stop();
                        DialogResult resultado = MessageBox.Show("Game Over. ¿Deseas reiniciar el juego?", "Game Over", MessageBoxButtons.YesNo);
                        if (resultado == DialogResult.Yes)
                        {
                            ReiniciarJuego();
                        }
                        else
                        {
                            this.Close();
                        }
                    }
                }
            }
                VerificarColisiones();
            if (enemigos.Count == 0)
            {
                timer1.Stop();
                DialogResult resultado = MessageBox.Show("¡Ganaste! Puntaje: " + puntaje + "\n¿Deseas reiniciar el juego?", "Victoria", MessageBoxButtons.YesNo);
                if (resultado == DialogResult.Yes)
                {
                    ReiniciarJuego();
                }
                else
                {
                    this.Close();
                }
            }
        }

        private void MoverEnemigos()
        {
            bool cambiarDireccion = false;

            foreach (var enemy in enemigos)
            {
                if (enemy.Imagen.Visible)
                {
                    if (movimientoDerecha && enemy.PosX + enemy.Imagen.Width + pasoHorizontal > pnlJuego.Width)
                    {
                        cambiarDireccion = true;
                        break;
                    }
                    else if (!movimientoDerecha && enemy.PosX - pasoHorizontal < 0)
                    {
                        cambiarDireccion = true;
                        break;
                    }
                }
            }

            if (cambiarDireccion)
            {
                foreach (var enemy in enemigos)
                {
                    enemy.PosY += pasoVertical;
                    enemy.Imagen.Location = new Point(enemy.PosX, enemy.PosY);
                    enemy.AlternarImagen();

                    if (enemy.Imagen.Bounds.IntersectsWith(tanque.Imagen.Bounds))
                    {
                        timer1.Stop();
                        MessageBox.Show("Game Over");
                        return;
                    }
                }
                movimientoDerecha = !movimientoDerecha;
            }
            else
            {
                foreach (var enemy in enemigos)
                {
                    enemy.PosX += (movimientoDerecha ? pasoHorizontal : -pasoHorizontal);
                    enemy.Imagen.Location = new Point(enemy.PosX, enemy.PosY);
                    enemy.AlternarImagen();

                    if (enemy.Imagen.Bounds.IntersectsWith(tanque.Imagen.Bounds))
                    {
                        timer1.Stop();
                        MessageBox.Show("Game Over");
                        return;
                    }
                }
            }
        }



        private void VerificarColisiones()
        {
            for (int i = 0; i < balas.Count; i++)
            {
                Bala bala = balas[i];
                for (int j = 0; j < enemigos.Count; j++)
                {
                    Enemigo enemy = enemigos[j];
                    if (enemy.Imagen.Visible && bala.Imagen.Bounds.IntersectsWith(enemy.Imagen.Bounds))
                    {
                        puntaje += enemy.Puntos;
                        lblPuntaje.Text = "Puntaje: " + puntaje;

                        pnlJuego.Controls.Remove(enemy.Imagen);
                        enemigos.RemoveAt(j);
                        j--;

                        pnlJuego.Controls.Remove(bala.Imagen);
                        balas.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }


        private void ReiniciarJuego()
        {
            timer1.Stop();
            pnlJuego.Controls.Clear();
            enemigos.Clear();
            balas.Clear();
            balasEnemigas.Clear();
            vidasIcons.Clear();
            puntaje = 0;
            tanque.Vidas = 3;
            lblPuntaje.Text = "Puntaje: " + puntaje;
            lblVidas.Text = "Vidas: " + tanque.Vidas;
            InicializarJuego();

        }

        private void PnlJuego_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Bala nuevaBala = tanque.Disparar();
                balas.Add(nuevaBala);
                pnlJuego.Controls.Add(nuevaBala.Imagen);
            }
        }

    }
}