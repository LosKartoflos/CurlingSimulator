using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace CurlingSimulator
{
    class CStone
    {
        Vector3 m_position;
        Model m_model;
        double m_vX, m_vY;
        double m_diameter;

        public CStone(Model model, int x, int y, int z)
        {
            m_model = model;
            m_position.X = x;
            m_position.Y = y;
            m_position.Z = z;
            m_vX = 0;
            m_vY = 0;
        }

        public void setVx(float vX)
        {
            m_vX = vX;
        }

        public double getVx()
        {
            return m_vX;
        }

        public void setVy(double vY)
        {
            m_vY = vY;
        }

        public double getVy()
        {
            return m_vY;
        }

        public void setPosition(Vector3 position)
        {
            m_position.X = position.X;
            m_position.Z = position.Z;
        }

        public Vector3 getPosition()
        {
            return m_position;
        }

        public Model getModel()
        {
            return m_model;
        }

        public void applyResistance()
        {
            m_vY *= 0.8;
        }

        public void checkCollisionWith(CStone other)
        {
            double dX = this.getPosition().X - other.getPosition().X;
            double dY = this.getPosition().Y - other.getPosition().Y;
            if (Math.Sqrt(dX * dX + dY * dY) <= m_diameter)
            {
                applyCollision(other);
            }
        }

        public void applyCollision(CStone other)
        {

        }
    }

}