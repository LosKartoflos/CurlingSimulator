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

        Vector2 m_increasePower;

        //Texture2D m_texture;
        Texture2D m_slider;

        float m_max, m_min;

        bool goRight;

        public Diversion(Vector2 pos)
        {
            //m_texture = texture;
            m_pos = pos;
            //float test = m_texture.Height;
            m_sliderZero = new Vector2(m_pos.X, m_pos.Y);
            m_sliderPos = m_sliderZero;
            m_increasePower = new Vector2(1, 0);
            goRight = true;
            m_max = m_pos.X + 20;
            m_min = m_pos.X - 20;
        }

        public void setSlider(Texture2D slider)
        {
            m_slider = slider;
        }

        public float getValue()
        {
            if (m_sliderPos.X > m_pos.X)
                return (m_sliderPos.X - m_pos.X) / 20;
            else
                return (m_pos.X - m_sliderPos.X) / 20 * (-1);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            //spriteBatch.Draw(m_texture, m_pos, null, Color.White, 0, m_pos, 0.05f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(m_slider, m_sliderPos, null, Color.White, 90, m_pos, 1.0f, SpriteEffects.None, 0.0f);

        }

        public void update()
        {
            if (goRight)
                m_sliderPos += m_increasePower;
            else
                m_sliderPos -= m_increasePower;

            if (m_sliderPos.X >= m_max)
                goRight = false;

            if (m_sliderPos.X <= m_min)
                goRight = true;
        }

        public void setZero()
        {
            m_sliderPos = m_sliderZero;
        }
    }
}
