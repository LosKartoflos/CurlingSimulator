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
    class Diversion
    {
        Vector2 m_pos;
        Vector2 m_sliderPos;
        Vector2 m_sliderZero;

        Vector2 m_div;

        //Texture2D m_texture;
        Texture2D m_slider;

        float m_max, m_min;

        public Diversion(Vector2 pos)
        {
            //m_texture = texture;
            m_pos = pos;
            //float test = m_texture.Height;
            m_sliderZero = new Vector2(m_pos.X + 400, m_pos.Y + 500);
            m_sliderPos = m_sliderZero;
            m_div = new Vector2(4, 0);
            m_max = m_sliderPos.X + 40;
            m_min = m_sliderPos.X - 40;
        }

        public void setSlider(Texture2D slider)
        {
            m_slider = slider;
        }

        public float getValue()
        {
            if (m_sliderPos.X > m_sliderZero.X)
                return (m_sliderPos.X - m_sliderZero.X) / 40;
            else
                return (m_sliderZero.X - m_sliderPos.X) / 40 * (-1);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(m_slider, m_sliderPos, null, Color.White, 0, m_pos, 1.0f, SpriteEffects.None, 0.0f);
        }
  
        public void moveLeft()
        {
            m_sliderPos -= m_div;

            if (m_sliderPos.X < m_min)
                m_sliderPos.X = m_min;
        }

        public void moveRight()
        {
            m_sliderPos += m_div;

            if (m_sliderPos.X > m_max)
                m_sliderPos.X = m_max;
        }

        public void setZero()
        {
            m_sliderPos = m_sliderZero;
        }
    }
}
