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
        float m_vX, m_vY;
        float m_diameter;

        public CStone(Model model, float x, float y, float z)
        {
            m_model = model;
            m_position.X = x;
            m_position.Y = y;
            m_position.Z = z;
            m_vX = 0;
            m_vY = 0;
            m_diameter = 3;
        }

        public void setVx(float vX)
        {
            if (Math.Abs(vX) < 0.001)
                vX = 0;
            m_vX = vX;
        }

        public float getVx()
        {
            return m_vX;
        }

        public void setVy(float vY)
        {
            if (vY > -0.001)
                vY = 0;
            m_vY = vY;
        }

        public float getVy()
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
            setVy(m_vY * 0.99f);
            setVx(m_vX * 0.99f);
        }

        public void checkCollisionWith(CStone other)
        {
            double dX = this.getPosition().X - other.getPosition().X;
            double dY = this.getPosition().Z - other.getPosition().Z;
            if (Math.Sqrt(dX * dX + dY * dY) <= m_diameter)
            {
                applyCollision(other);
            }
        }

        public void applyCollision(CStone other)
        {
            float a = m_diameter * (float)Math.Tan(Math.Atan(m_vX/m_vY) - Math.Asin(Math.Abs(other.getPosition().X - m_position.X)/m_diameter));
            float vOtherTotal = (1 - a / m_diameter) * (float)Math.Sqrt(m_vX * m_vX + m_vY * m_vY);
            float otherNewVx = other.getVx() + vOtherTotal * (other.getPosition().X - m_position.X) / (other.getPosition().Z - m_position.Z) / (float)Math.Sqrt(2) + m_vX;
            other.setVx(otherNewVx);
            float otherNewVy = other.getVy() + (float)Math.Sqrt(vOtherTotal * vOtherTotal - other.getVx() * other.getVx());
            other.setVy(-otherNewVy);
            m_vY = 0;
            m_vX = 0;
        }
    }

}